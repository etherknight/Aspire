using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Project.Functions;

public class TodoNotifierFn
{
    private readonly ILogger<TodoNotifierFn> _logger;

    public TodoNotifierFn(ILogger<TodoNotifierFn> logger)
    {
        _logger = logger;
    }

    [Function("TodoNotifierF")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}
