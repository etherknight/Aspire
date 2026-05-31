using MediatR;
using Microsoft.AspNetCore.Mvc;
using Project.BusinessLogic.Todos;
using Project.BusinessLogic.Todos.Models;

namespace Project.Api.Controllers.Application;

public class TodoApi : BaseApi, IApiRouteBuilder{
    const string apiName = "todo";
    
    public void Map(IEndpointRouteBuilder app) {
        app.MapGet(apiName, 
            async ([FromServices] ISender sender, CancellationToken token, [FromQuery]int start = 0, [FromQuery]int limit = 25) => {
            GetTodoListQuery query = new(start, limit);
            return await sender.Send(query, token)
                    .Finally(Results.Ok, HandleError);
        })
        .WithSummary("Get list of todos.")
        .WithDescription("This is a description.")
        .WithName("GetTodoList")
        .WithTags(apiName);
        
        app.MapPost(apiName, async ([FromServices] ISender sender, [FromBody] TodoDTO todo, CancellationToken token) => {
            CreateTodoCommand command = new(todo);
            await sender.Send(command, token)
                    .Finally(Results.Ok, HandleError);
        })
        .WithSummary("This is a summary.")
        .WithDescription("This is a description.")
        .WithName("CreateTodo")
        .WithTags(apiName);
    }
}