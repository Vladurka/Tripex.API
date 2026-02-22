namespace Profiles.Application.Profiles.Commands.UpdateAvatar;

public class UpdateAvatarHandler(IProfilesRepository repo, IProfilesRedisRepository redisRepo,
    IBlobStorageService blobStorage) : ICommandHandler<UpdateAvatarCommand, UpdateAvatarResult>
{
    public async Task<UpdateAvatarResult> Handle(UpdateAvatarCommand command, CancellationToken cancellationToken)
    {
        var profile = await repo.GetProfileByIdAsync(ProfileId.Of(command.ProfileId), cancellationToken, false) ??
            throw new NotFoundException("Profile", command.ProfileId);
    
        if (command.Avatar == null)
        {
            await blobStorage.DeletePhotoAsync(command.ProfileId, cancellationToken);
            profile.UpdateAvatar(string.Empty);
            await repo.SaveChangesAsync(cancellationToken);
            await redisRepo.UpdateProfileAsync(profile);
            
            return new UpdateAvatarResult(profile.AvatarUrl);
        }
        
        profile.UpdateAvatar(await blobStorage.UploadPhotoAsync(command.Avatar, command.ProfileId, cancellationToken));
        profile.LastModified = DateTime.UtcNow;
        
        await redisRepo.UpdateBasicInfoAsync(profile);
        await redisRepo.UpdateProfileAsync(profile);
        await repo.SaveChangesAsync(cancellationToken);

        return new UpdateAvatarResult(profile.AvatarUrl);
    }
}