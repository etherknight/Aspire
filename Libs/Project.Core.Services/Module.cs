using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Core.Services.Messaging;

namespace Project.Core.Services;

internal static class Module
{
    public static IServiceCollection RegisterCoreServices(this IServiceCollection services, IConfiguration configuration) {
        services.RegisterMessaging(configuration);
        return services;
    }
}
