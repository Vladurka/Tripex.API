using BuildingBlocks.Pagination;

namespace Profiles.Application.Profiles.Queries.GetProfiles;

public record GetProfilesQuery(PaginationRequest Pagination) : IQuery<GetProfilesResult>; 
public record GetProfilesResult(PaginatedResult<GetProfileResult> Profiles);