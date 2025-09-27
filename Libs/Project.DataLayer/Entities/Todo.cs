using NodaTime;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Core.DataLayer.Entities;

[Table("todo")]
public class Todo
{
    public int Id { get; set; }

    [MaxLength(100)]
    public required string Title { get; set; }

    public ZonedDateTime? DueBy { get; set; } = null;

    public bool IsComplete { get; set; } = false;
}