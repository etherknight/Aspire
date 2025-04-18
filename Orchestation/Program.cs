using Aspire.Hosting.Azure;
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
        IResourceBuilder<ParameterResource> username = builder.AddParameter("username");
        IResourceBuilder<ParameterResource> password = builder.AddParameter("password");
        
        IResourceBuilder<PostgresDatabaseResource> database = SetupDatabase(builder, username, password);
        IResourceBuilder<RabbitMQServerResource> messageBus = SetupMessageBus(builder, username, password);
        
        IResourceBuilder<AzureFunctionsProjectResource> functions = builder.AddAzureFunctionsProject<Project_Worker_Functions>("ProjectFunctions")
                                                                           .WithExternalHttpEndpoints();

        var api = builder.AddProject<Project_Api>("ProjectApi")
               .WaitFor(database)
               .WaitFor(messageBus)
               .WithReference(database)
               .WithReference(messageBus);

        builder.AddProject<Project_Worker>("ProjectWorker")
            .WaitFor(database)
            .WaitFor(messageBus)
            .WithReference(functions)
            .WithReference(database)
            .WithReference(messageBus)
            .WithReplicas(2);

        builder.AddYarnApp("Dashboard", "../client/project-site", "dev")
               .WithReference(api)
               .WithEnvironment("BROWSER", "none")
               .WithEnvironment("VITE_API_URL", api.GetEndpoint("http"))
               .WithHttpEndpoint(port: 2080, targetPort: 5173, env: "VITE_PORT")             
               .WithYarnPackageInstallation();

        return builder;
    }

    private static IResourceBuilder<PostgresDatabaseResource> SetupDatabase(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<ParameterResource> username, 
        IResourceBuilder<ParameterResource> password 
    ) {
        IResourceBuilder<PostgresDatabaseResource> database = builder.AddPostgres("ClientDB", username, password, port: 2050)
            // Persist DB data.
            .WithDataVolume("application_db")
            // Stops Aspire killing the DB container each time it stops debugging.
            .WithLifetime(ContainerLifetime.Persistent)
            .AddDatabase("ApplicationDB", "application");
        return database;
    }

    private static IResourceBuilder<RabbitMQServerResource> SetupMessageBus(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<ParameterResource> username, 
        IResourceBuilder<ParameterResource> password
    ) {
        IResourceBuilder<RabbitMQServerResource> messageBus = builder
            .AddRabbitMQ("MessageBus", username, password, port: 2060)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataVolume("message_bus")
            .WithManagementPlugin();
        
        return messageBus;
    }
}