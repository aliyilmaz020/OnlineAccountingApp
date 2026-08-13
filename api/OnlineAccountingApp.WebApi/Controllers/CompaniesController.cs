using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Create;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Delete;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanyById;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.MigrateCompanyDb;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Update;

namespace OnlineAccountingApp.WebApi.Controllers;

public class CompaniesController(IMediator mediator) : BaseApiController(mediator)
{
    [HttpPost("[action]")]
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetCompanies([FromQuery] GetCompaniesQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("[action]/{id}")]
    public async Task<IActionResult> GetCompanyById(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCompanyByIdQuery { Id = id }, cancellationToken);
        return Ok(result);
    }

    [HttpPut("[action]/{id}")]
    public async Task<IActionResult> UpdateCompany(string id, [FromBody] UpdateCompanyCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("[action]/{id}")]
    public async Task<IActionResult> DeleteCompany(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteCompanyCommand { Id = id }, cancellationToken);
        return Ok(result);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> MigrateCompanyDb(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new MigrateCompanyDbCommand(), cancellationToken);
        return Ok(result);
    }
}
