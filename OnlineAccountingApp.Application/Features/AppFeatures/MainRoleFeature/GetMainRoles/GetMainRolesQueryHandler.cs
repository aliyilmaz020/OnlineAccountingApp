using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Framework.MedatR.GetList;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.GetMainRoles;

public sealed class GetMainRolesQueryHandler(IUnitOfWork unitOfWork)
    : BaseGetListQueryHandler<GetMainRolesQuery, MainRole, MainRoleListItemDto>(unitOfWork)
{
    protected override Expression<Func<MainRole, bool>>? BuildPredicate(GetMainRolesQuery request)
        => string.IsNullOrWhiteSpace(request.SearchTerm)
            ? null
            : mainRole => mainRole.Title.Contains(request.SearchTerm);
}
