using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Project.Core.Services;
using Rebus.OpenTelemetry.Configuration;

namespace Project.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.RegisterCoreServices(builder.Configuration, "worker");
       
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(cfg => {
                cfg.AddService("client.worker");
            })
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
           cfg.AddOtlpExporter();
       });
        
       builder.Services.ConfigureOpenTelemetryMeterProvider(cfg => cfg.AddOtlpExporter());
       builder.Services.ConfigureOpenTelemetryTracerProvider(cfg => cfg.AddOtlpExporter());

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
