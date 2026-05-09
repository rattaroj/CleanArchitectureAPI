using Ardalis.ApiEndpoints;
using CleanArchitecture.API.Contracts.Common;
using CleanArchitecture.API.Contracts.Users;
using CleanArchitecture.API.Extensions;
using CleanArchitecture.Application.Features.Users.Commands.UpdateUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanArchitecture.API.Endpoints.Users;

public sealed class UpdateUserEndpoint(ISender sender)
    : EndpointBaseAsync
        .WithRequest<UpdateUserRequest>
        .WithActionResult<ApiResponse<UpdateUserResponse>>
{
    [HttpPut("api/users/{id:guid}", Name = "Users.Update")]
    [SwaggerOperation(
        Summary = "Updates a user.",
        Description = "Updates the name and email of an existing user.",
        OperationId = "users.update",
        Tags = new[] { "Users" })]
    public override async Task<ActionResult<ApiResponse<UpdateUserResponse>>> HandleAsync(
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!HttpContext.Request.RouteValues.TryGetValue("id", out var routeId)
            || !Guid.TryParse(routeId?.ToString(), out var id))
        {
            return BadRequest(ApiResponse<UpdateUserResponse>.Fail("Users.InvalidId", "Invalid user ID."));
        }

        var command = new UpdateUserCommand(id, request.Name, request.Email);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code switch
            {
                "Users.UserNotFound" => NotFound(ApiResponse<UpdateUserResponse>.Fail(result.Error.Code, result.Error.Message)),
                "Users.EmailAlreadyExists" => Conflict(ApiResponse<UpdateUserResponse>.Fail(result.Error.Code, result.Error.Message)),
                _ => result.ToActionResult()
            };
        }

        return Ok(ApiResponse<UpdateUserResponse>.Ok(result.Value!, "Updated"));
    }
}
