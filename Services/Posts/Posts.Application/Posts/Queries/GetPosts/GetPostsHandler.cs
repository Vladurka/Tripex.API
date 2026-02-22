using BuildingBlocks.Pagination;
using Posts.Application.Data;
using Posts.Application.Posts.Extensions;

namespace Posts.Application.Posts.Queries.GetPosts;

public class GetPostsHandler(IPostRepository repo) 
    : IQueryHandler<GetPostsQuery, GetPostsResult>
{
    public async Task<GetPostsResult> Handle(GetPostsQuery query, CancellationToken cancellationToken)
    {
        var pageIndex = query.Pagination.PageIndex;
        var pageSize = query.Pagination.PageSize;

        var posts = await repo.GetAllPostsAsync(pageIndex, pageSize);
        var totalCount = await repo.GetTotalCountAsync();

        return new GetPostsResult(
            new PaginatedResult<PostDto>(pageIndex, pageSize, totalCount,
                posts.Select(x => x.ToDto())));
    }
}