using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.Delete;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Tests.Features.MainRoleFeature;

public class DeleteMainRoleCommandHandlerTests
{
    private static MainRole ExistingMainRole() => new()
    {
        Id = "main-role-1",
        Title = "Muhasebeci",
        IsRoleCreateByAdmin = false,
        CompanyId = "company-1"
    };

    [Fact]
    public async Task Handle_ShouldSoftDeleteMainRole_WhenExists()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRole>> repository) = UnitOfWorkMockFactory.Create<MainRole>();

        repository.Setup(r => r.GetByIdAsync("main-role-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingMainRole());
        repository.Setup(r => r.SoftDeleteAsync(It.IsAny<MainRole>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        DeleteMainRoleCommandHandler handler = new(unitOfWork.Object);
        DeleteMainRoleCommand command = new() { Id = "main-role-1" };

        bool result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        repository.Verify(r => r.SoftDeleteAsync(It.IsAny<MainRole>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenMainRoleDoesNotExist()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRole>> repository) = UnitOfWorkMockFactory.Create<MainRole>();

        repository.Setup(r => r.GetByIdAsync("main-role-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((MainRole?)null);

        DeleteMainRoleCommandHandler handler = new(unitOfWork.Object);
        DeleteMainRoleCommand command = new() { Id = "main-role-1" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.MainRole.NotFound, exception.ErrorCode);
        repository.Verify(r => r.SoftDeleteAsync(It.IsAny<MainRole>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
