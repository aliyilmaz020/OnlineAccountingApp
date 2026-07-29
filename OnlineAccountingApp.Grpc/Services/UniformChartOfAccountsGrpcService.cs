using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Create;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Delete;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetById;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Update;
using OnlineAccountingApp.Application.Services;
using OnlineAccountingApp.Grpc.Protos;

namespace OnlineAccountingApp.Grpc.Services;

[Authorize]
public sealed class UniformChartOfAccountsGrpcService(IMediator mediator)
    : UniformChartOfAccounts.UniformChartOfAccountsBase
{
    public override async Task<UniformChartOfAccountItem> CreateUniformChartOfAccount(
        CreateUniformChartOfAccountRequest request, ServerCallContext context)
    {
        UniformChartOfAccountListItemDto result = await mediator.Send(new CreateUniformChartOfAccountCommand
        {
            Code = request.Code,
            Name = request.Name,
            Type = request.Type
        }, context.CancellationToken);

        UniformChartOfAccountItem item = ToItem(result);
        item.Meta = new ApiResponseMeta { Success = true };
        return item;
    }

    public override async Task<GetUniformChartOfAccountsResponse> GetUniformChartOfAccounts(
        GetUniformChartOfAccountsRequest request, ServerCallContext context)
    {
        PagedResult<UniformChartOfAccountListItemDto> result = await mediator.Send(new GetUniformChartOfAccountsQuery
        {
            PageNumber = request.PageNumber == 0 ? 1 : request.PageNumber,
            PageSize = request.PageSize == 0 ? 20 : request.PageSize,
            SearchTerm = request.HasSearchTerm ? request.SearchTerm : null
        }, context.CancellationToken);

        GetUniformChartOfAccountsResponse response = new()
        {
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages,
            Meta = new ApiResponseMeta { Success = true }
        };
        response.Items.AddRange(result.Items.Select(ToItem));
        return response;
    }

    public override async Task<UniformChartOfAccountItem> GetUniformChartOfAccountById(
        GetByIdRequest request, ServerCallContext context)
    {
        UniformChartOfAccountListItemDto result = await mediator.Send(
            new GetUniformChartOfAccountByIdQuery { Id = request.Id }, context.CancellationToken);

        UniformChartOfAccountItem item = ToItem(result);
        item.Meta = new ApiResponseMeta { Success = true };
        return item;
    }

    public override async Task<UniformChartOfAccountItem> UpdateUniformChartOfAccount(
        UpdateUniformChartOfAccountRequest request, ServerCallContext context)
    {
        UniformChartOfAccountListItemDto result = await mediator.Send(new UpdateUniformChartOfAccountCommand
        {
            Id = request.Id,
            Code = request.Code,
            Name = request.Name,
            Type = request.Type
        }, context.CancellationToken);

        UniformChartOfAccountItem item = ToItem(result);
        item.Meta = new ApiResponseMeta { Success = true };
        return item;
    }

    public override async Task<DeleteResponse> DeleteUniformChartOfAccount(
        GetByIdRequest request, ServerCallContext context)
    {
        bool success = await mediator.Send(
            new DeleteUniformChartOfAccountCommand { Id = request.Id }, context.CancellationToken);

        return new DeleteResponse { Success = success, Meta = new ApiResponseMeta { Success = true } };
    }

    private static UniformChartOfAccountItem ToItem(UniformChartOfAccountListItemDto dto) => new()
    {
        Id = dto.Id,
        Code = dto.Code,
        Name = dto.Name,
        Type = dto.Type
    };
}
