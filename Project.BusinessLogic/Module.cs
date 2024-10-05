using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Core.DataLayer;

namespace Project.BusinessLogic;

public static class Module
{
    public static IServiceCollection RegisterBusinessLogic(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterDataLayer(configuration);

        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(Module).Assembly);
        });
        return services;
    }
}
