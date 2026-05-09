
using Ardalis.ApiEndpoints;
using CleanArchitecture.API.Contracts.Common;
using CleanArchitecture.API.Contracts.Users;
using CleanArchitecture.API.Extensions;
using CleanArchitecture.Application.Features.Users.Commands.CreateUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanArchitecture.API.Endpoints.Users;

public sealed class CreateUserEndpoint(ISender sender)
    : EndpointBaseAsync
        .WithRequest<CreateUserRequest>
        .WithActionResult<ApiResponse<CreateUserResponse>>
{
    [HttpPost("api/users")]
    [SwaggerOperation(
        Summary = "Creates a new user.",
        Description = "Creates a new user with the provided details.",
        OperationId = "users.create",
        Tags = new[] { "Users" })]
    
    public override async Task<ActionResult<ApiResponse<CreateUserResponse>>> HandleAsync(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateUserCommand(
            request.Name,
            request.Email
        );

        var result = await sender.Send(command, cancellationToken);

        if(result.IsFailure)
        {
            return result.ToActionResult();
        }

        return CreatedAtRoute(
            "Users.GetById",
            new { id = result.Value!.Id },
            ApiResponse<CreateUserResponse>.Ok(result.Value, "Created")
        );
    }
}
