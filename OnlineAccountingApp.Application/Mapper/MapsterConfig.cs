using Mapster;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Create;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Create;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Update;
using OnlineAccountingApp.Domain.CompanyEntities;
using OnlineAccountingApp.Domain.Entities;

namespace OnlineAccountingApp.Application.Mapper;

public static class MapsterConfig
{
    public static void RegisterCompanyMappings()
    {
        TypeAdapterConfig<Company, CreateCompanyCommand>.NewConfig();
    }

    public static void RegisterUniformChartOfAccountMappings()
    {
        TypeAdapterConfig<CreateUniformChartOfAccountCommand, UniformChartOfAccount>.NewConfig();
        TypeAdapterConfig<UpdateUniformChartOfAccountCommand, UniformChartOfAccount>.NewConfig();
        TypeAdapterConfig<UniformChartOfAccount, UniformChartOfAccountListItemDto>.NewConfig();
    }
}
