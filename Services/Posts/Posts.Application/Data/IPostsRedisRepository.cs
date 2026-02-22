namespace Posts.Application.Data;

public interface IPostsRedisRepository
{
    public Task CachePostsAsync(IEnumerable<Post> posts, ProfileId profileId);
    public Task<IEnumerable<Post>?> GetCachedPostsAsync(ProfileId profileId);
    public Task<bool> ArePostsCachedAsync(ProfileId profileId);
    public Task AddPostAsync(Post post);
    public Task DeletePostAsync(PostId postId, ProfileId profileId);
    public Task DeletePostsByProfileAsync(ProfileId profileId);
}