using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Project.Core.Services.Interfaces.Messaging.Functions;

namespace Project.Functions;

public class TodoEmailerFn {
    private readonly ILogger<TodoEmailerFn> _logger;

    public TodoEmailerFn(ILogger<TodoEmailerFn> logger) {
        _logger = logger;
    }

    [Function("TodoEmailerFn")]
    public void Run([RabbitMQTrigger("client_ipc.func", ConnectionStringSetting = "MessageBus")] TodoEmailerM message) {
        _logger.LogInformation("Sending an email from an Azure Function '{Subject}'", message.Message);
    }
}