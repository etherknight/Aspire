using Project.BusinessLogic.Todos.Models;
using Project.Core.DataLayer;
using Project.Core.DataLayer.Entities;
using Project.Core.Services.Interfaces.Messaging;
using Project.Core.Services.Interfaces.Messaging.Messages;

namespace Project.BusinessLogic.Todos;

public sealed record CreateTodoCommand(TodoDTO Todo) : IRequest<Option<TodoDTO>> { }

internal class CreateTodoCommandHandler(
        IApplicationDbContext dbContext,
        IMessagingService messagingService,
        ILogger<CreateTodoCommandHandler> logger
    ) : IRequestHandler<CreateTodoCommand, Option<TodoDTO>> {
    
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IMessagingService _messagingService = messagingService;
    private readonly ILogger<CreateTodoCommandHandler> _logger = logger;

    private CancellationToken _cancellation = CancellationToken.None;

    public async Task<Option<TodoDTO>> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        _cancellation = cancellationToken;
        
        return await ValidateRequest(request)
                        .Then(validDto => CreateTodo(validDto))
                        .Then(todo => SaveTodo(todo))
                        .Then(todo => UpdateTodoId(todo, request.Todo))
                        .Then(todo => FireEvents(todo));
    }

    private Option<TodoDTO> ValidateRequest(CreateTodoCommand request) {
        
        Option<TodoDTO> valid = request.Todo;

        valid.Guard(() => request.Todo is not null)
             .Guard(() => false == string.IsNullOrWhiteSpace(request.Todo?.Title));
        
        return valid;
    }

    private Option<Todo> CreateTodo(TodoDTO validDto) {
        Todo todo = new() {
            Title = validDto.Title,
            DueBy = validDto.DueBy,
            IsComplete = validDto.IsComplete,
        };
        return todo;
    }

    private async Task<Option<Todo>> SaveTodo(Todo todo) {
        Option<Todo> result = OptionError.NotComplete;
        try {
            _dbContext.Todos.Add(todo);
            await _dbContext.SaveChangesAsync(_cancellation);
            result = todo;
        }
        catch (DbUpdateException ex) {
            result = OptionError.FromException(ex);
        }

        return result;
    }

    private Option<TodoDTO> UpdateTodoId(Todo addedTodo, TodoDTO originalDto) {
        originalDto.Id = addedTodo.Id;
        return originalDto;
    }

    private Option<TodoDTO> FireEvents(TodoDTO todoDto) {
        _logger.LogDebug("Firing created todo event");
        _messagingService.Send( new TodoCreatedM(todoDto.Id));
        return todoDto;
    }
}
