using MediatR;
using Microsoft.EntityFrameworkCore;
using Project.BusinessLogic.Todos.Models;
using Project.Core.DataLayer;

namespace Project.BusinessLogic.Todos;

public sealed record GetTodoListQuery(int Start, int Limit = 100) : IRequest<Option<IEnumerable<TodoDTO>>> { }

internal class GetTodoListQueryHandler(IApplicationDbContext applicationDb)
    : IRequestHandler<GetTodoListQuery, Option<IEnumerable<TodoDTO>>>
{
    public async Task<Option<IEnumerable<TodoDTO>>> Handle(GetTodoListQuery request, CancellationToken cancellationToken)
    {
        List<TodoDTO> todos = await applicationDb.Todos
                                     .OrderBy(todo => todo.Id)
                                     .Skip(request.Start)
                                     .Take(Math.Max(10, request.Limit))
                                     .Select(TodoDTO.Projection)
                                     .ToListAsync(cancellationToken);
        return todos;
    }
}
