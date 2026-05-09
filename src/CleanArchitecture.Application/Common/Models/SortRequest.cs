namespace CleanArchitecture.Application.Common.Models;

public sealed record SortRequest<TColumn>(
    TColumn SortBy,
    SortDirection Direction = SortDirection.Asc)
    where TColumn : struct, Enum;
