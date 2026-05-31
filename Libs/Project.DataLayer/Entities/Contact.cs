using Project.Core.DataLayer.Entities.Interfaces;

namespace Project.Core.DataLayer.Entities;

[Table("contact")]
public class Contact : Entity
{
    [MaxLength(100)]
    public required string FirstName { get; set; }

    [MaxLength(100)]
    public required string LastName { get; set; }

    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    public ICollection<CustomFieldValue> CustomFieldValues { get; set; } = [];
}
