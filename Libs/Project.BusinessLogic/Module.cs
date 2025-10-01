using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Core.DataLayer;
using Project.BusinessLogic.Behaviours;

namespace Project.BusinessLogic;

public static class Module
{
    public static IServiceCollection RegisterBusinessLogic(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterDataLayer(configuration);

        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(Module).Assembly);
        });
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DiagnosticBehaviour<,>));
        return services;
    }
}
