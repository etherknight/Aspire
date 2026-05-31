using Project.BusinessLogic.Contacts.Models;
using Project.Core.DataLayer;

namespace Project.BusinessLogic.Contacts;

public sealed record GetContactQuery(Guid Id) : IRequest<Option<ContactDTO>> { }

internal class GetContactQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetContactQuery, Option<ContactDTO>>
{
    public async Task<Option<ContactDTO>> Handle(GetContactQuery request, CancellationToken cancellationToken)
    {
        ContactDTO? contact = await dbContext.Contacts
            .Include(c => c.CustomFieldValues)
            .Where(c => c.Id == request.Id)
            .Select(ContactDTO.Projection)
            .FirstOrDefaultAsync(cancellationToken);

        if (contact is null)
            return OptionError.GuardError("notFound", $"Contact {request.Id} not found");

        return contact;
    }
}
