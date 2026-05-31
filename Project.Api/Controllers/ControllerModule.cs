using System.Reflection;
using Rebus.Retry.ErrorTracking;

namespace Project.Api.Controllers;

public interface IApiRouteBuilder {
    void Map(IEndpointRouteBuilder app);
}

public static class ControllerModule {
    extension(IEndpointRouteBuilder app) {
        public void MapProjectApi() {
            IEnumerable<Type> controllers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => typeof(IApiRouteBuilder).IsAssignableFrom(type) 
                               && false == type.IsInterface);
            
            foreach (Type type in controllers) {
                var controller = Activator.CreateInstance(type);
                if (controller is IApiRouteBuilder route) {
                    route.Map(app);
                }
            }
        }
    }
}

public abstract class BaseApi  {
    public IResult HandleError(OptionError error) {
        return Results.Problem(error.Message, statusCode: 400);
    }
}
