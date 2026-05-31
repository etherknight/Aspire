using Project.Core.DataLayer.Entities;
using Project.Shared.Interfaces.Data;
using System.Linq.Expressions;

namespace Project.BusinessLogic.Contacts.Models;

public class CustomFieldValueDTO : IProjection<CustomFieldValue, CustomFieldValueDTO>
{
    public Guid Id { get; set; }
    public Guid CustomFieldDefinitionId { get; set; }
    public string? Value { get; set; }

    public static Expression<Func<CustomFieldValue, CustomFieldValueDTO>> Projection =>
        cfv => new CustomFieldValueDTO
        {
            Id = cfv.Id,
            CustomFieldDefinitionId = cfv.CustomFieldDefinitionId,
            Value = cfv.Value
        };
}
