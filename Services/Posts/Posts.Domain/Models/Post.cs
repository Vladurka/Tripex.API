namespace Posts.Domain.Models;

public class Post : Entity<PostId>
{
    public ProfileId ProfileId { get; private set; }
    public ContentUrl ContentUrl { get; private set; }
    public string? Description { get; private set; }

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
}