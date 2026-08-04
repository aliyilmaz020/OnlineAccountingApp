using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.GetMainRoleAndRoleRelationships;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.MainRoleAndRoleRelationshipFeature;

public class GetMainRoleAndRoleRelationshipsQueryHandlerTests
{
    private static MainRoleAndRoleRelationship BuildRelationship(string id, string roleId, string mainRoleId) => new()
    {
        Id = id,
        RoleId = roleId,
        MainRoleId = mainRoleId
    };

    [Fact]
    public async Task Handle_ShouldReturnPagedResult()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRoleAndRoleRelationship>> repository) =
            UnitOfWorkMockFactory.Create<MainRoleAndRoleRelationship>();

        Expression<Func<MainRoleAndRoleRelationship, bool>>? capturedPredicate = null;
        PagedResult<MainRoleAndRoleRelationship> pagedRelationships = new()
        {
            Items = [BuildRelationship("1", "role-1", "main-role-1"), BuildRelationship("2", "role-2", "main-role-1")],
            TotalCount = 2,
            PageNumber = 1,
            PageSize = 20
        };

        repository.Setup(r => r.GetPagedAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Expression<Func<MainRoleAndRoleRelationship, bool>>?>(),
                It.IsAny<Expression<Func<MainRoleAndRoleRelationship, object>>[]?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Callback<int, int, Expression<Func<MainRoleAndRoleRelationship, bool>>?, Expression<Func<MainRoleAndRoleRelationship, object>>[]?, bool, CancellationToken>(
                (_, _, predicate, _, _, _) => capturedPredicate = predicate)
            .ReturnsAsync(pagedRelationships);

        GetMainRoleAndRoleRelationshipsQueryHandler handler = new(unitOfWork.Object);
        GetMainRoleAndRoleRelationshipsQuery query = new() { PageNumber = 1, PageSize = 20 };

        PagedResult<MainRoleAndRoleRelationshipListItemDto> result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(capturedPredicate);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(20, result.PageSize);
        Assert.Equal(2, result.Items.Count);
        Assert.Contains(result.Items, i => i.RoleId == "role-1");
        Assert.Contains(result.Items, i => i.RoleId == "role-2");
    }
}
