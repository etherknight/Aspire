using MediatR;
using Microsoft.AspNetCore.Mvc;

using Project.BusinessLogic.Core;

namespace Project.Api.Controllers;

/// <summary>
/// Invoke system commands.
/// </summary>
[Route("[controller]")]
[ApiController]
public class SystemController : ControllerBase
{
    private readonly ISender _sender;

    public SystemController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("startup")]
    public async Task<IActionResult> Startup(CancellationToken token)
    {
        await _sender.Send(new DatabaseInitCommand(), token);
        return Ok();
    }
}
