using System.Runtime.InteropServices.ComTypes;
using Project.Core.Services.Interfaces.Diagnostics;

namespace Project.BusinessLogic.Behaviours;

public class DiagnosticBehaviour<TRequest, TResponse>(
    ILogger<DiagnosticBehaviour<TRequest, TResponse>> logger, 
    IProjectTracer tracer) 
    : IPipelineBehavior<TRequest, TResponse> {
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        string commandName = request.GetType().Name;
        using Activity? activity = tracer.StartActivity(commandName);
        using (logger.BeginScope(request)) {
            logger.LogInformation("Command Started: {command.Name}", commandName);
            activity?.AddBaggage("command.Name", commandName);
            
            TResponse response = await next(cancellationToken);
            logger.LogInformation("Command Completed: {command.Name} in {command.Duration}", commandName, activity?.Duration ?? TimeSpan.Zero);
            return response;
        }
    }
}