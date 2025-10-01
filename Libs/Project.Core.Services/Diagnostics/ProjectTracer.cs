using System.Diagnostics;
using System.Reflection;
using Project.Core.Services.Interfaces.Diagnostics;
using Project.Shared.Interfaces;

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

public static class ProjectTracerExtensions {
    public static void SetStatus<TData>(this Activity? activity, Option<TData> result)
     where TData: class {
        ActivityStatusCode status = result.Finally(some => ActivityStatusCode.Ok, error => ActivityStatusCode.Error);
        activity?.SetStatus(status);
    }
}