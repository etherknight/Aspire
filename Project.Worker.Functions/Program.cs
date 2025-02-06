using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

public static class Program
{
    private static readonly CancellationTokenSource _tokenSource = new();

    public static async Task Main(string[] args)
    {
        try
        {
            var host = new HostBuilder()
                        .RegisterOpenTelemetry()
                        .ConfigureFunctionsWorkerDefaults()
                        .Build();

            host.Run();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            _tokenSource?.Dispose();
        }
    }

    private static HostBuilder RegisterOpenTelemetry(this HostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddOpenTelemetry()
                    .WithTracing()
                    .WithLogging();

            services.Configure<OpenTelemetryLoggerOptions>(cfg => cfg.AddOtlpExporter("client.worker.functions", options => { }));
            services.ConfigureOpenTelemetryMeterProvider(cfg => cfg.AddOtlpExporter("client.worker.functions", options => { }));
            services.ConfigureOpenTelemetryTracerProvider(cfg => cfg.AddOtlpExporter("client.worker.functions", options => { }));
        });

        //builder.ConfigureLogging(logging =>
        //{
        //    logging.AddOpenTelemetry(cfg =>
        //    {
        //        cfg.IncludeFormattedMessage = true;
        //        cfg.IncludeScopes = true;
        //    });
        //});


        //// REF: https://www.youtube.com/watch?v=oHE1MztOP3I&t=492s
        //builder.Logging.AddOpenTelemetry(cfg => {
        //    cfg.IncludeFormattedMessage = true;
        //    cfg.IncludeScopes = true;
        //});



       

        return builder;
    }

}




