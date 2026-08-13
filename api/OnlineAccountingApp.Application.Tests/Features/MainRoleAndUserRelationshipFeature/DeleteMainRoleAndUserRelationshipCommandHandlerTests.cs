using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.Delete;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Tests.Features.MainRoleAndUserRelationshipFeature;

public class DeleteMainRoleAndUserRelationshipCommandHandlerTests
{
    private static MainRoleAndUserRelationship ExistingRelationship() => new()
    {
        Id = "relationship-1",
        UserId = "user-1",
        MainRoleId = "main-role-1",
        CompanyId = "company-1"
    };

    [Fact]
    public async Task Handle_ShouldSoftDeleteRelationship_WhenExists()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRoleAndUserRelationship>> repository) =
            UnitOfWorkMockFactory.Create<MainRoleAndUserRelationship>();

        repository.Setup(r => r.GetByIdAsync("relationship-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingRelationship());
        repository.Setup(r => r.SoftDeleteAsync(It.IsAny<MainRoleAndUserRelationship>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        DeleteMainRoleAndUserRelationshipCommandHandler handler = new(unitOfWork.Object);
        DeleteMainRoleAndUserRelationshipCommand command = new() { Id = "relationship-1" };

        bool result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        repository.Verify(r => r.SoftDeleteAsync(It.IsAny<MainRoleAndUserRelationship>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenRelationshipDoesNotExist()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRoleAndUserRelationship>> repository) =
            UnitOfWorkMockFactory.Create<MainRoleAndUserRelationship>();

        repository.Setup(r => r.GetByIdAsync("relationship-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((MainRoleAndUserRelationship?)null);

        DeleteMainRoleAndUserRelationshipCommandHandler handler = new(unitOfWork.Object);
        DeleteMainRoleAndUserRelationshipCommand command = new() { Id = "relationship-1" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.MainRoleAndUserRelationship.NotFound, exception.ErrorCode);
        repository.Verify(r => r.SoftDeleteAsync(It.IsAny<MainRoleAndUserRelationship>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
