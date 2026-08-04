using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Framework.MedatR.GetList;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.GetMainRoleAndUserRelationships;

public sealed class GetMainRoleAndUserRelationshipsQueryHandler(IUnitOfWork unitOfWork)
    : BaseGetListQueryHandler<GetMainRoleAndUserRelationshipsQuery, MainRoleAndUserRelationship, MainRoleAndUserRelationshipListItemDto>(unitOfWork)
{
}
