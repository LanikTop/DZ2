using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Presentation.Interceptors;

public sealed class LoggingInterceptor : Interceptor
{
    private readonly ILogger<LoggingInterceptor> _logger;

    public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            _logger.LogInformation($"Grps Request {context.Method}: {request}");
            var response = await continuation(request, context);
            _logger.LogInformation($"Grps Response {context.Method}: {response}");
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogWarning($"gRPC Error {context.Method}: {ex.StatusCode} - {ex.Status.Detail}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Grpc Request {context.Method} failed with exception: {ex.Message}");
            throw new RpcException(new Status(StatusCode.Internal, $"{ex.Message}"), $"{ex.Message}");
        }
    }
}