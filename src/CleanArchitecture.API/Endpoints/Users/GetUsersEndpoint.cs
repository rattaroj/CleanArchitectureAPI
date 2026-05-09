using Ardalis.ApiEndpoints;
using CleanArchitecture.API.Contracts.Common;
using CleanArchitecture.API.Contracts.Users;
using CleanArchitecture.API.Extensions;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Features.Users.Queries.GetUserById;
using CleanArchitecture.Application.Features.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanArchitecture.API.Endpoints.Users;

public sealed class GetUsersEndpoint(ISender sender)
    : EndpointBaseAsync
        .WithRequest<GetUsersRequest>
        .WithActionResult<ApiResponse<IReadOnlyList<UserDto>>>
{
    [HttpGet("api/users")]
    [SwaggerOperation(
        Summary = "Gets users with filtering, sorting, and pagination.",
        Description = "Filter by nameContains, emailEquals, isActive. Sort by: name, email, isactive. Direction: asc, desc.",
        OperationId = "users.getAll",
        Tags = new[] { "Users" })]
    public override async Task<ActionResult<ApiResponse<IReadOnlyList<UserDto>>>> HandleAsync(
        [FromQuery] GetUsersRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUsersQuery(
            PageNumber: request.PageNumber,
            PageSize: request.PageSize,
            SortBy: ParseSortColumn(request.SortBy),
            Direction: ParseDirection(request.Direction),
            NameContains: request.NameContains,
            EmailEquals: request.EmailEquals,
            IsActive: request.IsActive);

        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return result.ToActionResult<PagedResult<UserDto>, IReadOnlyList<UserDto>>();
        }

        return Ok(ApiResponse<UserDto>.Paged(result.Value!));
    }

    private static UserSortColumn ParseSortColumn(string? raw) =>
        raw?.Trim().ToLowerInvariant() switch
        {
            "email"    => UserSortColumn.Email,
            "isactive" => UserSortColumn.IsActive,
            _          => UserSortColumn.Name
        };

    private static SortDirection ParseDirection(string? raw) =>
        raw?.Trim().ToLowerInvariant() switch
        {
            "desc" => SortDirection.Desc,
            _      => SortDirection.Asc
        };
}
