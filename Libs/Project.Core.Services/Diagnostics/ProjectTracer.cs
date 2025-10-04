using System.Reflection;

namespace Project.Core.Services.Diagnostics;

public class ProjectTracer : IProjectTracer {
    private const string DefaultService = "Project";
    public static string GetActivitySourceName()
        => Assembly.GetEntryAssembly()?.GetName().Name ?? DefaultService;
    
    private static readonly ActivitySource Source = new( GetActivitySourceName() );
    
    public Activity? StartActivity(string activityName) {
        Activity? activity = Source.StartActivity(activityName);
        return activity;
    }

    public Activity? StartActivity<TModule>(string activityName) {
        string moduleName = typeof(TModule).Name;
        return Source.StartActivity($"{moduleName}::{activityName}", ActivityKind.Internal);
    }
}