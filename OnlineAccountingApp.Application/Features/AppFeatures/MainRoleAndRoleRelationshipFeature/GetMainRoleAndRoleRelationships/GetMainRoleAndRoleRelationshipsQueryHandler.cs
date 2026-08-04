using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Framework.MedatR.GetList;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.GetMainRoleAndRoleRelationships;

public sealed class GetMainRoleAndRoleRelationshipsQueryHandler(IUnitOfWork unitOfWork)
    : BaseGetListQueryHandler<GetMainRoleAndRoleRelationshipsQuery, MainRoleAndRoleRelationship, MainRoleAndRoleRelationshipListItemDto>(unitOfWork)
{
}
