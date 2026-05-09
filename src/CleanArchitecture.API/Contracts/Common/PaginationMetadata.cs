using CleanArchitecture.Application.Common.Models;

namespace CleanArchitecture.API.Contracts.Common;

public sealed record PaginationMetadata(
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage)
{
    public static PaginationMetadata From<T>(PagedResult<T> page)
    {
        ArgumentNullException.ThrowIfNull(page);

        return new PaginationMetadata(
            page.PageNumber,
            page.PageSize,
            page.TotalCount,
            page.TotalPages,
            page.HasPreviousPage,
            page.HasNextPage);
    }
}
