using Ardalis.ApiEndpoints;
using CleanArchitecture.API.Contracts.Common;
using CleanArchitecture.Application.Features.Roles.Commands.RemoveRole;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanArchitecture.API.Endpoints.Roles;

public sealed class RemoveRoleEndpoint(ISender sender)
    : EndpointBaseAsync
        .WithoutRequest
        .WithActionResult<ApiResponse<object>>
{
    [HttpDelete("api/users/{id:guid}/roles/{roleId:guid}")]
    [SwaggerOperation(
        Summary = "Removes a role from a user.",
        Description = "Removes the specified role from the user.",
        OperationId = "users.removeRole",
        Tags = new[] { "Roles" })]
    public override async Task<ActionResult<ApiResponse<object>>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        if (!HttpContext.Request.RouteValues.TryGetValue("id", out var routeId)
            || !Guid.TryParse(routeId?.ToString(), out var userId))
        {
            return BadRequest(ApiResponse<object>.Fail("Users.InvalidId", "Invalid user ID."));
        }

        if (!HttpContext.Request.RouteValues.TryGetValue("roleId", out var routeRoleId)
            || !Guid.TryParse(routeRoleId?.ToString(), out var roleId))
        {
            return BadRequest(ApiResponse<object>.Fail("Roles.InvalidId", "Invalid role ID."));
        }

        var command = new RemoveRoleCommand(userId, roleId);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code switch
            {
                "Users.UserNotFound" => NotFound(ApiResponse<object>.Fail(result.Error.Code, result.Error.Message)),
                "Roles.NotAssigned" => NotFound(ApiResponse<object>.Fail(result.Error.Code, result.Error.Message)),
                _ => BadRequest(ApiResponse<object>.Fail(result.Error.Code, result.Error.Message))
            };
        }

        return Ok(ApiResponse<object>.Ok(new { }, "Role removed successfully."));
    }
}
