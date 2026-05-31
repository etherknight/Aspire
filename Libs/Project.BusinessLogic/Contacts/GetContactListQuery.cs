using Project.BusinessLogic.Contacts.Models;
using Project.Core.DataLayer;

namespace Project.BusinessLogic.Contacts;

public sealed record GetContactListQuery(int Start, int Limit = 100) : IRequest<Option<IEnumerable<ContactDTO>>> { }

internal class GetContactListQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetContactListQuery, Option<IEnumerable<ContactDTO>>>
{
    public async Task<Option<IEnumerable<ContactDTO>>> Handle(GetContactListQuery request, CancellationToken cancellationToken)
    {
        List<ContactDTO> contacts = await dbContext.Contacts
            .Include(c => c.CustomFieldValues)
            .OrderBy(c => c.Id)
            .Skip(request.Start)
            .Take(Math.Max(10, request.Limit))
            .Select(ContactDTO.Projection)
            .ToListAsync(cancellationToken);

        return contacts;
    }
}
