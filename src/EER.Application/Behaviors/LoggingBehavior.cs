using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EER.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("[START] Handling {RequestName} with data: {@Request}", requestName, request);
        try
        {
            var response = await next(cancellationToken);
            stopwatch.Stop();
            _logger.LogInformation("[END] Handled {RequestName} successfully in {ElapsedMilliseconds:0.0000}ms", requestName, stopwatch.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            throw;
        }
    }
}
