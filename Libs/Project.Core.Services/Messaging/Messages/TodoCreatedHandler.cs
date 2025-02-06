using Microsoft.Extensions.Logging;
using NodaTime;
using Project.Core.DataLayer;
using Project.Core.DataLayer.Entities;
using Project.Core.Services.Interfaces.Messaging.Messages;
using Rebus.Handlers;

namespace Project.Core.Services.Messaging.Messages;

internal sealed class TodoCreatedHandler(ILogger<TodoCreatedHandler> logger, IApplicationDbContext dbContext) 
    : IHandleMessages<TodoCreatedM> {
    private readonly ILogger<TodoCreatedHandler> _logger = logger;
    private readonly IApplicationDbContext _dbContext = dbContext;

    public Task Handle(TodoCreatedM message) {
        _logger.LogInformation("Created new todo with Id {TodoId}", message.TodoId);
        
        Todo? todo = _dbContext.Todos.FirstOrDefault(todo => todo.Id == message.TodoId);
        if (todo is not null) {
            todo.DueBy = ZonedDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow.AddDays(7));
            _dbContext.SaveChangesAsync(CancellationToken.None);
        }
        
        return Task.CompletedTask;
    }
}