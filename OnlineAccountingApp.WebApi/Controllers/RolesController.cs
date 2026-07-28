using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.AssignRoleToUser;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.Create;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.Delete;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoleById;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.RemoveRoleFromUser;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.Update;

namespace OnlineAccountingApp.WebApi.Controllers;

public class RolesController(IMediator mediator) : BaseApiController(mediator)
{
    [HttpPost("[action]")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetRoles([FromQuery] GetRolesQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("[action]/{id}")]
    public async Task<IActionResult> GetRoleById(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRoleByIdQuery { Id = id }, cancellationToken);
        return Ok(result);
    }

    [HttpPut("[action]/{id}")]
    public async Task<IActionResult> UpdateRole(string id, [FromBody] UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("[action]/{id}")]
    public async Task<IActionResult> DeleteRole(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteRoleCommand { Id = id }, cancellationToken);
        return Ok(result);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleToUserCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("[action]")]
    public async Task<IActionResult> RemoveRoleFromUser([FromBody] RemoveRoleFromUserCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
