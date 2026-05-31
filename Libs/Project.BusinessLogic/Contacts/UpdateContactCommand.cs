using Project.BusinessLogic.Contacts.Models;
using Project.Core.DataLayer;
using Project.Core.DataLayer.Entities;
using Project.Core.Services.Interfaces.Diagnostics;

namespace Project.BusinessLogic.Contacts;

public sealed record UpdateContactCommand(Guid Id, ContactDTO Contact) : IRequest<Option<ContactDTO>> { }

internal class UpdateContactCommandHandler(
    IApplicationDbContext dbContext,
    IProjectTracer tracer
) : IRequestHandler<UpdateContactCommand, Option<ContactDTO>>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IProjectTracer _tracer = tracer;

    private CancellationToken _cancellation = CancellationToken.None;

    public async Task<Option<ContactDTO>> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        using var activity = _tracer.StartActivity<UpdateContactCommandHandler>(nameof(Handle));
        _cancellation = cancellationToken;

        Contact? found = await _dbContext.Contacts
            .Include(c => c.CustomFieldValues)
            .FirstOrDefaultAsync(c => c.Id == request.Id, _cancellation);

        Option<Contact> loaded = found is not null
            ? found
            : OptionError.GuardError("notFound", $"Contact {request.Id} not found");

        Option<Contact> coreApplied = loaded.Then(c => ApplyCoreFields(c, request.Contact));
        Option<Contact> customApplied = coreApplied.Then(c => ApplyCustomFields(c, request.Contact.CustomFields));
        Option<Contact> saved = await customApplied.Then(c => SaveContact(c));
        return saved.Then(c => MapToDto(c)).EndTrace(activity);
    }

    private Option<Contact> ApplyCoreFields(Contact contact, ContactDTO dto)
    {
        contact.FirstName = dto.FirstName;
        contact.LastName = dto.LastName;
        contact.Email = dto.Email;
        contact.Phone = dto.Phone;
        contact.ModifiedAt = DateTimeOffset.UtcNow;
        return contact;
    }

    private Option<Contact> ApplyCustomFields(Contact contact, List<CustomFieldValueDTO> incoming)
    {
        foreach (var dto in incoming)
        {
            var existing = contact.CustomFieldValues
                .FirstOrDefault(v => v.CustomFieldDefinitionId == dto.CustomFieldDefinitionId);

            if (existing is not null)
                existing.Value = dto.Value;
            else
                contact.CustomFieldValues.Add(new CustomFieldValue
                {
                    CustomFieldDefinitionId = dto.CustomFieldDefinitionId,
                    Value = dto.Value
                });
        }
        return contact;
    }

    private async Task<Option<Contact>> SaveContact(Contact contact)
    {
        Option<Contact> result = OptionError.NotComplete;
        try
        {
            await _dbContext.SaveChangesAsync(_cancellation);
            result = contact;
        }
        catch (DbUpdateException ex)
        {
            result = OptionError.FromException(ex);
        }
        return result;
    }

    private Option<ContactDTO> MapToDto(Contact contact) => new ContactDTO
    {
        Id = contact.Id,
        FirstName = contact.FirstName,
        LastName = contact.LastName,
        Email = contact.Email,
        Phone = contact.Phone,
        CustomFields = contact.CustomFieldValues.Select(v => new CustomFieldValueDTO
        {
            Id = v.Id,
            CustomFieldDefinitionId = v.CustomFieldDefinitionId,
            Value = v.Value
        }).ToList()
    };
}
