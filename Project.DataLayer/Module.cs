using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Project.Core.DataLayer;

public static class Module
{
    public static IServiceCollection RegisterDataLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        string? connect = configuration.GetConnectionString("ApplicationDB");

        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            opt.UseNpgsql(connect, config =>
            {
                config.UseNodaTime();
            })
            .UseLowerCaseNamingConvention();
        });

        return services;
    }
}
