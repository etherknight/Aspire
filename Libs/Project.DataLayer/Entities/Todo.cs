using NodaTime;
using Project.Core.DataLayer.Entities.Interfaces;

namespace Project.Core.DataLayer.Entities;

[Table("todo")]
public class Todo : Entity
{
    [MaxLength(100)]
    public required string Title { get; set; }

    public ZonedDateTime? DueBy { get; set; } = null;

    public bool IsComplete { get; set; } = false;
}