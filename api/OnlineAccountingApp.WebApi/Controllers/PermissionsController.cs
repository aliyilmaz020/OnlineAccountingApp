using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineAccountingApp.Application.Features.AppFeatures.PermissionFeature.GetMyPermissions;
using OnlineAccountingApp.WebApi.Tenancy;

namespace OnlineAccountingApp.WebApi.Controllers;

[RequiresCompanyHeader]
public class PermissionsController(IMediator mediator) : BaseApiController(mediator)
{
    [HttpGet("[action]")]
    public async Task<IActionResult> GetMyPermissions(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMyPermissionsQuery(), cancellationToken);
        return Ok(result);
    }
}
