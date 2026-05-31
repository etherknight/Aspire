using MediatR;
using Microsoft.AspNetCore.Mvc;
using Project.BusinessLogic.Contacts;
using Project.BusinessLogic.Contacts.Models;

namespace Project.Api.Controllers.Application;

public class CustomFieldApi : BaseApi, IApiRouteBuilder
{
    private const string ApiName = "customfield";

    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiName,
            async ([FromServices] ISender sender, [FromQuery] string entityType, CancellationToken token) =>
            {
                GetCustomFieldDefinitionsQuery query = new(entityType);
                return await sender.Send(query, token)
                    .Finally(Results.Ok, HandleError);
            })
            .WithSummary("Get all custom field definitions for a given entity type.")
            .WithName("GetCustomFieldDefinitions")
            .WithTags(ApiName);

        app.MapPost(ApiName,
            async ([FromServices] ISender sender, [FromBody] CustomFieldDefinitionDTO definition, CancellationToken token) =>
            {
                CreateCustomFieldDefinitionCommand command = new(definition);
                return await sender.Send(command, token)
                    .Finally(Results.Ok, HandleError);
            })
            .WithSummary("Create a new custom field definition for an entity type.")
            .WithName("CreateCustomFieldDefinition")
            .WithTags(ApiName);
    }
}
