using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Project.Shared.Services;

namespace Project.Shared.Proto;

public static class Module {

    public static IServiceCollection RegisterGrpc(this IServiceCollection services) {
        services.AddGrpc();
        services.AddGrpcReflection();
        return services;
    }
    
    public static WebApplication MapGrpcServices(this WebApplication app) {
        app.MapGrpcReflectionService(); // TODO: Make this development only.
        app.MapGrpcService<TodoService>();
        app.UseGrpcWeb();

        return app;
    }
}