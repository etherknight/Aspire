using MediatR;
using Microsoft.EntityFrameworkCore;
using Project.BusinessLogic.Todos.Models;
using Project.Core.DataLayer;
using Project.Core.DataLayer.Entities;

namespace Project.BusinessLogic.Todos;

public sealed record CreateTodoCommand(TodoDTO Todo) : IRequest<Option<TodoDTO>> { }

internal class CreateTodoCommandHandler(
        IApplicationDbContext dbContext
    ) : IRequestHandler<CreateTodoCommand, Option<TodoDTO>> {
    
    private readonly IApplicationDbContext _dbContext = dbContext;

    private CancellationToken _cancellation = CancellationToken.None;

    public async Task<Option<TodoDTO>> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        _cancellation = cancellationToken;
        
        return await ValidateRequest(request)
                        .Then(validDto => CreateTodo(validDto))
                        .Then(todo => SaveTodo(todo))
                        .Then(todo => UpdateTodoId(todo, request.Todo));
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
}
