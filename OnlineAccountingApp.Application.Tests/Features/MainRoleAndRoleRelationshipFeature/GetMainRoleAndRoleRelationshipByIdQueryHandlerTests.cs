using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.GetMainRoleAndRoleRelationshipById;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.GetMainRoleAndRoleRelationships;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.MainRoleAndRoleRelationshipFeature;

public class GetMainRoleAndRoleRelationshipByIdQueryHandlerTests
{
    private static MainRoleAndRoleRelationship ExistingRelationship() => new()
    {
        Id = "relationship-1",
        RoleId = "role-1",
        MainRoleId = "main-role-1"
    };

    [Fact]
    public async Task Handle_ShouldReturnRelationship_WhenExists()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRoleAndRoleRelationship>> repository) =
            UnitOfWorkMockFactory.Create<MainRoleAndRoleRelationship>();

        Expression<Func<MainRoleAndRoleRelationship, bool>>? capturedPredicate = null;
        repository.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<MainRoleAndRoleRelationship, bool>>>(),
                It.IsAny<Expression<Func<MainRoleAndRoleRelationship, object>>[]?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Callback<Expression<Func<MainRoleAndRoleRelationship, bool>>, Expression<Func<MainRoleAndRoleRelationship, object>>[]?, bool, CancellationToken>(
                (predicate, _, _, _) => capturedPredicate = predicate)
            .ReturnsAsync(ExistingRelationship());

        GetMainRoleAndRoleRelationshipByIdQueryHandler handler = new(unitOfWork.Object);
        GetMainRoleAndRoleRelationshipByIdQuery query = new() { Id = "relationship-1" };

        MainRoleAndRoleRelationshipListItemDto result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal("relationship-1", result.Id);
        Assert.Equal("role-1", result.RoleId);
        Assert.Equal("main-role-1", result.MainRoleId);

        Assert.NotNull(capturedPredicate);
        Func<MainRoleAndRoleRelationship, bool> compiledPredicate = capturedPredicate!.Compile();
        Assert.True(compiledPredicate(new MainRoleAndRoleRelationship { Id = "relationship-1" }));
        Assert.False(compiledPredicate(new MainRoleAndRoleRelationship { Id = "some-other-id" }));
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenMissing()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRoleAndRoleRelationship>> repository) =
            UnitOfWorkMockFactory.Create<MainRoleAndRoleRelationship>();

        repository.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<MainRoleAndRoleRelationship, bool>>>(),
                It.IsAny<Expression<Func<MainRoleAndRoleRelationship, object>>[]?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((MainRoleAndRoleRelationship?)null);

        GetMainRoleAndRoleRelationshipByIdQueryHandler handler = new(unitOfWork.Object);
        GetMainRoleAndRoleRelationshipByIdQuery query = new() { Id = "missing-id" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(query, CancellationToken.None));

        Assert.Equal(AppErrorCodes.MainRoleAndRoleRelationship.NotFound, exception.ErrorCode);
    }
}
