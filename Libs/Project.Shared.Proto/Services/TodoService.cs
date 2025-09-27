using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using NodaTime;
using Project.BusinessLogic.Todos;
using Project.Shared.GRPC;
using Project.BusinessLogic.Todos.Models;
using Project.Shared.Interfaces;

namespace Project.Shared.Services;

public class TodoService(ISender sender) : Project.Shared.GRPC.TodoService.TodoServiceBase
{
    private readonly ISender _sender = sender;

    public override async Task<CreateTodoResponse> CreateTodo(CreateTodoRequest request, ServerCallContext context) {
        TodoDTO dto = new() {
            Title = request.Title,
            //DueBy = ZonedDateTime.FromDateTimeOffset(request.DueBy.ToDateTimeOffset()),
            IsComplete = false
        };
        
        CreateTodoCommand command = new(dto);
        Option<TodoDTO> result = await _sender.Send(command, context.CancellationToken);
        
        return result.Finally<CreateTodoResponse>(
                todoDto => new CreateTodoResponse() {
                    Id = todoDto.Id, 
                    Title = todoDto.Title, 
                    IsComplete = todoDto.IsComplete
                },
                error => throw new RpcException(new Status(StatusCode.InvalidArgument, error.Message)));
    }
}