using Projects;

namespace Orchestration;

internal class Program
{
    private static void Main(string[] args)
    {        
        Orchestrate(args)
            .Build()
            .Run();
    }

    private static IDistributedApplicationBuilder Orchestrate(string[] args)
    {
        IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

        // Params come from Orchestation\appsettings.Development.json   
        var username = builder.AddParameter("username");
        var password = builder.AddParameter("password");
        
        var database = builder.AddPostgres("ClientDB", username, password, port: 2050)
                            // Persist DB data.
                            .WithDataVolume("application_db")
                            // Stops Aspire killing the DB container each time it stops debugging.
                            .WithLifetime(ContainerLifetime.Persistent)
                            .AddDatabase("ApplicationDB", "application");
        
        builder.AddProject<Project_Api>("ProjectApi")
               .WaitFor(database)
               .WithReference(database);

        builder.AddProject<Project_Worker>("ProjectWorker")
            .WaitFor(database)
            .WithReference(database)
            .WithReplicas(2);

        return builder;
    }
}