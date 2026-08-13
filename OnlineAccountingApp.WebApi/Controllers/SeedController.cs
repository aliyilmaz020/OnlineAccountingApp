using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineAccountingApp.Application.Features.AppFeatures.SeedFeature.SeedSampleData;

namespace OnlineAccountingApp.WebApi.Controllers;

public class SeedController(IMediator mediator) : BaseApiController(mediator)
{
    [HttpPost("[action]")]
    public async Task<IActionResult> SeedSampleData(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new SeedSampleDataCommand(), cancellationToken);
        return Ok(result);
    }
}
