using BuildingBlocks.Exceptions;
using Cassandra.Data.Linq;
using Microsoft.Extensions.Options;
using Posts.Application.Data;
using Posts.Application.Posts.DTO;
using Posts.Application.Posts.Extensions;
using Posts.Domain.Models;
using Posts.Domain.ValueObjects;

namespace Posts.Infrastructure.Data;

public class PostRepository : IPostRepository
{
    private readonly Table<PostByIdDb> _postsById;
    private readonly Table<PostByProfileDb> _postsByProfile;

    public PostRepository(IOptions<ScyllaDbSettings> options)
    {
        var session = ScyllaDbSession.Connect(options);
        _postsById = new Table<PostByIdDb>(session);
        _postsByProfile = new Table<PostByProfileDb>(session);
    }

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
        Console.WriteLine(db?.CreatedAt);
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

    public async Task<IEnumerable<Post>> GetAllPostsAsync()
    {
        var result = await _postsById.Select(_ => _).ExecuteAsync();
        return result.Select(PostMapper.ToDomain).ToList();
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
        var posts = await _postsById
            .Where(p => p.ProfileId == profileId.Value)
            .ExecuteAsync();

        foreach (var post in posts)
        {
            await _postsById
                .Where(p => p.Id == post.Id)
                .Delete()
                .ExecuteAsync();

            await _postsByProfile
                .Where(p => p.ProfileId == post.ProfileId && p.Id == post.Id)
                .Delete()
                .ExecuteAsync();
        }
    }

}