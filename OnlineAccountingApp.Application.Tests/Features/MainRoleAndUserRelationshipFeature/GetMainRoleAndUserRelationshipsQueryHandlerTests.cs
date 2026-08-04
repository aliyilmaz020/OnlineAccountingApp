using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.GetMainRoleAndUserRelationships;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.MainRoleAndUserRelationshipFeature;

public class GetMainRoleAndUserRelationshipsQueryHandlerTests
{
    private static MainRoleAndUserRelationship BuildRelationship(string id, string userId) => new()
    {
        Id = id,
        UserId = userId,
        MainRoleId = "main-role-1",
        CompanyId = "company-1"
    };

    [Fact]
    public async Task Handle_ShouldReturnPagedResult()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRoleAndUserRelationship>> repository) =
            UnitOfWorkMockFactory.Create<MainRoleAndUserRelationship>();

        Expression<Func<MainRoleAndUserRelationship, bool>>? capturedPredicate = null;
        PagedResult<MainRoleAndUserRelationship> pagedRelationships = new()
        {
            Items = [BuildRelationship("1", "user-1"), BuildRelationship("2", "user-2")],
            TotalCount = 2,
            PageNumber = 1,
            PageSize = 20
        };

        repository.Setup(r => r.GetPagedAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Expression<Func<MainRoleAndUserRelationship, bool>>?>(),
                It.IsAny<Expression<Func<MainRoleAndUserRelationship, object>>[]?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Callback<int, int, Expression<Func<MainRoleAndUserRelationship, bool>>?, Expression<Func<MainRoleAndUserRelationship, object>>[]?, bool, CancellationToken>(
                (_, _, predicate, _, _, _) => capturedPredicate = predicate)
            .ReturnsAsync(pagedRelationships);

        GetMainRoleAndUserRelationshipsQueryHandler handler = new(unitOfWork.Object);
        GetMainRoleAndUserRelationshipsQuery query = new() { PageNumber = 1, PageSize = 20 };

        PagedResult<MainRoleAndUserRelationshipListItemDto> result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(capturedPredicate);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(20, result.PageSize);
        Assert.Equal(2, result.Items.Count);
        Assert.Contains(result.Items, i => i.UserId == "user-1");
        Assert.Contains(result.Items, i => i.UserId == "user-2");
    }
}
