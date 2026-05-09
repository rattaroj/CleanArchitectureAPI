using CleanArchitecture.API.Contracts.Common;
using CleanArchitecture.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Extensions;

public static class ResultExtensions
{
    public static ActionResult<ApiResponse<T>> ToActionResult<T>(this Result<T> result)
    {
        return ToActionResult<T, T>(result);
    }

    public static ActionResult<ApiResponse<TResponse>> ToActionResult<TValue, TResponse>(
        this Result<TValue> result)
    {
        var statusCode = result.Error.Code switch
        {
            var code when code.Contains("NotFound", StringComparison.OrdinalIgnoreCase)
                => StatusCodes.Status404NotFound,

            var code when code.Contains("Conflict", StringComparison.OrdinalIgnoreCase)
                => StatusCodes.Status409Conflict,

            var code when code.Contains("Validation", StringComparison.OrdinalIgnoreCase)
                => StatusCodes.Status400BadRequest,

            _ => StatusCodes.Status500InternalServerError
        };

        return new ObjectResult(ApiResponse<TResponse>.Fail(
            result.Error.Code,
            result.Error.Message))
        {
            StatusCode = statusCode
        };
    }
}
