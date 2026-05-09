using Ardalis.ApiEndpoints;
using CleanArchitecture.API.Contracts.Common;
using CleanArchitecture.Application.Features.Users.Commands.DeleteUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanArchitecture.API.Endpoints.Users;

public sealed class DeleteUserEndpoint(ISender sender)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<ApiResponse<DeleteUserResponse>>
{
    [HttpDelete("api/users/{Id:guid}", Name = "Users.Delete")]
    [SwaggerOperation(
        Summary = "Deletes a user.",
        Description = "Deletes a user with the provided Id.",
        OperationId = "users.delete",
        Tags = new[] { "Users" })]
    public override async Task<ActionResult<ApiResponse<DeleteUserResponse>>> HandleAsync(
        [FromRoute(Name = "Id")] Guid Id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteUserCommand(Id);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code switch
            {
                "Users.UserNotFound" => NotFound(ApiResponse<DeleteUserResponse>.Fail("Users.UserNotFound",result.Error.Message)),
                _ => BadRequest(ApiResponse<DeleteUserResponse>.Fail("Users.DeleteFailed", result.Error.Message))
            };
        }

        return Ok(ApiResponse<DeleteUserResponse>.Ok(result.Value!, "Deleted"));
    }
}