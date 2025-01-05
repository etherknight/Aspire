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
        
        builder.AddProject<Project_Api>("ProjectApi")
               .WaitFor(database)
               .WaitFor(messageBus)
               .WithReference(database)
               .WithReference(messageBus);

        builder.AddProject<Project_Worker>("ProjectWorker")
            .WaitFor(database)
            .WaitFor(messageBus)
            .WithReference(database)
            .WithReference(messageBus)
            .WithReplicas(2);

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