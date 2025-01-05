using Microsoft.Extensions.Logging;
using Project.Core.Services.Interfaces.Messaging.Messages;
using Rebus.Handlers;

namespace Project.Core.Services.Messaging.Messages;

internal sealed class TodoCreatedHandler(ILogger<TodoCreatedHandler> logger) : IHandleMessages<TodoCreatedM> {
    private readonly ILogger<TodoCreatedHandler> _logger = logger;
    
    public Task Handle(TodoCreatedM message) {
        _logger.LogInformation("Created new todo with Id {TodoId}", message.TodoId);
        return Task.CompletedTask;
    }
}