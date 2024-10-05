using MediatR;
using Microsoft.EntityFrameworkCore;
using Project.BusinessLogic.Todos.Models;
using Project.Core.DataLayer;

namespace Project.BusinessLogic.Todos;

internal sealed record GetTodoQuery : IRequest<IEnumerable<TodoDTO>> { }

internal class GetTodoQueryHandler : IRequestHandler<GetTodoQuery, IEnumerable<TodoDTO>>
{
    private readonly IApplicationDbContext _applicationDb;

    public GetTodoQueryHandler(IApplicationDbContext applicationDb) 
    {
        _applicationDb = applicationDb;
    }

    public async Task<IEnumerable<TodoDTO>> Handle(GetTodoQuery request, CancellationToken cancellationToken)
    {
        return await _applicationDb.Todos.Select(TodoDTO.Projection)
                                         .ToListAsync(cancellationToken);
    }
}
