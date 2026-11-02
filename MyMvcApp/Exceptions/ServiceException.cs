namespace MyMvcApp.Exceptions;

public sealed class ServiceException(int statusCode, string message, IReadOnlyList<string>? errors = null)
    : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public IReadOnlyList<string>? Errors { get; } = errors;
}
