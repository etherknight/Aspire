﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Project.BusinessLogic.Todos.Models;
using Project.Core.DataLayer;

namespace Project.BusinessLogic.Todos;

internal sealed record GetTodoListQuery : IRequest<IEnumerable<TodoDTO>> { }

internal class GetTodoListQueryHandler : IRequestHandler<GetTodoListQuery, IEnumerable<TodoDTO>>
{
    private readonly IApplicationDbContext _applicationDb;

    public GetTodoListQueryHandler(IApplicationDbContext applicationDb) 
    {
        _applicationDb = applicationDb;
    }

    public async Task<IEnumerable<TodoDTO>> Handle(GetTodoListQuery request, CancellationToken cancellationToken)
    {
        return await _applicationDb.Todos.Select(TodoDTO.Projection)
                                         .ToListAsync(cancellationToken);
    }
}