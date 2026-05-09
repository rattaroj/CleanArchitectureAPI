using Ardalis.ApiEndpoints;
using CleanArchitecture.API.Contracts.Common;
using CleanArchitecture.API.Extensions;
using CleanArchitecture.Application.Features.Users.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanArchitecture.API.Endpoints.Users;

public sealed class GetUserByIdEndpoint(ISender sender)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<ApiResponse<UserDto>>
{
    [HttpGet("api/users/{id:guid}", Name = "Users.GetById")]
    [SwaggerOperation(
        Summary = "Gets a user by id.",
        Description = "Gets a user by id.",
        OperationId = "users.getById",
        Tags = new[] { "Users" })]
    public override async Task<ActionResult<ApiResponse<UserDto>>> HandleAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetUserByIdQuery(id), cancellationToken);

        if (result.IsFailure)
        {
            return result.ToActionResult();
        }

        return Ok(ApiResponse<UserDto>.Ok(result.Value!));
    }
}
