using BuildingBlocks.AzureBlob;
using BuildingBlocks.ContentModeration;
using BuildingBlocks.Messaging.Events.Cache;
using BuildingBlocks.Messaging.Events.Profiles;
using Mapster;
using MassTransit;
using Posts.Application.Data;
using Posts.Application.Posts.Extensions;

namespace Posts.Application.Posts.Commands.CreatePost;

public class CreatePostHandler(IPostRepository repo, IBlobStorageService blobStorageService,
    IPostsRedisRepository redisRepo, IPublishEndpoint publishEndpoint,
    IContentModerationService moderationService) : ICommandHandler<CreatePostCommand, CreatePostResult>
{
    public async Task<CreatePostResult> Handle(CreatePostCommand command, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        
        if (!await moderationService.ModeratePhoto(command.Photo))
            throw new BadRequestException("This photo is not allowed");

        var url = await blobStorageService.UploadPhotoAsync(command.Photo, id, cancellationToken);

        var post = Post.Create(PostId.Of(id), ProfileId.Of(command.ProfileId), ContentUrl.Of(url), command.Description, DateTime.UtcNow);
        
        var cacheInfoMessage = command.Adapt<CacheBasicInfoEvent>();
        var incrementPostCountMessage = command.Adapt<IncrementPostCountEvent>();
        
        var profileId = ProfileId.Of(command.ProfileId);
        
        var arePostsCachedTask = redisRepo.ArePostsCachedAsync(profileId);

        var tasks = new List<Task>
        {
            repo.AddPostAsync(post),
            publishEndpoint.Publish(cacheInfoMessage, cancellationToken),
            publishEndpoint.Publish(incrementPostCountMessage, cancellationToken),
            arePostsCachedTask 
        };

        await Task.WhenAll(tasks);
        
        if (arePostsCachedTask.Result) 
            await redisRepo.AddPostAsync(post);

        return new CreatePostResult(post.Id.Value);
    }
}
