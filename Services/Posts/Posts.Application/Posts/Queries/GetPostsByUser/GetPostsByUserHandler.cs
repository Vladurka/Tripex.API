using Posts.Application.Data;
using Posts.Application.Posts.Extensions;

namespace Posts.Application.Posts.Queries.GetPostsByUser;

public class GetPostsByUserHandler (IPostRepository repo, IPostsRedisRepository redisRepo)
    : IQueryHandler<GetPostsByUserQuery, GetPostsByUserResult>
{
    public async Task<GetPostsByUserResult> Handle(GetPostsByUserQuery query, CancellationToken cancellationToken)
    {
        var profileId = ProfileId.Of(query.ProfileId);
        
        var posts = await redisRepo.GetCachedPostsAsync(profileId);
        
        posts ??= await repo.GetPostsByProfileAsync(profileId);
        
        return new GetPostsByUserResult(posts.Select(p => p.ToDto()));
    }
}