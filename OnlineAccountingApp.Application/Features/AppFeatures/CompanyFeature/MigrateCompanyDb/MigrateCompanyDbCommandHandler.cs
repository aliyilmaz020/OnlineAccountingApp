using MediatR;
using OnlineAccountingApp.Application.Services.AppServices;

namespace OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.MigrateCompanyDb;

public sealed class MigrateCompanyDbCommandHandler(ICompanyService companyService) : IRequestHandler<MigrateCompanyDbCommand, bool>
{
    public async Task<bool> Handle(MigrateCompanyDbCommand request, CancellationToken cancellationToken)
    {
        return await companyService.MigrateCompanyDbAsync();
    }
}
