using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Project.Core.DataLayer;
using Project.Core.Services;
using Rebus.OpenTelemetry.Configuration;

namespace Project.Worker;

public class Program
{
    private static readonly CancellationTokenSource _tokenSource = new();
    public static async Task Main(string[] args)
    {
        AppDomain.CurrentDomain.ProcessExit += (sender, e) => {
            _tokenSource?.Cancel();
            Console.WriteLine("Process is exiting...");
        };
        
        var builder = WebApplication.CreateBuilder(args);
        try {
            await CreateWebApplication(args)
                .ConfigureWebApp()
                .RunAsync(_tokenSource.Token);        
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally {
            _tokenSource?.Dispose();
        }

        
    }
    
    private static WebApplication CreateWebApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.RegisterDataLayer(builder.Configuration);
        builder.Services.RegisterCoreServices(builder.Configuration, "worker");
        //builder.Services.RegisterBusinessLogic(builder.Configuration);
        
        RegisterOpenTelemetry(builder);
        builder.Services.AddCors(cfg =>
        {
            cfg.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyHeader()
                    .AllowAnyOrigin()
                    .AllowAnyMethod();
            });
        });
        
        return builder.Build();
    }
    
    private static void RegisterOpenTelemetry(WebApplicationBuilder builder) {
        builder.Services.AddOpenTelemetry()
            .WithTracing(cfg => {
                cfg.AddAspNetCoreInstrumentation();
                cfg.AddNpgsql();
                cfg.AddRebusInstrumentation();
            })
            .WithMetrics(cfg => {
                cfg.AddAspNetCoreInstrumentation();
                cfg.AddNpgsqlInstrumentation();
                cfg.AddRebusInstrumentation();
            });
        
        // REF: https://www.youtube.com/watch?v=oHE1MztOP3I&t=492s
        builder.Logging.AddOpenTelemetry(cfg => {
            cfg.IncludeFormattedMessage = true;
            cfg.IncludeScopes = true;
        });
        
        builder.Services.Configure<OpenTelemetryLoggerOptions>(cfg => {
            cfg.AddOtlpExporter("client.worker", options => { });
        });
        
        builder.Services.ConfigureOpenTelemetryMeterProvider(cfg => cfg.AddOtlpExporter("client.worker", options => { }));
        builder.Services.ConfigureOpenTelemetryTracerProvider(cfg => cfg.AddOtlpExporter("client.worker", options => { }));
    }
}

internal static class WebApplicationExtensions
{
    internal static WebApplication ConfigureWebApp(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.UseCors();

        return app;
    }
}
