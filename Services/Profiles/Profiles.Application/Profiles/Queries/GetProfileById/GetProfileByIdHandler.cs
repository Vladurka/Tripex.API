using BuildingBlocks.Messaging.Events.Cache;

namespace Profiles.Application.Profiles.Queries.GetProfileById;

public class GetProfileByIdHandler(IProfilesRepository repo, IOutboxRepository outboxRepo,
    IProfilesRedisRepository redisRepo) : IQueryHandler<GetProfileByIdQuery, GetProfileResult>
{
    public async Task<GetProfileResult> Handle(GetProfileByIdQuery query, CancellationToken cancellationToken)
    {
        var profileId = ProfileId.Of(query.ProfileId);
        var profile = await redisRepo.GetCachedProfileAsync(profileId);
        
        if (profile == null)
        {
            profile = await repo.GetProfileByIdAsync(profileId, cancellationToken, false) ?? 
                throw new NotFoundException("Profile", query.ProfileId);

            if (profile.ShouldBeCached())
            {
                var eventMessage = query.Adapt<CacheUserEvent>();
                var outboxMessage = new OutboxMessage(typeof(CacheUserEvent).AssemblyQualifiedName!,
                    JsonSerializer.Serialize(eventMessage));
                await outboxRepo.AddOutboxMessageAsync(outboxMessage);
            }
            
            await repo.SaveChangesAsync(cancellationToken, false);
        }

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
}