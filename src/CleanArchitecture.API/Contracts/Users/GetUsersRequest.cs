using Swashbuckle.AspNetCore.Annotations;

namespace CleanArchitecture.API.Contracts.Users;

public sealed class GetUsersRequest
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    [SwaggerSchema(Description = "Column to sort by. Allowed values: name, email, isactive. Default: name")]
    public string? SortBy { get; init; }

    [SwaggerSchema(Description = "Sort direction. Allowed values: asc, desc. Default: asc")]
    public string? Direction { get; init; }

    public string? NameContains { get; init; }
    public string? EmailEquals { get; init; }
    public bool? IsActive { get; init; }
}
