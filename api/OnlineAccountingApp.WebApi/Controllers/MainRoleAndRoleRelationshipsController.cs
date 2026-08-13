using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.Create;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.Delete;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.GetMainRoleAndRoleRelationshipById;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.GetMainRoleAndRoleRelationships;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.Update;

namespace OnlineAccountingApp.WebApi.Controllers;

public class MainRoleAndRoleRelationshipsController(IMediator mediator) : BaseApiController(mediator)
{
    [HttpPost("[action]")]
    public async Task<IActionResult> CreateMainRoleAndRoleRelationship(
        [FromBody] CreateMainRoleAndRoleRelationshipCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetMainRoleAndRoleRelationships(
        [FromQuery] GetMainRoleAndRoleRelationshipsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("[action]/{id}")]
    public async Task<IActionResult> GetMainRoleAndRoleRelationshipById(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMainRoleAndRoleRelationshipByIdQuery { Id = id }, cancellationToken);
        return Ok(result);
    }

    [HttpPut("[action]/{id}")]
    public async Task<IActionResult> UpdateMainRoleAndRoleRelationship(
        string id, [FromBody] UpdateMainRoleAndRoleRelationshipCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("[action]/{id}")]
    public async Task<IActionResult> DeleteMainRoleAndRoleRelationship(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteMainRoleAndRoleRelationshipCommand { Id = id }, cancellationToken);
        return Ok(result);
    }
}
