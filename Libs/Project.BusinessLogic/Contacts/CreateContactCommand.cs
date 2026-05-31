using Project.BusinessLogic.Contacts.Models;
using Project.Core.DataLayer;
using Project.Core.DataLayer.Entities;
using Project.Core.Services.Interfaces.Diagnostics;
using Project.Core.Services.Interfaces.Messaging;
using Project.Core.Services.Interfaces.Messaging.Messages;

namespace Project.BusinessLogic.Contacts;

public sealed record CreateContactCommand(ContactDTO Contact) : IRequest<Option<ContactDTO>> { }

internal class CreateContactCommandHandler(
    IApplicationDbContext dbContext,
    IMessagingService messagingService,
    ILogger<CreateContactCommandHandler> logger,
    IProjectTracer tracer
) : IRequestHandler<CreateContactCommand, Option<ContactDTO>>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IMessagingService _messagingService = messagingService;
    private readonly ILogger<CreateContactCommandHandler> _logger = logger;
    private readonly IProjectTracer _tracer = tracer;

    private CancellationToken _cancellation = CancellationToken.None;

    public async Task<Option<ContactDTO>> Handle(CreateContactCommand request, CancellationToken cancellationToken)
    {
        using var activity = _tracer.StartActivity<CreateContactCommandHandler>(nameof(Handle));
        _cancellation = cancellationToken;

        return await ValidateRequest(request)
            .Then(dto => CreateContact(dto))
            .Then(contact => SaveContact(contact))
            .Then(contact => UpdateContactId(contact, request.Contact))
            .Then(dto => FireEvents(dto))
            .EndTrace(activity);
    }

    private Option<ContactDTO> ValidateRequest(CreateContactCommand request)
    {
        using var activity = _tracer.StartActivity<CreateContactCommandHandler>(nameof(ValidateRequest));
        Option<ContactDTO> valid = request.Contact;

        valid.Guard(() => request.Contact is not null, "missingDto", "Must provide a ContactDTO")
             .Guard(() => false == string.IsNullOrWhiteSpace(request.Contact?.FirstName), "requiredField", "Must provide a FirstName")
             .Guard(() => false == string.IsNullOrWhiteSpace(request.Contact?.LastName), "requiredField", "Must provide a LastName")
             .EndTrace(activity);

        return valid;
    }

    private Option<Contact> CreateContact(ContactDTO dto)
    {
        using var activity = _tracer.StartActivity<CreateContactCommandHandler>(nameof(CreateContact));
        var contact = new Contact
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone
        };
        foreach (var cfv in dto.CustomFields)
        {
            contact.CustomFieldValues.Add(new CustomFieldValue
            {
                CustomFieldDefinitionId = cfv.CustomFieldDefinitionId,
                Value = cfv.Value
            });
        }
        activity.EndTraceOk();
        return contact;
    }

    private async Task<Option<Contact>> SaveContact(Contact contact)
    {
        using var activity = _tracer.StartActivity<CreateContactCommandHandler>(nameof(SaveContact));
        Option<Contact> result = OptionError.NotComplete;
        try
        {
            _dbContext.Contacts.Add(contact);
            await _dbContext.SaveChangesAsync(_cancellation);
            result = contact;
        }
        catch (DbUpdateException ex)
        {
            result = OptionError.FromException(ex);
        }

        result.EndTrace(activity);
        return result;
    }

    private Option<ContactDTO> UpdateContactId(Contact saved, ContactDTO original)
    {
        using var activity = _tracer.StartActivity<CreateContactCommandHandler>(nameof(UpdateContactId));
        original.Id = saved.Id;
        activity.EndTraceOk();
        return original;
    }

    private Option<ContactDTO> FireEvents(ContactDTO dto)
    {
        using var activity = _tracer.StartActivity<CreateContactCommandHandler>(nameof(FireEvents));
        _logger.LogDebug("Firing ContactCreated event for {ContactId}", dto.Id);
        _messagingService.Send(new ContactCreatedM(dto.Id));
        activity.EndTraceOk();
        return dto;
    }
}
