using Project.Core.DataLayer.Entities;
using Project.Shared.Interfaces.Data;
using System.Linq.Expressions;

namespace Project.BusinessLogic.Contacts.Models;

public class ContactDTO : IProjection<Contact, ContactDTO>
{
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public List<CustomFieldValueDTO> CustomFields { get; set; } = [];

    public static Expression<Func<Contact, ContactDTO>> Projection =>
        contact => new ContactDTO
        {
            Id = contact.Id,
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            Email = contact.Email,
            Phone = contact.Phone,
            CustomFields = contact.CustomFieldValues.Select(cfv => new CustomFieldValueDTO
            {
                Id = cfv.Id,
                CustomFieldDefinitionId = cfv.CustomFieldDefinitionId,
                Value = cfv.Value
            }).ToList()
        };
}
