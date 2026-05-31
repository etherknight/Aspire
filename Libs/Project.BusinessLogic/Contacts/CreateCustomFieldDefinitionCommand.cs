using Project.BusinessLogic.Contacts.Models;
using Project.Core.DataLayer;
using Project.Core.DataLayer.Entities;
using Project.Core.Services.Interfaces.Diagnostics;

namespace Project.BusinessLogic.Contacts;

public sealed record CreateCustomFieldDefinitionCommand(CustomFieldDefinitionDTO Definition)
    : IRequest<Option<CustomFieldDefinitionDTO>> { }

internal class CreateCustomFieldDefinitionCommandHandler(
    IApplicationDbContext dbContext,
    IProjectTracer tracer
) : IRequestHandler<CreateCustomFieldDefinitionCommand, Option<CustomFieldDefinitionDTO>>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IProjectTracer _tracer = tracer;

    private CancellationToken _cancellation = CancellationToken.None;

    public async Task<Option<CustomFieldDefinitionDTO>> Handle(
        CreateCustomFieldDefinitionCommand request, CancellationToken cancellationToken)
    {
        using var activity = _tracer.StartActivity<CreateCustomFieldDefinitionCommandHandler>(nameof(Handle));
        _cancellation = cancellationToken;

        return await ValidateRequest(request)
            .Then(dto => CreateDefinition(dto))
            .Then(definition => SaveDefinition(definition, request.Definition))
            .EndTrace(activity);
    }

    private Option<CustomFieldDefinitionDTO> ValidateRequest(CreateCustomFieldDefinitionCommand request)
    {
        using var activity = _tracer.StartActivity<CreateCustomFieldDefinitionCommandHandler>(nameof(ValidateRequest));
        Option<CustomFieldDefinitionDTO> valid = request.Definition;

        valid.Guard(() => request.Definition is not null, "missingDto", "Must provide a CustomFieldDefinitionDTO")
             .Guard(() => false == string.IsNullOrWhiteSpace(request.Definition?.Name), "requiredField", "Must provide a Name")
             .Guard(() => false == string.IsNullOrWhiteSpace(request.Definition?.EntityType), "requiredField", "Must provide an EntityType")
             .EndTrace(activity);

        return valid;
    }

    private Option<CustomFieldDefinition> CreateDefinition(CustomFieldDefinitionDTO dto)
    {
        using var activity = _tracer.StartActivity<CreateCustomFieldDefinitionCommandHandler>(nameof(CreateDefinition));
        activity.EndTraceOk();
        return new CustomFieldDefinition
        {
            Name = dto.Name,
            FieldType = dto.FieldType,
            EntityType = dto.EntityType,
            IsRequired = dto.IsRequired,
            SortOrder = dto.SortOrder
        };
    }

    private async Task<Option<CustomFieldDefinitionDTO>> SaveDefinition(
        CustomFieldDefinition definition, CustomFieldDefinitionDTO original)
    {
        using var activity = _tracer.StartActivity<CreateCustomFieldDefinitionCommandHandler>(nameof(SaveDefinition));
        Option<CustomFieldDefinitionDTO> result = OptionError.NotComplete;
        try
        {
            _dbContext.CustomFieldDefinitions.Add(definition);
            await _dbContext.SaveChangesAsync(_cancellation);
            original.Id = definition.Id;
            result = original;
        }
        catch (DbUpdateException ex)
        {
            result = OptionError.FromException(ex);
        }
        result.EndTrace(activity);
        return result;
    }
}
