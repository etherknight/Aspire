using Project.Core.DataLayer.Entities;
using Project.Shared.Interfaces.Data;
using System.Linq.Expressions;

namespace Project.BusinessLogic.Contacts.Models;

public class CustomFieldDefinitionDTO : IProjection<CustomFieldDefinition, CustomFieldDefinitionDTO>
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public CustomFieldTypeE FieldType { get; set; }
    public required string EntityType { get; set; }
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }

    public static Expression<Func<CustomFieldDefinition, CustomFieldDefinitionDTO>> Projection =>
        cfd => new CustomFieldDefinitionDTO
        {
            Id = cfd.Id,
            Name = cfd.Name,
            FieldType = cfd.FieldType,
            EntityType = cfd.EntityType,
            IsRequired = cfd.IsRequired,
            SortOrder = cfd.SortOrder
        };
}
