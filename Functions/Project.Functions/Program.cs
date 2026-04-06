using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Rebus.Config;
using Rebus.OpenTelemetry.Configuration;

namespace Project.Functions;

public class Program {
    public static void Main(string[] args) {
        FunctionsApplicationBuilder builder = FunctionsApplication.CreateBuilder(args);

        builder.ConfigureFunctionsWebApplication();
        builder.Services.RegisterMessaging(builder.Configuration);
        
        RegisterOpenTelemetry(builder);
        // builder.Services
        //     .AddApplicationInsightsTelemetryWorkerService()
        //     .ConfigureFunctionsApplicationInsights();

        builder.Build().Run();

    }
    
    private static FunctionsApplicationBuilder RegisterOpenTelemetry(FunctionsApplicationBuilder builder) {
        string activitySourceName = "client.function";
        
        builder.Services.AddOpenTelemetry()
            .WithTracing(cfg => {
                cfg.AddRebusInstrumentation();
            })
            .WithMetrics(cfg => {
            });
        
        // REF: https://www.youtube.com/watch?v=oHE1MztOP3I&t=492s
        builder.Logging.AddOpenTelemetry(cfg => {
            cfg.IncludeFormattedMessage = true;
            cfg.IncludeScopes = true;
        });
        
        builder.Services.Configure<OpenTelemetryLoggerOptions>(cfg => {
            cfg.AddOtlpExporter(activitySourceName, options => { });
        });
        
        builder.Services.ConfigureOpenTelemetryMeterProvider(cfg => cfg.AddOtlpExporter(activitySourceName, options => { }));
        builder.Services.ConfigureOpenTelemetryTracerProvider(cfg => {
            cfg.AddOtlpExporter(activitySourceName, options => { });
            cfg.AddSource(activitySourceName);
        });
        return builder;
    }
    

}

internal static class ProgramExt {
    internal static IServiceCollection RegisterMessaging(this IServiceCollection services, ConfigurationManager configuration)
    {
        const string QueueName = "client_ipc";

        string connectionString = configuration.GetConnectionString("MessageBus") ?? "";

        services.AddRebus(cfg =>
            cfg
                .Options(opts =>
                {
                    opts.EnableDiagnosticSources();
                })
                .Transport(transport => { 
                    transport.UseRabbitMq(connectionString, $"{QueueName}.func");
                })
        );
        

        return services;
    }
}
