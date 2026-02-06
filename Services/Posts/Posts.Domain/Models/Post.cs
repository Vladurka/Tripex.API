using BuildingBlocks.Cache;

namespace Posts.Domain.Models;

public class Post : Entity<PostId>, ICachable
{
    public ProfileId ProfileId { get; private set; }
    public ContentUrl ContentUrl { get; private set; }
    public string? Description { get; private set; }
    public bool IsCached { get; private set; }

    private Post() { }

    public static Post Create(PostId id, ProfileId profileId, 
        ContentUrl contentUrlUrl, string? description, DateTime createdAt)
    {
        return new Post
        {
            Id = id,
            ProfileId = profileId,
            ContentUrl = contentUrlUrl,
            Description = description,
            CreatedAt = createdAt
        };
    }

    public void SetIsCached(bool value) => 
        IsCached = value;
}