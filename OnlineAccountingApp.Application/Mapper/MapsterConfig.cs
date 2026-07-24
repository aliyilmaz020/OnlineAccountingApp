using Mapster;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Create;
using OnlineAccountingApp.Domain.Entities;

namespace OnlineAccountingApp.Application.Mapper;

public static class MapsterConfig
{
    public static void RegisterCompanyMappings()
    {
        TypeAdapterConfig<Company, CreateCompanyCommand>.NewConfig();
    }
}
