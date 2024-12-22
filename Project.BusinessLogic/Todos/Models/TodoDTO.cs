using NodaTime;
using Project.Core.DataLayer.Entities;
using Project.Shared.Interfaces.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Project.BusinessLogic.Todos.Models;

public class TodoDTO : IProjection<Todo, TodoDTO>
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public ZonedDateTime? DueBy { get; set; } = null;

    public bool IsComplete { get; set; } = false;

    public static Expression<Func<Todo, TodoDTO>> Projection
    {
        get
        {
            return todo => new TodoDTO()
            {
                Id = todo.Id,
                Title = todo.Title,
                DueBy = todo.DueBy,
                IsComplete = todo.IsComplete,
            };
        }
    }
}
