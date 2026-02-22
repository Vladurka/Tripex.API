using System.Text.Json;
using BuildingBlocks.Exceptions;
using Posts.Application.Data;
using Posts.Application.Posts.DTO;
using Posts.Application.Posts.Extensions;
using Posts.Domain.Models;
using Posts.Domain.ValueObjects;
using StackExchange.Redis;

namespace Posts.Infrastructure.Data;

public class PostsRedisRepository : IPostsRedisRepository
{
    private readonly IDatabase _redisDb;
    private static readonly TimeSpan PostsTtl = TimeSpan.FromMinutes(15);

    public PostsRedisRepository(IConnectionMultiplexer redis) =>
        _redisDb = redis.GetDatabase();

    public async Task CachePostsAsync(IEnumerable<Post> posts, ProfileId profileId)
    {
        var key = ConvertToKey(profileId);
        var value = JsonSerializer.Serialize(posts.Select(p => p.ToCachedPostDto()));
        await _redisDb.StringSetAsync(key, value, PostsTtl);
    }

    public async Task<IEnumerable<Post>?> GetCachedPostsAsync(ProfileId profileId)
    {
        var value = await _redisDb.StringGetAsync(ConvertToKey(profileId));
        return value.IsNullOrEmpty
            ? null
            : JsonSerializer.Deserialize<IEnumerable<CachedPostDto>>(value!)!.Select(p => p.ToDomain());
    }

    public Task<bool> ArePostsCachedAsync(ProfileId profileId) =>
        _redisDb.KeyExistsAsync(ConvertToKey(profileId));

    public async Task AddPostAsync(Post post)
    {
        var key = ConvertToKey(post.ProfileId);
        var value = await _redisDb.StringGetAsync(key);

        IEnumerable<Post> posts = value.IsNullOrEmpty
            ? new[] { post }
            : JsonSerializer.Deserialize<IEnumerable<CachedPostDto>>(value!)!.Select(p => p.ToDomain()).Append(post);

        var serialized = JsonSerializer.Serialize(posts.Select(p => p.ToCachedPostDto()));
        await _redisDb.StringSetAsync(key, serialized, PostsTtl);
    }

    public async Task DeletePostAsync(PostId postId, ProfileId profileId)
    {
        var key = ConvertToKey(profileId);
        var value = await _redisDb.StringGetAsync(key);

        if (value.IsNullOrEmpty)
           return;
        
        var posts = JsonSerializer.Deserialize<List<CachedPostDto>>(value!)!;
    
        var initialCount = posts.Count;
        var updatedPosts = posts.Where(p => p.Id != postId.Value).ToList();

        if (updatedPosts.Count == initialCount)
            return;

        var serialized = JsonSerializer.Serialize(updatedPosts);
        await _redisDb.StringSetAsync(key, serialized, PostsTtl);
    }

    public async Task DeletePostsByProfileAsync(ProfileId profileId)=>
        await _redisDb.KeyDeleteAsync(ConvertToKey(profileId));

    private static string ConvertToKey(ProfileId profileId) =>
        $"posts:profile:{profileId.Value}";
}
