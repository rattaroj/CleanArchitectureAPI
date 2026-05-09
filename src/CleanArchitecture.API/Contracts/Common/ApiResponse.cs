using CleanArchitecture.Application.Common.Models;

namespace CleanArchitecture.API.Contracts.Common;

public sealed record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data,
    ApiError? Error = null,
    PaginationMetadata? Pagination = null)
{
    public static ApiResponse<T> Ok(T data, string message = "Success")
    {
        return new ApiResponse<T>(true, message, data);
    }

    public static ApiResponse<IReadOnlyList<T>> Paged(
        PagedResult<T> page,
        string message = "Success")
    {
        ArgumentNullException.ThrowIfNull(page);

        return new ApiResponse<IReadOnlyList<T>>(
            true,
            message,
            page.Items,
            Pagination: PaginationMetadata.From(page));
    }

    public static ApiResponse<T> Fail(
        string code,
        string message,
        IReadOnlyDictionary<string, string[]>? details = null)
    {
        return new ApiResponse<T>(
            false,
            message,
            default,
            new ApiError(code, message, details));
    }
}
