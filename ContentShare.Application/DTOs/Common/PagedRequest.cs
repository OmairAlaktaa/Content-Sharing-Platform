namespace ContentShare.Application.DTOs.Common;

public class PagedRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    /// <summary>title | -title | created_at | -created_at (default -created_at)</summary>
    public string? Sort { get; init; }

    /// <summary>Search by title contains</summary>
    public string? Search { get; init; }

    /// <summary>game | video | artwork | music</summary>
    public string? Category { get; init; }
}
