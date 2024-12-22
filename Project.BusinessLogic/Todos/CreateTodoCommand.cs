using MediatR;
using Project.BusinessLogic.Todos.Models;

namespace Project.BusinessLogic.Todos;

public sealed record CreateTodoCommand(TodoDTO todo) : IRequest<Option<TodoDTO>> { }

internal class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, Option<TodoDTO>>
{

    public async Task<Option<TodoDTO>> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        return OptionError.NotImplemented;
    }
}
