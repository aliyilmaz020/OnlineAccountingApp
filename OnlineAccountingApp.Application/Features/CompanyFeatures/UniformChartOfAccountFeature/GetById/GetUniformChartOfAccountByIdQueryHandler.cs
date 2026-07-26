using Mapster;
using MediatR;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.CompanyEntities;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetById;

public sealed class GetUniformChartOfAccountByIdQueryHandler(IUniformChartOfAccountService uniformChartOfAccountService) : IRequestHandler<GetUniformChartOfAccountByIdQuery, UniformChartOfAccountListItemDto>
{
    public async Task<UniformChartOfAccountListItemDto> Handle(GetUniformChartOfAccountByIdQuery request, CancellationToken cancellationToken)
    {
        UniformChartOfAccount? account = await uniformChartOfAccountService.GetByIdAsync(request.Id, cancellationToken);
        if (account is null || account.Deleted)
        {
            throw new BusinessException(AppErrorCodes.UniformChartOfAccount.NotFound, "Uniform chart of account not found.");
        }

        return account.Adapt<UniformChartOfAccountListItemDto>();
    }
}
