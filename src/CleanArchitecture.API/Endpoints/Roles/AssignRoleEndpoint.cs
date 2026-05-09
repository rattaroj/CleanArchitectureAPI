using Ardalis.ApiEndpoints;
using CleanArchitecture.API.Contracts.Common;
using CleanArchitecture.API.Contracts.Roles;
using CleanArchitecture.Application.Features.Roles.Commands.AssignRole;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanArchitecture.API.Endpoints.Roles;

public sealed class AssignRoleEndpoint(ISender sender)
    : EndpointBaseAsync
        .WithRequest<AssignRoleRequest>
        .WithActionResult<ApiResponse<object>>
{
    [HttpPost("api/users/{id:guid}/roles")]
    [SwaggerOperation(
        Summary = "Assigns a role to a user.",
        Description = "Assigns the specified role to the user. Returns 409 if the role is already assigned.",
        OperationId = "users.assignRole",
        Tags = new[] { "Roles" })]
    public override async Task<ActionResult<ApiResponse<object>>> HandleAsync(
        [FromBody] AssignRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!HttpContext.Request.RouteValues.TryGetValue("id", out var routeId)
            || !Guid.TryParse(routeId?.ToString(), out var userId))
        {
            return BadRequest(ApiResponse<object>.Fail("Users.InvalidId", "Invalid user ID."));
        }

        var command = new AssignRoleCommand(userId, request.RoleId);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code switch
            {
                "Users.UserNotFound" => NotFound(ApiResponse<object>.Fail(result.Error.Code, result.Error.Message)),
                "Roles.RoleNotFound" => NotFound(ApiResponse<object>.Fail(result.Error.Code, result.Error.Message)),
                "Roles.AlreadyAssigned" => Conflict(ApiResponse<object>.Fail(result.Error.Code, result.Error.Message)),
                _ => BadRequest(ApiResponse<object>.Fail(result.Error.Code, result.Error.Message))
            };
        }

        return Ok(ApiResponse<object>.Ok(new { }, "Role assigned successfully."));
    }
}
