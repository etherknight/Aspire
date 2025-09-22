using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Core.Services.Interfaces.Messaging;
using Project.Core.Services.Messaging.Messages;
using Rebus.Config;
using Rebus.Routing;
using Rebus.Routing.TypeBased;

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
                .Transport(transport => { 
                    transport.UseRabbitMq(connectionString, $"{QueueName}.{role}");
                })
                .Routing(SetupMessageRouting)
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
             GetDestination(messageType, typeBasedRouter);  
        }
    }

    private static void GetDestination(Type messageType, TypeBasedRouterConfigurationExtensions.TypeBasedRouterConfigurationBuilder router) { 
        var attributes = messageType.GetCustomAttributes<IpcMessageAttribute>();
        foreach (var attribute in attributes) {
            string destination = _addresses[attribute.Destination];
            router.Map(messageType, destination);
        }
    }
}

