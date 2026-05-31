using Project.BusinessLogic.Contacts.Models;
using Project.Core.DataLayer;

namespace Project.BusinessLogic.Contacts;

public sealed record GetCustomFieldDefinitionsQuery(string EntityType)
    : IRequest<Option<IEnumerable<CustomFieldDefinitionDTO>>> { }

internal class GetCustomFieldDefinitionsQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetCustomFieldDefinitionsQuery, Option<IEnumerable<CustomFieldDefinitionDTO>>>
{
    public async Task<Option<IEnumerable<CustomFieldDefinitionDTO>>> Handle(
        GetCustomFieldDefinitionsQuery request, CancellationToken cancellationToken)
    {
        List<CustomFieldDefinitionDTO> definitions = await dbContext.CustomFieldDefinitions
            .Where(cfd => cfd.EntityType == request.EntityType)
            .OrderBy(cfd => cfd.SortOrder)
            .Select(CustomFieldDefinitionDTO.Projection)
            .ToListAsync(cancellationToken);

        return definitions;
    }
}
