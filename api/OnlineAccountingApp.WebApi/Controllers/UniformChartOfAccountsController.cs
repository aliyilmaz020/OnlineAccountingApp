using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Create;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Delete;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetById;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Update;
using OnlineAccountingApp.Domain.Roles;
using OnlineAccountingApp.WebApi.Tenancy;

namespace OnlineAccountingApp.WebApi.Controllers;

[RequiresCompanyHeader]
public class UniformChartOfAccountsController(IMediator mediator) : BaseApiController(mediator)
{
    [HttpPost("[action]")]
    [Authorize(Policy = RoleList.UCAFCreateCode)]
    public async Task<IActionResult> CreateUniformChartOfAccount([FromBody] CreateUniformChartOfAccountCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("[action]")]
    [Authorize(Policy = RoleList.UCAFReadCode)]
    public async Task<IActionResult> GetUniformChartOfAccounts([FromQuery] GetUniformChartOfAccountsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("[action]/{id}")]
    [Authorize(Policy = RoleList.UCAFReadCode)]
    public async Task<IActionResult> GetUniformChartOfAccountById(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUniformChartOfAccountByIdQuery { Id = id }, cancellationToken);
        return Ok(result);
    }

    [HttpPut("[action]/{id}")]
    [Authorize(Policy = RoleList.UCAFUpdateCode)]
    public async Task<IActionResult> UpdateUniformChartOfAccount(string id, [FromBody] UpdateUniformChartOfAccountCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("[action]/{id}")]
    [Authorize(Policy = RoleList.UCAFDeleteCode)]
    public async Task<IActionResult> DeleteUniformChartOfAccount(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteUniformChartOfAccountCommand { Id = id }, cancellationToken);
        return Ok(result);
    }
}
