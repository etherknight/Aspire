using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using Project.BusinessLogic.Core;

namespace Project.Api.Controllers;

/// <summary>
/// Invoke system commands.
/// </summary>
public class SystemController : IApiRouteBuilder{
    const string apiName = "system";
    
    public void Map(IEndpointRouteBuilder app) {
        app.MapGet("${apiName}/startup", async ([FromServices] ISender sender, CancellationToken token) => {
            await sender.Send(new DatabaseInitCommand(), token);
            return Results.Ok();
        });
    }
}
