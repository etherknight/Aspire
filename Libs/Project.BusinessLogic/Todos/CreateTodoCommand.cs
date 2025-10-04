using Project.BusinessLogic.Todos.Models;
using Project.Core.DataLayer;
using Project.Core.DataLayer.Entities;
using Project.Core.Services.Interfaces.Diagnostics;
using Project.Core.Services.Interfaces.Messaging;
using Project.Core.Services.Interfaces.Messaging.Messages;

namespace Project.BusinessLogic.Todos;

public sealed record CreateTodoCommand(TodoDTO Todo) : IRequest<Option<TodoDTO>> { }

internal class CreateTodoCommandHandler(
        IApplicationDbContext dbContext,
        IMessagingService messagingService,
        ILogger<CreateTodoCommandHandler> logger,
        IProjectTracer tracer
    ) : IRequestHandler<CreateTodoCommand, Option<TodoDTO>> {
    
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IMessagingService _messagingService = messagingService;
    private readonly ILogger<CreateTodoCommandHandler> _logger = logger;
    private readonly IProjectTracer _tracer = tracer;

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
        using var activity = _tracer.StartActivity<CreateTodoCommandHandler>(nameof(ValidateRequest));
        Option<TodoDTO> valid = request.Todo;

        valid.Guard(() => request.Todo is not null)
             .Guard(() => false == string.IsNullOrWhiteSpace(request.Todo?.Title))
             .EndTrace(activity);
        
        return valid;
    }

    private Option<Todo> CreateTodo(TodoDTO validDto) {
        using var activity = _tracer.StartActivity<CreateTodoCommandHandler>(nameof(CreateTodo));
        Todo todo = CreateTodoInternal(validDto);
        return todo;
    }

    /// <summary>
    /// Pointless internal method just to test out tracing.
    /// </summary>
    /// <param name="validDto"></param>
    /// <returns></returns>
    private Todo CreateTodoInternal(TodoDTO validDto) {
        using var activity = _tracer.StartActivity<CreateTodoCommandHandler>(nameof(CreateTodoInternal));
        return new Todo() {
            Title = validDto.Title,
            DueBy = validDto.DueBy,
            IsComplete = validDto.IsComplete,
        };
    }

    private async Task<Option<Todo>> SaveTodo(Todo todo) {
        using var activity = _tracer.StartActivity<CreateTodoCommandHandler>(nameof(SaveTodo));
        Option<Todo> result = OptionError.NotComplete;
        try {
            _dbContext.Todos.Add(todo);
            await _dbContext.SaveChangesAsync(_cancellation);
            result = todo;
        }
        catch (DbUpdateException ex) {
            result = OptionError.FromException(ex);
        }
        
        result.EndTrace(activity);
        return result;
    }

    private Option<TodoDTO> UpdateTodoId(Todo addedTodo, TodoDTO originalDto) {
        using var activity = _tracer.StartActivity<CreateTodoCommandHandler>(nameof(UpdateTodoId));
        originalDto.Id = addedTodo.Id;
        activity.EndTraceOk();
        return originalDto;
    }

    private Option<TodoDTO> FireEvents(TodoDTO todoDto) {
        using var activity = _tracer.StartActivity<CreateTodoCommandHandler>(nameof(FireEvents));
        _logger.LogDebug("Firing created todo event");
        _messagingService.Send( new TodoCreatedM(todoDto.Id));
        activity.EndTraceOk();
        return todoDto;
    }
}
