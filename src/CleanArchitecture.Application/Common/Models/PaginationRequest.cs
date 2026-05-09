namespace CleanArchitecture.Application.Common.Models;

public sealed record PaginationRequest(int PageNumber = 1, int PageSize = 10)
{
    public const int MaxPageSize = 100;

    public int PageNumber { get; } = Math.Max(1, PageNumber);
    public int PageSize { get; } = Math.Clamp(PageSize, 1, MaxPageSize);

    public int Skip => (PageNumber - 1) * PageSize;
}
