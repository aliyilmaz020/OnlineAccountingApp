using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Create;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.MigrateCompanyDb;

namespace OnlineAccountingApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController(IMediator mediator) : ControllerBase
{
    [HttpPost("[action]")]
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> MigrateCompanyDb()
    {
        var result = await mediator.Send(new MigrateCompanyDbCommand());
        return Ok(result);
    }
}
