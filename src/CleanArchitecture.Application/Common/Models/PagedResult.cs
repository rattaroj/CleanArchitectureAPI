namespace CleanArchitecture.Application.Common.Models;

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => TotalCount == 0
        ? 0
        : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public static PagedResult<T> Create(
        IReadOnlyList<T> items,
        int pageNumber,
        int pageSize,
        int totalCount)
    {
        ArgumentNullException.ThrowIfNull(items);

        var request = new PaginationRequest(pageNumber, pageSize);
        return new PagedResult<T>(
            items,
            request.PageNumber,
            request.PageSize,
            Math.Max(0, totalCount));
    }

    public PagedResult<TTarget> Map<TTarget>(Func<T, TTarget> mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);

        return new PagedResult<TTarget>(
            Items.Select(mapper).ToList(),
            PageNumber,
            PageSize,
            TotalCount);
    }
}
