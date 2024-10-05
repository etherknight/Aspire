using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Project.Core.Services;

internal static class Module
{
    public static IServiceCollection RegisterCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        
        return services;
    }
}
