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
using Rebus.Routing;
using System.Reflection;

namespace Project.Core.Services.Messaging;

internal static class Module
{
    private static IDictionary<MessagingDestinationE, string> _addresses = new Dictionary<MessagingDestinationE, string> {
            { MessagingDestinationE.Worker, "client_ipc.worker" },
            { MessagingDestinationE.Admin, "client_ipc.admin" },
            { MessagingDestinationE.Api, "client_ipc.api" }
     };

    internal static IServiceCollection RegisterMessaging(this IServiceCollection services, IConfiguration configuration, string role)
    {
        const string QueueName = "client_ipc";

        string connectionString = configuration.GetConnectionString("MessageBus") ?? "";

        services.AddRebus(cfg =>
            cfg
                .Options(opts =>
                {
                    opts.EnableDiagnosticSources()
                        .LogPipeline(true);
                })
                .Transport(transport => transport.UseRabbitMq(connectionString, $"{QueueName}.{role}"))
                .Routing(router => SetupMessageRouting(router))
            );

        services
            .AutoRegisterHandlersFromAssemblyOf<TodoCreatedHandler>();

        return services;
    }

    private static void SetupMessageRouting(StandardConfigurer<IRouter> router) {
        var typeBasedRouter = router.TypeBased();

        IEnumerable<Type> messages = Assembly.Load("Project.Core.Services.Interfaces")
                                             .GetTypes()
                                             .Where(type => type.IsAssignableTo(typeof(IIpcMessage)) && type.IsClass );

        foreach (Type messageType in messages) {
            string destination = GetDestination(messageType);            
            typeBasedRouter.Map(messageType, destination);            
        }

    }

    private static string GetDestination(Type messageType) { 
        var attribute = messageType.GetCustomAttribute<IpcMessageAttribute>();

        if (attribute is null) {
            throw new Exception($"IpcMessage '{messageType.FullName}' has not implemented IpcMessage attribute");
        }

        return _addresses[attribute.Destination];
    }
}
