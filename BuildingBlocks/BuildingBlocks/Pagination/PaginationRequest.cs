namespace BuildingBlocks.Pagination;

public record PaginationRequest(int PageIndex = 0, int PageSize = 10)
{
    public int PageIndex { get; } = Math.Max(0, PageIndex);
    public int PageSize { get; } = Math.Clamp(PageSize, 1, 100);
}
