using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.GetMainRoleById;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.GetMainRoles;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.MainRoleFeature;

public class GetMainRoleByIdQueryHandlerTests
{
    private static MainRole ExistingMainRole() => new()
    {
        Id = "main-role-1",
        Title = "Muhasebeci",
        IsRoleCreateByAdmin = false,
        CompanyId = "company-1"
    };

    [Fact]
    public async Task Handle_ShouldReturnMainRole_WhenExists()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRole>> repository) = UnitOfWorkMockFactory.Create<MainRole>();

        Expression<Func<MainRole, bool>>? capturedPredicate = null;
        repository.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<MainRole, bool>>>(),
                It.IsAny<Expression<Func<MainRole, object>>[]?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Callback<Expression<Func<MainRole, bool>>, Expression<Func<MainRole, object>>[]?, bool, CancellationToken>(
                (predicate, _, _, _) => capturedPredicate = predicate)
            .ReturnsAsync(ExistingMainRole());

        GetMainRoleByIdQueryHandler handler = new(unitOfWork.Object);
        GetMainRoleByIdQuery query = new() { Id = "main-role-1" };

        MainRoleListItemDto result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal("main-role-1", result.Id);
        Assert.Equal("Muhasebeci", result.Title);

        Assert.NotNull(capturedPredicate);
        Func<MainRole, bool> compiledPredicate = capturedPredicate!.Compile();
        Assert.True(compiledPredicate(new MainRole { Id = "main-role-1" }));
        Assert.False(compiledPredicate(new MainRole { Id = "some-other-id" }));
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenMissing()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRole>> repository) = UnitOfWorkMockFactory.Create<MainRole>();

        repository.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<MainRole, bool>>>(),
                It.IsAny<Expression<Func<MainRole, object>>[]?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((MainRole?)null);

        GetMainRoleByIdQueryHandler handler = new(unitOfWork.Object);
        GetMainRoleByIdQuery query = new() { Id = "missing-id" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(query, CancellationToken.None));

        Assert.Equal(AppErrorCodes.MainRole.NotFound, exception.ErrorCode);
    }
}
