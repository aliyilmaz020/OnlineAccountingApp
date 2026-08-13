using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.Delete;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Tests.Features.MainRoleAndRoleRelationshipFeature;

public class DeleteMainRoleAndRoleRelationshipCommandHandlerTests
{
    private static MainRoleAndRoleRelationship ExistingRelationship() => new()
    {
        Id = "relationship-1",
        RoleId = "role-1",
        MainRoleId = "main-role-1"
    };

    [Fact]
    public async Task Handle_ShouldSoftDeleteRelationship_WhenExists()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRoleAndRoleRelationship>> repository) =
            UnitOfWorkMockFactory.Create<MainRoleAndRoleRelationship>();

        repository.Setup(r => r.GetByIdAsync("relationship-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingRelationship());
        repository.Setup(r => r.SoftDeleteAsync(It.IsAny<MainRoleAndRoleRelationship>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        DeleteMainRoleAndRoleRelationshipCommandHandler handler = new(unitOfWork.Object);
        DeleteMainRoleAndRoleRelationshipCommand command = new() { Id = "relationship-1" };

        bool result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        repository.Verify(r => r.SoftDeleteAsync(It.IsAny<MainRoleAndRoleRelationship>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenRelationshipDoesNotExist()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRoleAndRoleRelationship>> repository) =
            UnitOfWorkMockFactory.Create<MainRoleAndRoleRelationship>();

        repository.Setup(r => r.GetByIdAsync("relationship-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((MainRoleAndRoleRelationship?)null);

        DeleteMainRoleAndRoleRelationshipCommandHandler handler = new(unitOfWork.Object);
        DeleteMainRoleAndRoleRelationshipCommand command = new() { Id = "relationship-1" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.MainRoleAndRoleRelationship.NotFound, exception.ErrorCode);
        repository.Verify(r => r.SoftDeleteAsync(It.IsAny<MainRoleAndRoleRelationship>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
