using Mapster;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Create;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.Create;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.Update;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Create;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Update;
using OnlineAccountingApp.Domain.CompanyEntities;
using OnlineAccountingApp.Domain.Entities;
using OnlineAccountingApp.Domain.Entities.Identity;

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

    public static void RegisterRoleMappings()
    {
        // Id is assigned by the handler, never mapped in from the request.
        TypeAdapterConfig<CreateRoleCommand, AppRole>.NewConfig()
            .Ignore(role => role.Id);
        TypeAdapterConfig<UpdateRoleCommand, AppRole>.NewConfig()
            .Ignore(role => role.Id);
        TypeAdapterConfig<AppRole, RoleListItemDto>.NewConfig();
    }
}
