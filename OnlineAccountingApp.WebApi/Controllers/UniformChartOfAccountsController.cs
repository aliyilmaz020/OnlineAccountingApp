using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Create;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Delete;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetById;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Update;
using OnlineAccountingApp.WebApi.Tenancy;

namespace OnlineAccountingApp.WebApi.Controllers;

[RequiresCompanyHeader]
public class UniformChartOfAccountsController(IMediator mediator) : BaseApiController(mediator)
{
    [HttpPost("[action]")]
    public async Task<IActionResult> CreateUniformChartOfAccount([FromBody] CreateUniformChartOfAccountCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetUniformChartOfAccounts([FromQuery] GetUniformChartOfAccountsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("[action]/{id}")]
    public async Task<IActionResult> GetUniformChartOfAccountById(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUniformChartOfAccountByIdQuery { Id = id }, cancellationToken);
        return Ok(result);
    }

    [HttpPut("[action]/{id}")]
    public async Task<IActionResult> UpdateUniformChartOfAccount(string id, [FromBody] UpdateUniformChartOfAccountCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("[action]/{id}")]
    public async Task<IActionResult> DeleteUniformChartOfAccount(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteUniformChartOfAccountCommand { Id = id }, cancellationToken);
        return Ok(result);
    }
}
