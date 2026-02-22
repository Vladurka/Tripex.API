using BuildingBlocks.Pagination;

namespace Posts.Application.Posts.Queries.GetPosts;

public record GetPostsQuery(PaginationRequest Pagination) : IQuery<GetPostsResult>;
public record GetPostsResult(PaginatedResult<PostDto> Posts);