using System.Net;
using MyMvcApp.Exceptions;

namespace MyMvcApp.Middleware;

public sealed class ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await next(context);
            return;
        }

        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // The client disconnected; there is no response to write.
        }
        catch (Exception exception)
        {
            if (context.Response.HasStarted) throw;

            var (status, message, errors) = exception switch
            {
                ServiceException serviceException => (serviceException.StatusCode, serviceException.Message, serviceException.Errors),
                UnauthorizedAccessException => ((int)HttpStatusCode.Forbidden, "You are not authorized to perform this action.", null),
                _ => ((int)HttpStatusCode.InternalServerError, "An unexpected server error occurred.", null)
            };

            if (status >= 500) logger.LogError(exception, "Unhandled API exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            else if (exception is not ServiceException) logger.LogWarning(exception, "Request rejected for {Method} {Path}", context.Request.Method, context.Request.Path);

            context.Response.Clear();
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { message, errors });
        }
    }
}
