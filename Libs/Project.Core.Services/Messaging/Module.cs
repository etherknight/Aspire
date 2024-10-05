using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Project.Core.Services.Messaging;

internal static class Module
{
    internal static IServiceCollection RegisterMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(config =>
        {
            // elided...
            

            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h => {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
