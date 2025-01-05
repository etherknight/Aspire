using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Core.Services.Interfaces.Messaging;
using Project.Core.Services.Messaging;

namespace Project.Core.Services;

public static class Module
{
    public static IServiceCollection RegisterCoreServices(this IServiceCollection services, IConfiguration configuration, string role) {
        services.AddTransient<IMessagingService, MessagingService>();
        
        services.RegisterMessaging(configuration, role);
        
        return services;
    }
}
