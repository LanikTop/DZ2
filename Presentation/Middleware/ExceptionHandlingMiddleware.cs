using System.Net;
using System.Text.Json;
using Grpc.Core;

namespace Presentation.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception exception)
        {

            _logger.LogError(exception, $"Необработанная ошибка при обработке {context.Request.Method} {context.Request.Path}");

            if (context.Response.HasStarted)
                throw;

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                title = "Внутренняя ошибка сервера",
                status = context.Response.StatusCode,
                traceId = context.TraceIdentifier
            }));
        }
    }
}