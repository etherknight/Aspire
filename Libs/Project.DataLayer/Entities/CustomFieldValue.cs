using Project.Core.DataLayer.Entities.Interfaces;

namespace Project.Core.DataLayer.Entities;

[Table("custom_field_value")]
public class CustomFieldValue : Entity
{
    public Guid CustomFieldDefinitionId { get; set; }

    public CustomFieldDefinition CustomFieldDefinition { get; set; } = null!;

    [MaxLength(1000)]
    public string? Value { get; set; }
}
