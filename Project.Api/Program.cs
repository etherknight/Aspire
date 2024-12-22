using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Project.BusinessLogic;

namespace Project.Api;

public static class Program
{
    private static CancellationTokenSource _tokenSource = new();

    public static async Task Main(string[] args) {
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

        builder.Services.AddOpenTelemetry()
            .WithTracing(cfg => {
                cfg.AddAspNetCoreInstrumentation();
                cfg.AddNpgsql();
            })
            .WithMetrics(cfg => {
                cfg.AddAspNetCoreInstrumentation();
                cfg.AddNpgsqlInstrumentation();
            });

        builder.Logging.AddOpenTelemetry(cfg => {
            cfg.IncludeFormattedMessage = true;
            cfg.IncludeScopes = true;
        });

        builder.Services.Configure<OpenTelemetryLoggerOptions>(cfg => {
            cfg.AddOtlpExporter();
        });
        
        builder.Services.ConfigureOpenTelemetryMeterProvider(cfg => cfg.AddOtlpExporter());
        builder.Services.ConfigureOpenTelemetryTracerProvider(cfg => cfg.AddOtlpExporter());

        builder.Services.RegisterBusinessLogic(builder.Configuration);

        return builder.Build();
    }
}

internal static class WebApplicationExtensions
{
    internal static WebApplication ConfigureWebApp(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}