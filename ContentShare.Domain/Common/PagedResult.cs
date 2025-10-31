namespace ContentShare.Domain.Common;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; }
    public int Total { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }

    public PagedResult(IReadOnlyList<T> items, int total, int page, int pageSize)
        => (Items, Total, Page, PageSize) = (items, total, page, pageSize);
}
