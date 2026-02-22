namespace Profiles.Application.Profiles.Commands.IncrementPostCount;

public class IncrementPostCoutHandler(IProfilesRepository repo, 
    IProfilesRedisRepository redisRepo) : ICommandHandler<IncrementPostCountCommand>
{
    public async Task<Unit> Handle(IncrementPostCountCommand command, CancellationToken cancellationToken)
    {
        var profileId = ProfileId.Of(command.ProfileId);

        var profile = await repo.GetProfileByIdAsync(profileId, cancellationToken, false) ??
            throw new NotFoundException("Profile", profileId.Value);
        
        profile.IncrementPostCount();
        
        await Task.WhenAll(
            repo.SaveChangesAsync(cancellationToken),
            redisRepo.UpdateProfileAsync(profile)
        );
        
        return Unit.Value;   
    }
}