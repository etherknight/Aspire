using Project.Core.Services.Interfaces.Diagnostics;

namespace Project.BusinessLogic.Behaviours;

public class DiagnosticBehaviour<TRequest, TResponse>(
    ILogger<DiagnosticBehaviour<TRequest, TResponse>> logger, 
    IProjectTracer tracer) 
    : IPipelineBehavior<TRequest, TResponse> {
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        using Activity? activity = tracer.StartActivity(typeof(TRequest).Name);
        using (logger.BeginScope(request)) {
            activity?.AddBaggage("project.commandName", typeof(TRequest).Name);

            TResponse response = await next(cancellationToken);
            activity?.Stop();
            return response;
        }
    }
}