using Ardalis.ApiEndpoints;
using CleanArchitecture.API.Contracts.Common;
using CleanArchitecture.API.Extensions;
using CleanArchitecture.Application.Features.Roles.Queries.GetRoles;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanArchitecture.API.Endpoints.Roles;

public sealed class GetRolesEndpoint(ISender sender)
    : EndpointBaseAsync
        .WithoutRequest
        .WithActionResult<ApiResponse<IReadOnlyList<RoleDto>>>
{
    [HttpGet("api/roles")]
    [SwaggerOperation(
        Summary = "Gets all available roles.",
        Description = "Returns the list of all roles that can be assigned to users.",
        OperationId = "roles.getAll",
        Tags = new[] { "Roles" })]
    public override async Task<ActionResult<ApiResponse<IReadOnlyList<RoleDto>>>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetRolesQuery(), cancellationToken);

        if (result.IsFailure)
            return result.ToActionResult();

        return Ok(ApiResponse<IReadOnlyList<RoleDto>>.Ok(result.Value!));
    }
}
