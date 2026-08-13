using MediatR;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;

public sealed class GetRolesQuery : IRequest<PagedResult<RoleListItemDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchTerm { get; set; }
}
