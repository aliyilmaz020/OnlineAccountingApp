namespace OnlineAccountingApp.WebApi.Models;

public sealed class ApiResponse
{
    public bool Success { get; init; }
    public object? Data { get; init; }
    public string? ErrorCode { get; init; }
    public string? Message { get; init; }
    public IReadOnlyDictionary<string, string[]>? Errors { get; init; }

    public static ApiResponse Ok(object? data) => new() { Success = true, Data = data };

    public static ApiResponse Fail(string? errorCode, string message, IReadOnlyDictionary<string, string[]>? errors = null) =>
        new() { Success = false, ErrorCode = errorCode, Message = message, Errors = errors };
}
