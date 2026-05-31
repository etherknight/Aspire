using Project.Core.DataLayer.Entities.Interfaces;

namespace Project.Core.DataLayer.Entities;

[Table("custom_field_definition")]
public class CustomFieldDefinition : Entity
{
    [MaxLength(100)]
    public required string Name { get; set; }

    public CustomFieldTypeE FieldType { get; set; }

    [MaxLength(100)]
    public required string EntityType { get; set; }

    public bool IsRequired { get; set; }

    public int SortOrder { get; set; }
}
