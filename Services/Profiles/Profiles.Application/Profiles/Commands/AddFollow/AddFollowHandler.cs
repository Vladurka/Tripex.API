namespace Profiles.Application.Profiles.Commands.AddFollow;

public class AddFollowHandler(IProfilesRepository repo) : ICommandHandler<AddFollowCommand>
{
    public async Task<Unit> Handle(AddFollowCommand command, CancellationToken cancellationToken)
    {
        var profileId = ProfileId.Of(command.ProfileId);
        var profile = await repo.GetProfileByIdAsync(profileId, cancellationToken, false) ??
                      throw new NotFoundException("Profile", command.ProfileId);
        
        var followerId = ProfileId.Of(command.FollowerId);
        var follower = await repo.GetProfileByIdAsync(followerId, cancellationToken, false) ??
                      throw new NotFoundException("Profile", command.FollowerId);
        
        profile.AddFollower();
        follower.AddFollowing();
        await repo.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}