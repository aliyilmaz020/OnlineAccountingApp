using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.GetMainRoleAndUserRelationships;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.Update;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.MainRoleAndUserRelationshipFeature;

public class UpdateMainRoleAndUserRelationshipCommandHandlerTests
{
    private static MainRoleAndUserRelationship ExistingRelationship() => new()
    {
        Id = "relationship-1",
        UserId = "user-1",
        MainRoleId = "main-role-1",
        CompanyId = "company-1"
    };

    private static UpdateMainRoleAndUserRelationshipCommand BuildCommand() => new()
    {
        Id = "relationship-1",
        UserId = "user-1",
        MainRoleId = "main-role-2",
        CompanyId = "company-1"
    };

    [Fact]
    public async Task Handle_ShouldUpdateRelationship_WhenValid()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRoleAndUserRelationship>> repository) =
            UnitOfWorkMockFactory.Create<MainRoleAndUserRelationship>();

        repository.Setup(r => r.GetByIdAsync("relationship-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingRelationship());
        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<MainRoleAndUserRelationship, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repository.Setup(r => r.UpdateAsync(It.IsAny<MainRoleAndUserRelationship>(), It.IsAny<CancellationToken>()))
            .Returns<MainRoleAndUserRelationship, CancellationToken>((entity, _) => Task.FromResult(entity));

        UpdateMainRoleAndUserRelationshipCommandHandler handler = new(unitOfWork.Object);
        UpdateMainRoleAndUserRelationshipCommand command = BuildCommand();

        MainRoleAndUserRelationshipListItemDto result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("relationship-1", result.Id);
        Assert.Equal("main-role-2", result.MainRoleId);

        repository.Verify(r => r.UpdateAsync(It.IsAny<MainRoleAndUserRelationship>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenRelationshipDoesNotExist()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRoleAndUserRelationship>> repository) =
            UnitOfWorkMockFactory.Create<MainRoleAndUserRelationship>();

        repository.Setup(r => r.GetByIdAsync("relationship-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((MainRoleAndUserRelationship?)null);

        UpdateMainRoleAndUserRelationshipCommandHandler handler = new(unitOfWork.Object);
        UpdateMainRoleAndUserRelationshipCommand command = BuildCommand();

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.MainRoleAndUserRelationship.NotFound, exception.ErrorCode);
        repository.Verify(r => r.UpdateAsync(It.IsAny<MainRoleAndUserRelationship>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowConflict_WhenAnotherRelationshipHasSameTriple()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRoleAndUserRelationship>> repository) =
            UnitOfWorkMockFactory.Create<MainRoleAndUserRelationship>();

        repository.Setup(r => r.GetByIdAsync("relationship-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingRelationship());

        Expression<Func<MainRoleAndUserRelationship, bool>>? capturedPredicate = null;
        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<MainRoleAndUserRelationship, bool>>>(), It.IsAny<CancellationToken>()))
            .Callback<Expression<Func<MainRoleAndUserRelationship, bool>>, CancellationToken>((predicate, _) => capturedPredicate = predicate)
            .ReturnsAsync(true);

        UpdateMainRoleAndUserRelationshipCommandHandler handler = new(unitOfWork.Object);
        UpdateMainRoleAndUserRelationshipCommand command = BuildCommand();

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.MainRoleAndUserRelationship.AlreadyExists, exception.ErrorCode);
        repository.Verify(r => r.UpdateAsync(It.IsAny<MainRoleAndUserRelationship>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.NotNull(capturedPredicate);
        Func<MainRoleAndUserRelationship, bool> compiledPredicate = capturedPredicate!.Compile();
        Assert.True(compiledPredicate(new MainRoleAndUserRelationship
        {
            Id = "relationship-2",
            UserId = command.UserId,
            MainRoleId = command.MainRoleId,
            CompanyId = command.CompanyId
        }));
        Assert.False(compiledPredicate(new MainRoleAndUserRelationship
        {
            Id = "relationship-1",
            UserId = command.UserId,
            MainRoleId = command.MainRoleId,
            CompanyId = command.CompanyId
        }));
        Assert.False(compiledPredicate(new MainRoleAndUserRelationship
        {
            Id = "relationship-2",
            UserId = "user-9",
            MainRoleId = command.MainRoleId,
            CompanyId = command.CompanyId
        }));
    }
}
