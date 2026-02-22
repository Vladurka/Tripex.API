namespace Posts.Application.Data;

public interface IPostRepository
{
    public Task AddPostAsync(Post postById);
    public Task<Post?> GetPostByIdAsync(PostId id);
    public Task<IEnumerable<Post>> GetPostsByProfileAsync(ProfileId profileId);
    public Task<IEnumerable<Guid>> GetPostIdsByUserAsync(ProfileId profileId);
    public Task<IEnumerable<Post>> GetAllPostsAsync(int pageIndex = 0, int pageSize = 10);
    public Task<long> GetTotalCountAsync();
    public Task DeletePostAsync(PostId id);
    public Task DeletePostsAsync(ProfileId profileId);
}