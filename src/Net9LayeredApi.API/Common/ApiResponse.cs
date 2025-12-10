namespace Net9LayeredApi.API.Common;

public record ApiResponse<T>(bool Success, string? Message = null, T? Data = default, IEnumerable<string>? Errors = null)
{
    public static ApiResponse<T> Ok(T data, string? message = null) => new(true, message, data, null);

    public static ApiResponse<T> Fail(string message, IEnumerable<string>? errors = null) => new(false, message, default, errors);
}

public record ApiResponse(bool Success, string? Message = null, IEnumerable<string>? Errors = null)
{
    public static ApiResponse Ok(string? message = null) => new(true, message, null);

    public static ApiResponse Fail(string message, IEnumerable<string>? errors = null) => new(false, message, errors);
}

