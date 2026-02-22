using BuildingBlocks.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Profiles.Application.Profiles.Queries.GetProfiles;

public class GetProfilesHandler(IProfilesRepository repo) 
    : IQueryHandler<GetProfilesQuery, GetProfilesResult> 
{
    public async Task<GetProfilesResult> Handle(GetProfilesQuery query, CancellationToken cancellationToken)
    {
        var pageIndex = query.Pagination.PageIndex;
        var pageSize = query.Pagination.PageSize;

        var totalCount = await repo.GetQueryable().LongCountAsync(cancellationToken);

        var profiles = await repo.GetQueryable()
            .OrderByDescending(p => p.CreatedAt)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Select(p => new GetProfileResult(
                p.Id.Value,
                p.ProfileName.Value,
                p.AvatarUrl,
                p.FirstName,
                p.LastName,
                p.Description,
                p.FollowerCount,
                p.FollowingCount))
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        return new GetProfilesResult(
            new PaginatedResult<GetProfileResult>(pageIndex, pageSize, totalCount, profiles));
    }
}