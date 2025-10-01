using System.Diagnostics;
using System.Reflection;

namespace Project.Core.Services.Interfaces.Diagnostics;

public interface IProjectTracer {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="activityName"></param>
    /// <returns></returns>
    Activity? StartActivity(string activityName);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="activityName"></param>
    /// <typeparam name="TModule"></typeparam>
    /// <returns></returns>
    Activity? StartActivity<TModule>(string activityName);
}