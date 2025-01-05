using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Core.Services.Interfaces.Messaging;
using Project.Core.Services.Interfaces.Messaging.Messages;
using Project.Core.Services.Messaging.Messages;
using Rebus.RabbitMq;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.Transport;
using Rebus.Transport.InMem;
using RabbitMQ.Client;

namespace Project.Core.Services.Messaging;

internal static class Module
{
    internal static IServiceCollection RegisterMessaging(this IServiceCollection services, IConfiguration configuration, string role) {
        const string QueueName = "client_ipc";
        
        string connectionString = configuration.GetConnectionString("MessageBus") ?? "";
        
        services.AddRebus(cfg => 
            cfg
                .Options(opts => {
                    opts.EnableDiagnosticSources()
                        .LogPipeline(true);
                })
                .Transport(transport => {
                    transport.UseRabbitMq(connectionString, $"{QueueName}.{role}");
                })
                .Routing(routing => {
                    routing.TypeBased()
                           .Map<TodoCreatedM>($"{QueueName}.worker");
                })
            );
        
        services
            .AutoRegisterHandlersFromAssemblyOf<TodoCreatedHandler>();
        return services;
    }
}
