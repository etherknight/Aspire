using System.Reflection.Metadata;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Project.BusinessLogic.Todos;
using Project.BusinessLogic.Todos.Models;

namespace Project.Api.Controllers.Application
{
    [Route("application/[controller]")]
    [ApiController]
    public class TodoController(ISender sender) : Controller
    {
        private readonly ISender _sender = sender;

        private IActionResult HandleFailure(OptionError error)
        {
            return Problem();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetTodoList([FromQuery]int start, [FromQuery]int limit, CancellationToken token)
        {
            GetTodoListQuery query = new(start, limit);
            return await _sender.Send(query, token)
                                .Finally(Ok, HandleFailure);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateTodo([FromBody] TodoDTO todo, CancellationToken token)
        {
            CreateTodoCommand command = new(todo);
            return await _sender.Send(command, token)
                                .Finally(Ok, HandleFailure);
        }
    }
}
