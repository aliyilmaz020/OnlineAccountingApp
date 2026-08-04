using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.Create;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.Delete;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.GetMainRoleById;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.GetMainRoles;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.Update;

namespace OnlineAccountingApp.WebApi.Controllers;

public class MainRolesController(IMediator mediator) : BaseApiController(mediator)
{
    [HttpPost("[action]")]
    public async Task<IActionResult> CreateMainRole([FromBody] CreateMainRoleCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetMainRoles([FromQuery] GetMainRolesQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("[action]/{id}")]
    public async Task<IActionResult> GetMainRoleById(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMainRoleByIdQuery { Id = id }, cancellationToken);
        return Ok(result);
    }

    [HttpPut("[action]/{id}")]
    public async Task<IActionResult> UpdateMainRole(string id, [FromBody] UpdateMainRoleCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("[action]/{id}")]
    public async Task<IActionResult> DeleteMainRole(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteMainRoleCommand { Id = id }, cancellationToken);
        return Ok(result);
    }
}
