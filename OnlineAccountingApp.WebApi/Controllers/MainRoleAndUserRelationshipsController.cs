using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.Create;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.Delete;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.GetMainRoleAndUserRelationshipById;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.GetMainRoleAndUserRelationships;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.Update;

namespace OnlineAccountingApp.WebApi.Controllers;

public class MainRoleAndUserRelationshipsController(IMediator mediator) : BaseApiController(mediator)
{
    [HttpPost("[action]")]
    public async Task<IActionResult> CreateMainRoleAndUserRelationship(
        [FromBody] CreateMainRoleAndUserRelationshipCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetMainRoleAndUserRelationships(
        [FromQuery] GetMainRoleAndUserRelationshipsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("[action]/{id}")]
    public async Task<IActionResult> GetMainRoleAndUserRelationshipById(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMainRoleAndUserRelationshipByIdQuery { Id = id }, cancellationToken);
        return Ok(result);
    }

    [HttpPut("[action]/{id}")]
    public async Task<IActionResult> UpdateMainRoleAndUserRelationship(
        string id, [FromBody] UpdateMainRoleAndUserRelationshipCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("[action]/{id}")]
    public async Task<IActionResult> DeleteMainRoleAndUserRelationship(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteMainRoleAndUserRelationshipCommand { Id = id }, cancellationToken);
        return Ok(result);
    }
}
