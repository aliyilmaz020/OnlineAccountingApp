using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.AssignRoleToUser;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.Create;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.Delete;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoleById;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetUserRoles;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.RemoveRoleFromUser;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.Update;
using OnlineAccountingApp.Framework.Services;
using OnlineAccountingApp.Grpc.Protos;

namespace OnlineAccountingApp.Grpc.Services;

[Authorize]
public sealed class RolesGrpcService(IMediator mediator) : Roles.RolesBase
{
    public override async Task<RoleItem> CreateRole(CreateRoleRequest request, ServerCallContext context)
    {
        RoleListItemDto result = await mediator.Send(
            new CreateRoleCommand { Name = request.Name, Code = request.Code }, context.CancellationToken);

        return ToItem(result);
    }

    public override async Task<GetRolesResponse> GetRoles(GetRolesRequest request, ServerCallContext context)
    {
        PagedResult<RoleListItemDto> result = await mediator.Send(new GetRolesQuery
        {
            PageNumber = request.PageNumber == 0 ? 1 : request.PageNumber,
            PageSize = request.PageSize == 0 ? 20 : request.PageSize,
            SearchTerm = request.HasSearchTerm ? request.SearchTerm : null
        }, context.CancellationToken);

        GetRolesResponse response = new()
        {
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };
        response.Items.AddRange(result.Items.Select(ToItem));
        return response;
    }

    public override async Task<RoleItem> GetRoleById(GetRoleByIdRequest request, ServerCallContext context)
    {
        RoleListItemDto result = await mediator.Send(
            new GetRoleByIdQuery { Id = request.Id }, context.CancellationToken);

        return ToItem(result);
    }

    public override async Task<RoleItem> UpdateRole(UpdateRoleRequest request, ServerCallContext context)
    {
        RoleListItemDto result = await mediator.Send(new UpdateRoleCommand
        {
            Id = request.Id,
            Name = request.Name,
            Code = request.Code,
            Status = request.Status
        }, context.CancellationToken);

        return ToItem(result);
    }

    public override async Task<DeleteRoleResponse> DeleteRole(GetRoleByIdRequest request, ServerCallContext context)
    {
        bool success = await mediator.Send(new DeleteRoleCommand { Id = request.Id }, context.CancellationToken);
        return new DeleteRoleResponse { Success = success };
    }

    public override async Task<AssignRoleToUserResponse> AssignRoleToUser(AssignRoleToUserRequest request, ServerCallContext context)
    {
        bool success = await mediator.Send(
            new AssignRoleToUserCommand { UserId = request.UserId, RoleCode = request.RoleCode }, context.CancellationToken);

        return new AssignRoleToUserResponse { Success = success };
    }

    public override async Task<RemoveRoleFromUserResponse> RemoveRoleFromUser(RemoveRoleFromUserRequest request, ServerCallContext context)
    {
        bool success = await mediator.Send(
            new RemoveRoleFromUserCommand { UserId = request.UserId, RoleName = request.RoleName }, context.CancellationToken);

        return new RemoveRoleFromUserResponse { Success = success };
    }

    public override async Task<GetUserRolesResponse> GetUserRoles(GetUserRolesRequest request, ServerCallContext context)
    {
        List<RoleListItemDto> result = await mediator.Send(
            new GetUserRolesQuery { UserId = request.UserId }, context.CancellationToken);

        GetUserRolesResponse response = new();
        response.Items.AddRange(result.Select(ToItem));
        return response;
    }

    private static RoleItem ToItem(RoleListItemDto dto)
    {
        RoleItem item = new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Code = dto.Code,
            Status = dto.Status,
            CreateDate = Timestamp.FromDateTime(DateTime.SpecifyKind(dto.CreateDate, DateTimeKind.Utc))
        };
        if (dto.EditDate.HasValue)
        {
            item.EditDate = Timestamp.FromDateTime(DateTime.SpecifyKind(dto.EditDate.Value, DateTimeKind.Utc));
        }
        return item;
    }
}
