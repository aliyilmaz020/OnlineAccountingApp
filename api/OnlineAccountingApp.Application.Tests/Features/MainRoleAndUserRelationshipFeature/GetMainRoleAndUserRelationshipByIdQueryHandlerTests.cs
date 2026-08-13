using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.GetMainRoleAndUserRelationshipById;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.GetMainRoleAndUserRelationships;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.MainRoleAndUserRelationshipFeature;

public class GetMainRoleAndUserRelationshipByIdQueryHandlerTests
{
    private static MainRoleAndUserRelationship ExistingRelationship() => new()
    {
        Id = "relationship-1",
        UserId = "user-1",
        MainRoleId = "main-role-1",
        CompanyId = "company-1"
    };

    [Fact]
    public async Task Handle_ShouldReturnRelationship_WhenExists()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRoleAndUserRelationship>> repository) =
            UnitOfWorkMockFactory.Create<MainRoleAndUserRelationship>();

        Expression<Func<MainRoleAndUserRelationship, bool>>? capturedPredicate = null;
        repository.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<MainRoleAndUserRelationship, bool>>>(),
                It.IsAny<Expression<Func<MainRoleAndUserRelationship, object>>[]?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Callback<Expression<Func<MainRoleAndUserRelationship, bool>>, Expression<Func<MainRoleAndUserRelationship, object>>[]?, bool, CancellationToken>(
                (predicate, _, _, _) => capturedPredicate = predicate)
            .ReturnsAsync(ExistingRelationship());

        GetMainRoleAndUserRelationshipByIdQueryHandler handler = new(unitOfWork.Object);
        GetMainRoleAndUserRelationshipByIdQuery query = new() { Id = "relationship-1" };

        MainRoleAndUserRelationshipListItemDto result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal("relationship-1", result.Id);
        Assert.Equal("user-1", result.UserId);
        Assert.Equal("main-role-1", result.MainRoleId);
        Assert.Equal("company-1", result.CompanyId);

        Assert.NotNull(capturedPredicate);
        Func<MainRoleAndUserRelationship, bool> compiledPredicate = capturedPredicate!.Compile();
        Assert.True(compiledPredicate(new MainRoleAndUserRelationship { Id = "relationship-1" }));
        Assert.False(compiledPredicate(new MainRoleAndUserRelationship { Id = "some-other-id" }));
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenMissing()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRoleAndUserRelationship>> repository) =
            UnitOfWorkMockFactory.Create<MainRoleAndUserRelationship>();

        repository.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<MainRoleAndUserRelationship, bool>>>(),
                It.IsAny<Expression<Func<MainRoleAndUserRelationship, object>>[]?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((MainRoleAndUserRelationship?)null);

        GetMainRoleAndUserRelationshipByIdQueryHandler handler = new(unitOfWork.Object);
        GetMainRoleAndUserRelationshipByIdQuery query = new() { Id = "missing-id" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(query, CancellationToken.None));

        Assert.Equal(AppErrorCodes.MainRoleAndUserRelationship.NotFound, exception.ErrorCode);
    }
}
