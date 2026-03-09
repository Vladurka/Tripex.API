using BuildingBlocks.Exceptions;
using Cassandra;
using Cassandra.Data.Linq;
using Posts.Application.Data;
using Posts.Application.Posts.DTO;
using Posts.Application.Posts.Extensions;
using Posts.Domain.Models;
using Posts.Domain.ValueObjects;

namespace Posts.Infrastructure.Data;

public class PostRepository(ISession session) : IPostRepository
{
    private readonly Table<PostByIdDb> _postsById = new(session);
    private readonly Table<PostByProfileDb> _postsByProfile = new(session);

    public async Task AddPostAsync(Post post)
    {
        
        await _postsById.Insert(post.ToDbById()).ExecuteAsync();
        await _postsByProfile.Insert(post.ToDbByProfile()).ExecuteAsync();
    }

    public async Task<Post?> GetPostByIdAsync(PostId id)
    {
        var result = await _postsById
            .Where(p => p.Id == id.Value)
            .ExecuteAsync();

        var db = result.FirstOrDefault();
        return db?.ToDomain();
    }

    public async Task<IEnumerable<Post>> GetPostsByProfileAsync(ProfileId profileId)
    { 
        var result = await _postsByProfile
            .Where(p => p.ProfileId == profileId.Value)
            .ExecuteAsync();

        return result.Select(PostMapper.ToDomain);
    }
    
    public async Task<IEnumerable<Guid>> GetPostIdsByUserAsync(ProfileId profileId)
    { 
        var result = await _postsByProfile
            .Where(p => p.ProfileId == profileId.Value)
            .Select(p => p.Id)
            .ExecuteAsync();

        return result;
    }

    public async Task<IEnumerable<Post>> GetAllPostsAsync(int pageIndex = 0, int pageSize = 10)
    {
        var result = await _postsById.Select(_ => _).ExecuteAsync();
        return result
            .OrderByDescending(p => p.CreatedAt)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Select(PostMapper.ToDomain)
            .ToList();
    }

    public async Task<long> GetTotalCountAsync()
    {
        var result = await _postsById.Count().ExecuteAsync();
        return result;
    }

    public async Task DeletePostAsync(PostId id)
    {
        var post = await _postsById
            .FirstOrDefault(p => p.Id == id.Value)
            .ExecuteAsync();

        if (post == null)
            throw new NotFoundException("Post", id.Value);

        await _postsById
            .Where(p => p.Id == id.Value)
            .Delete()
            .ExecuteAsync();

        await _postsByProfile
            .Where(p => p.ProfileId == post.ProfileId && p.Id == post.Id)
            .Delete()
            .ExecuteAsync();
    }
    
    public async Task DeletePostsAsync(ProfileId profileId)
    {
        var postIds = await _postsByProfile
            .Where(p => p.ProfileId == profileId.Value)
            .Select(p => p.Id)
            .ExecuteAsync();

        var ids = postIds.ToList();
        if (ids.Count == 0) return;

        var batch = new BatchStatement();

        foreach (var id in ids)
        {
            batch.Add(_postsById
                .Where(p => p.Id == id)
                .Delete()
                .SetConsistencyLevel(ConsistencyLevel.LocalQuorum));
        }

        batch.Add(_postsByProfile
            .Where(p => p.ProfileId == profileId.Value)
            .Delete()
            .SetConsistencyLevel(ConsistencyLevel.LocalQuorum));

        await session.ExecuteAsync(batch);
    }

}