using Ardalis.ApiEndpoints;
using CleanArchitecture.API.Contracts.Common;
using CleanArchitecture.API.Extensions;
using CleanArchitecture.Application.Features.Roles.Queries.GetUserRoles;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanArchitecture.API.Endpoints.Roles;

public sealed class GetUserRolesEndpoint(ISender sender)
    : EndpointBaseAsync
        .WithoutRequest
        .WithActionResult<ApiResponse<IReadOnlyList<UserRoleDto>>>
{
    [HttpGet("api/users/{id:guid}/roles")]
    [SwaggerOperation(
        Summary = "Gets roles of a user.",
        Description = "Returns all roles assigned to the specified user.",
        OperationId = "users.getRoles",
        Tags = new[] { "Roles" })]
    public override async Task<ActionResult<ApiResponse<IReadOnlyList<UserRoleDto>>>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        if (!HttpContext.Request.RouteValues.TryGetValue("id", out var routeId)
            || !Guid.TryParse(routeId?.ToString(), out var userId))
        {
            return BadRequest(ApiResponse<IReadOnlyList<UserRoleDto>>.Fail("Users.InvalidId", "Invalid user ID."));
        }

        var result = await sender.Send(new GetUserRolesQuery(userId), cancellationToken);

        if (result.IsFailure)
            return result.ToActionResult();

        return Ok(ApiResponse<IReadOnlyList<UserRoleDto>>.Ok(result.Value!));
    }
}
