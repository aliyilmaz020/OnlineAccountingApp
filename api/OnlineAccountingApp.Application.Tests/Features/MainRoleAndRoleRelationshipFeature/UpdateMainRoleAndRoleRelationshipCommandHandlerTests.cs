using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.GetMainRoleAndRoleRelationships;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.Update;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.MainRoleAndRoleRelationshipFeature;

public class UpdateMainRoleAndRoleRelationshipCommandHandlerTests
{
    private static MainRoleAndRoleRelationship ExistingRelationship() => new()
    {
        Id = "relationship-1",
        RoleId = "role-1",
        MainRoleId = "main-role-1"
    };

    private static UpdateMainRoleAndRoleRelationshipCommand BuildCommand() => new()
    {
        Id = "relationship-1",
        RoleId = "role-2",
        MainRoleId = "main-role-2"
    };

    [Fact]
    public async Task Handle_ShouldUpdateRelationship_WhenValid()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRoleAndRoleRelationship>> repository) =
            UnitOfWorkMockFactory.Create<MainRoleAndRoleRelationship>();

        repository.Setup(r => r.GetByIdAsync("relationship-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingRelationship());
        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<MainRoleAndRoleRelationship, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repository.Setup(r => r.UpdateAsync(It.IsAny<MainRoleAndRoleRelationship>(), It.IsAny<CancellationToken>()))
            .Returns<MainRoleAndRoleRelationship, CancellationToken>((entity, _) => Task.FromResult(entity));

        UpdateMainRoleAndRoleRelationshipCommandHandler handler = new(unitOfWork.Object);
        UpdateMainRoleAndRoleRelationshipCommand command = BuildCommand();

        MainRoleAndRoleRelationshipListItemDto result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("relationship-1", result.Id);
        Assert.Equal("role-2", result.RoleId);
        Assert.Equal("main-role-2", result.MainRoleId);

        repository.Verify(r => r.UpdateAsync(It.IsAny<MainRoleAndRoleRelationship>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenRelationshipDoesNotExist()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRoleAndRoleRelationship>> repository) =
            UnitOfWorkMockFactory.Create<MainRoleAndRoleRelationship>();

        repository.Setup(r => r.GetByIdAsync("relationship-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((MainRoleAndRoleRelationship?)null);

        UpdateMainRoleAndRoleRelationshipCommandHandler handler = new(unitOfWork.Object);
        UpdateMainRoleAndRoleRelationshipCommand command = BuildCommand();

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.MainRoleAndRoleRelationship.NotFound, exception.ErrorCode);
        repository.Verify(r => r.UpdateAsync(It.IsAny<MainRoleAndRoleRelationship>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowConflict_WhenAnotherRelationshipHasSamePair()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRoleAndRoleRelationship>> repository) =
            UnitOfWorkMockFactory.Create<MainRoleAndRoleRelationship>();

        repository.Setup(r => r.GetByIdAsync("relationship-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingRelationship());

        Expression<Func<MainRoleAndRoleRelationship, bool>>? capturedPredicate = null;
        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<MainRoleAndRoleRelationship, bool>>>(), It.IsAny<CancellationToken>()))
            .Callback<Expression<Func<MainRoleAndRoleRelationship, bool>>, CancellationToken>((predicate, _) => capturedPredicate = predicate)
            .ReturnsAsync(true);

        UpdateMainRoleAndRoleRelationshipCommandHandler handler = new(unitOfWork.Object);
        UpdateMainRoleAndRoleRelationshipCommand command = BuildCommand();

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.MainRoleAndRoleRelationship.AlreadyExists, exception.ErrorCode);
        repository.Verify(r => r.UpdateAsync(It.IsAny<MainRoleAndRoleRelationship>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.NotNull(capturedPredicate);
        Func<MainRoleAndRoleRelationship, bool> compiledPredicate = capturedPredicate!.Compile();
        Assert.True(compiledPredicate(new MainRoleAndRoleRelationship { Id = "relationship-2", RoleId = command.RoleId, MainRoleId = command.MainRoleId }));
        Assert.False(compiledPredicate(new MainRoleAndRoleRelationship { Id = "relationship-1", RoleId = command.RoleId, MainRoleId = command.MainRoleId }));
        Assert.False(compiledPredicate(new MainRoleAndRoleRelationship { Id = "relationship-2", RoleId = "role-9", MainRoleId = command.MainRoleId }));
    }
}
