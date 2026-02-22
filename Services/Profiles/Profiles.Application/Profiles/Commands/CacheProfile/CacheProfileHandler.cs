namespace Profiles.Application.Profiles.Commands.CacheProfile;

public class CacheProfileHandler(IProfilesRedisRepository redisRepo, IProfilesRepository repo) 
    : ICommandHandler<CacheProfileCommand>
{
    public async Task<Unit> Handle(CacheProfileCommand command, CancellationToken cancellationToken)
    {
        var profile = await repo.GetProfileByIdAsync(ProfileId.Of(command.ProfileId), cancellationToken) ??
                      throw new NotFoundException("Profile", command.ProfileId);

        await redisRepo.CacheProfileAsync(profile);
        
        return Unit.Value;
    }
}