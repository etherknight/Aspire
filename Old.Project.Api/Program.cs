using Project.BusinessLogic;

namespace Project.Api;

public class Program
{
    private static CancellationTokenSource tokenSource = new();

    /// <summary>
    /// Application Entry Point
    /// </summary>
    /// <param name="args"></param>
    public async static Task Main(string[] args)
    {
        await CrateWebApplication(args)
                  .ConfigureWebApp()
                  .RunAsync(tokenSource.Token);
    }

    private static WebApplication CrateWebApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

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
