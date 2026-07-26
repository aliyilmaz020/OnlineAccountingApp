using Mapster;
using MediatR;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanyById;

public sealed class GetCompanyByIdQueryHandler(ICompanyService companyService) : IRequestHandler<GetCompanyByIdQuery, CompanyListItemDto>
{
    public async Task<CompanyListItemDto> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        Company? company = await companyService.GetByIdAsync(request.Id, cancellationToken);
        if (company is null)
        {
            throw new BusinessException(AppErrorCodes.Company.NotFound, "Company not found.");
        }

        return company.Adapt<CompanyListItemDto>();
    }
}
