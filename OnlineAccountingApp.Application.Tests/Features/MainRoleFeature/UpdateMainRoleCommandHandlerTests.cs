using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.GetMainRoles;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.Update;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.MainRoleFeature;

public class UpdateMainRoleCommandHandlerTests
{
    private static MainRole ExistingMainRole() => new()
    {
        Id = "main-role-1",
        Title = "Old Title",
        IsRoleCreateByAdmin = false,
        CompanyId = "company-1"
    };

    private static UpdateMainRoleCommand BuildCommand() => new()
    {
        Id = "main-role-1",
        Title = "New Title",
        IsRoleCreateByAdmin = true,
        CompanyId = "company-1"
    };

    [Fact]
    public async Task Handle_ShouldUpdateMainRole_WhenValid()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRole>> repository) = UnitOfWorkMockFactory.Create<MainRole>();

        repository.Setup(r => r.GetByIdAsync("main-role-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingMainRole());
        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<MainRole, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repository.Setup(r => r.UpdateAsync(It.IsAny<MainRole>(), It.IsAny<CancellationToken>()))
            .Returns<MainRole, CancellationToken>((entity, _) => Task.FromResult(entity));

        UpdateMainRoleCommandHandler handler = new(unitOfWork.Object);
        UpdateMainRoleCommand command = BuildCommand();

        MainRoleListItemDto result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("main-role-1", result.Id);
        Assert.Equal("New Title", result.Title);
        Assert.True(result.IsRoleCreateByAdmin);

        repository.Verify(r => r.UpdateAsync(It.IsAny<MainRole>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenMainRoleDoesNotExist()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRole>> repository) = UnitOfWorkMockFactory.Create<MainRole>();

        repository.Setup(r => r.GetByIdAsync("main-role-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((MainRole?)null);

        UpdateMainRoleCommandHandler handler = new(unitOfWork.Object);
        UpdateMainRoleCommand command = BuildCommand();

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.MainRole.NotFound, exception.ErrorCode);
        repository.Verify(r => r.UpdateAsync(It.IsAny<MainRole>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowConflict_WhenAnotherMainRoleHasSameTitleInCompany()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRole>> repository) = UnitOfWorkMockFactory.Create<MainRole>();

        repository.Setup(r => r.GetByIdAsync("main-role-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingMainRole());

        Expression<Func<MainRole, bool>>? capturedPredicate = null;
        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<MainRole, bool>>>(), It.IsAny<CancellationToken>()))
            .Callback<Expression<Func<MainRole, bool>>, CancellationToken>((predicate, _) => capturedPredicate = predicate)
            .ReturnsAsync(true);

        UpdateMainRoleCommandHandler handler = new(unitOfWork.Object);
        UpdateMainRoleCommand command = BuildCommand();

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.MainRole.AlreadyExists, exception.ErrorCode);
        repository.Verify(r => r.UpdateAsync(It.IsAny<MainRole>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.NotNull(capturedPredicate);
        Func<MainRole, bool> compiledPredicate = capturedPredicate!.Compile();
        Assert.True(compiledPredicate(new MainRole { Id = "main-role-2", Title = command.Title, CompanyId = command.CompanyId }));
        Assert.False(compiledPredicate(new MainRole { Id = "main-role-1", Title = command.Title, CompanyId = command.CompanyId }));
        Assert.False(compiledPredicate(new MainRole { Id = "main-role-2", Title = "Different Title", CompanyId = command.CompanyId }));
    }
}
