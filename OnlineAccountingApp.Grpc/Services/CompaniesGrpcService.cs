using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Create;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Delete;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanyById;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.MigrateCompanyDb;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Update;
using OnlineAccountingApp.Application.Services;
using OnlineAccountingApp.Grpc.Protos;

namespace OnlineAccountingApp.Grpc.Services;

[Authorize]
public sealed class CompaniesGrpcService(IMediator mediator) : Companies.CompaniesBase
{
    public override async Task<CompanyItem> CreateCompany(CreateCompanyRequest request, ServerCallContext context)
    {
        CompanyListItemDto result = await mediator.Send(new CreateCompanyCommand
        {
            Name = request.Name,
            Address = request.Address,
            IdentityNumber = request.IdentityNumber,
            TaxDepartment = request.TaxDepartment,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            ServerName = request.ServerName,
            DatabaseName = request.DatabaseName,
            ServerUserId = request.ServerUserId,
            ServerPassword = request.ServerPassword
        }, context.CancellationToken);

        return ToItem(result);
    }

    public override async Task<GetCompaniesResponse> GetCompanies(GetCompaniesRequest request, ServerCallContext context)
    {
        PagedResult<CompanyListItemDto> result = await mediator.Send(new GetCompaniesQuery
        {
            PageNumber = request.PageNumber == 0 ? 1 : request.PageNumber,
            PageSize = request.PageSize == 0 ? 20 : request.PageSize,
            SearchTerm = request.HasSearchTerm ? request.SearchTerm : null
        }, context.CancellationToken);

        GetCompaniesResponse response = new()
        {
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };
        response.Items.AddRange(result.Items.Select(ToItem));
        return response;
    }

    public override async Task<CompanyItem> GetCompanyById(GetCompanyByIdRequest request, ServerCallContext context)
    {
        CompanyListItemDto result = await mediator.Send(
            new GetCompanyByIdQuery { Id = request.Id }, context.CancellationToken);

        return ToItem(result);
    }

    public override async Task<CompanyItem> UpdateCompany(UpdateCompanyRequest request, ServerCallContext context)
    {
        CompanyListItemDto result = await mediator.Send(new UpdateCompanyCommand
        {
            Id = request.Id,
            Name = request.Name,
            Address = request.Address,
            IdentityNumber = request.IdentityNumber,
            TaxDepartment = request.TaxDepartment,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            ServerName = request.ServerName,
            DatabaseName = request.DatabaseName,
            ServerUserId = request.ServerUserId,
            ServerPassword = request.ServerPassword
        }, context.CancellationToken);

        return ToItem(result);
    }

    public override async Task<DeleteCompanyResponse> DeleteCompany(GetCompanyByIdRequest request, ServerCallContext context)
    {
        bool success = await mediator.Send(new DeleteCompanyCommand { Id = request.Id }, context.CancellationToken);
        return new DeleteCompanyResponse { Success = success };
    }

    public override async Task<MigrateCompanyDbResponse> MigrateCompanyDb(Empty request, ServerCallContext context)
    {
        bool success = await mediator.Send(new MigrateCompanyDbCommand(), context.CancellationToken);
        return new MigrateCompanyDbResponse { Success = success };
    }

    private static CompanyItem ToItem(CompanyListItemDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Address = dto.Address,
        IdentityNumber = dto.IdentityNumber,
        TaxDepartment = dto.TaxDepartment,
        PhoneNumber = dto.PhoneNumber,
        Email = dto.Email
    };
}
