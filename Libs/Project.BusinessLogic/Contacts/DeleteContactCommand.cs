using Project.Core.DataLayer;
using Project.Core.DataLayer.Entities;
using Project.Core.Services.Interfaces.Diagnostics;

namespace Project.BusinessLogic.Contacts;

public sealed record DeleteContactCommand(Guid Id) : IRequest<Option<bool>> { }

internal class DeleteContactCommandHandler(
    IApplicationDbContext dbContext,
    IProjectTracer tracer
) : IRequestHandler<DeleteContactCommand, Option<bool>>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IProjectTracer _tracer = tracer;

    private CancellationToken _cancellation = CancellationToken.None;

    public async Task<Option<bool>> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
    {
        using var activity = _tracer.StartActivity<DeleteContactCommandHandler>(nameof(Handle));
        _cancellation = cancellationToken;

        Contact? found = await _dbContext.Contacts
            .Include(c => c.CustomFieldValues)
            .FirstOrDefaultAsync(c => c.Id == request.Id, _cancellation);

        Option<Contact> loaded = found is not null
            ? found
            : OptionError.GuardError("notFound", $"Contact {request.Id} not found");

        return await loaded.Then(c => DeleteContact(c)).EndTrace(activity);
    }

    private async Task<Option<bool>> DeleteContact(Contact contact)
    {
        Option<bool> result = OptionError.NotComplete;
        try
        {
            // Remove the custom field values themselves (join table rows cascade from Contact delete)
            _dbContext.CustomFieldValues.RemoveRange(contact.CustomFieldValues);
            _dbContext.Contacts.Remove(contact);
            await _dbContext.SaveChangesAsync(_cancellation);
            result = true;
        }
        catch (DbUpdateException ex)
        {
            result = OptionError.FromException(ex);
        }
        return result;
    }
}
