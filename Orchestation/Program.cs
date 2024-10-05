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
                              .WithEnvironment("POSTGRES_DB", "ApplicationDB")
                              .AddDatabase("ApplicationDB", "application");

        //var filestore = builder.AddAzureStorage("Filestore")
        //                       .RunAsEmulator()
        //                       .AddBlobs("dev");

        builder.AddProject<Projects.Project_Api>("ProjectApi")
      //         .WithReference(filestore)
               .WithReference(database);

        //builder.AddProject<Projects.Project_Worker>("ProjectWorker")
        //    .WithReference(database)
        //    .WithReplicas(2);

        return builder;
    }
}