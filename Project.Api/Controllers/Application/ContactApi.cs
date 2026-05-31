using MediatR;
using Microsoft.AspNetCore.Mvc;
using Project.BusinessLogic.Contacts;
using Project.BusinessLogic.Contacts.Models;

namespace Project.Api.Controllers.Application;

public class ContactApi : BaseApi, IApiRouteBuilder
{
    private const string ApiName = "contact";

    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiName,
            async ([FromServices] ISender sender, [FromQuery] int start, [FromQuery] int limit, CancellationToken token) =>
            {
                GetContactListQuery query = new(start, limit);
                return await sender.Send(query, token)
                    .Finally(Results.Ok, HandleError);
            })
            .WithSummary("Get paginated list of contacts with their custom fields.")
            .WithName("GetContactList")
            .WithTags(ApiName);

        app.MapPost(ApiName,
            async ([FromServices] ISender sender, [FromBody] ContactDTO contact, CancellationToken token) =>
            {
                CreateContactCommand command = new(contact);
                return await sender.Send(command, token)
                    .Finally(Results.Ok, HandleError);
            })
            .WithSummary("Create a new contact with optional custom field values.")
            .WithName("CreateContact")
            .WithTags(ApiName);

        app.MapGet($"{ApiName}/{{id:guid}}",
            async ([FromServices] ISender sender, Guid id, CancellationToken token) =>
            {
                GetContactQuery query = new(id);
                return await sender.Send(query, token)
                    .Finally(Results.Ok, HandleError);
            })
            .WithSummary("Get a single contact by ID with custom field values.")
            .WithName("GetContact")
            .WithTags(ApiName);

        app.MapPut($"{ApiName}/{{id:guid}}",
            async ([FromServices] ISender sender, Guid id, [FromBody] ContactDTO contact, CancellationToken token) =>
            {
                UpdateContactCommand command = new(id, contact);
                return await sender.Send(command, token)
                    .Finally(Results.Ok, HandleError);
            })
            .WithSummary("Update a contact's core fields and custom field values.")
            .WithName("UpdateContact")
            .WithTags(ApiName);

        app.MapDelete($"{ApiName}/{{id:guid}}",
            async ([FromServices] ISender sender, Guid id, CancellationToken token) =>
            {
                DeleteContactCommand command = new(id);
                return await sender.Send(command, token)
                    .Finally(Results.Ok, HandleError);
            })
            .WithSummary("Delete a contact and all its custom field values.")
            .WithName("DeleteContact")
            .WithTags(ApiName);
    }
}
