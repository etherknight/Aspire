using System.Diagnostics;
using System.Reflection;
using Project.Core.Services.Interfaces.Diagnostics;

namespace Project.Core.Services.Diagnostics;

public class ProjectTracer : IProjectTracer {
    private const string DEFAULT_SERVICE = "Project";
    
    private static readonly ActivitySource _source = new(Assembly.GetEntryAssembly().GetName().Name ?? DEFAULT_SERVICE);

    public Activity StartActivity(string activityName) {
        Activity activity = _source.StartActivity(activityName);
        return activity;
    }

    public Activity StartActivity<TModule>(string activityName) {
        string moduleName = typeof(TModule).Name;
        Activity activity = _source.StartActivity($"{moduleName}::{activityName}", ActivityKind.Internal);
        return activity;
    }
}