using Profiles.Application.Profiles.Queries;

namespace Profiles.Application.Profiles.Commands.UpdateProfile;

public class UpdateProfileHandler(
    IProfilesRepository repo, IOutboxRepository outboxRepo,
    IProfilesRedisRepository redisRepo) 
    : ICommandHandler<UpdateProfileCommand, GetProfileResult>
{
    public async Task<GetProfileResult> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        await using var transaction = await repo.BeginTransactionAsync(cancellationToken);
        try
        {
            var profile = await repo.GetProfileByIdAsync(ProfileId.Of(command.ProfileId), cancellationToken, false) ??
                throw new NotFoundException("Profile", command.ProfileId);

            profile.Update(command.FirstName, 
                command.LastName, command.Description);
            
            await repo.SaveChangesAsync(cancellationToken, false);
            await redisRepo.UpdateBasicInfoAsync(profile);
            

            if (!string.IsNullOrWhiteSpace(command.ProfileName) && profile.ProfileName.Value != command.ProfileName)
            {
                if (await repo.ProfileNameExistsAsync(command.ProfileName, cancellationToken))
                    throw new ExistsException("Profile", "username" , command.ProfileName);

                profile.UpdateProfileName(ProfileName.Of(command.ProfileName));
                profile.LastModified = DateTime.UtcNow;
                await repo.SaveChangesAsync(cancellationToken);
                await redisRepo.UpdateBasicInfoAsync(profile);

                var eventMessage = command.Adapt<UpdateUserNameEvent>();

                var outboxMessage = new OutboxMessage(typeof(UpdateUserNameEvent).AssemblyQualifiedName!,
                    JsonSerializer.Serialize(eventMessage));

                await outboxRepo.AddOutboxMessageAsync(outboxMessage);
            }
            
            await redisRepo.UpdateProfileAsync(profile);

            await transaction.CommitAsync(cancellationToken);

            return new GetProfileResult(
                profile.Id.Value,
                profile.ProfileName.Value,
                profile.AvatarUrl,
                profile.FirstName,
                profile.LastName,
                profile.Description,
                profile.FollowerCount,
                profile.FollowingCount
            );
        }
        
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
