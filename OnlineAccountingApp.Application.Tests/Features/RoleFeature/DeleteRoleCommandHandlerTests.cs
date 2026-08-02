using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.Delete;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Tests.Features.RoleFeature;

public class DeleteRoleCommandHandlerTests
{
    private static AppRole ExistingRole() => new() { Id = "role-1", Name = "Admin", Code = "ADMIN" };

    [Fact]
    public async Task Handle_ShouldSoftDeleteRole_WhenExists()
    {
        Mock<IRoleService> roleService = new();
        roleService.Setup(s => s.GetByIdAsync("role-1", It.IsAny<CancellationToken>())).ReturnsAsync(ExistingRole());
        roleService.Setup(s => s.SoftDeleteAsync(It.IsAny<AppRole>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        DeleteRoleCommandHandler handler = new(roleService.Object);
        DeleteRoleCommand command = new() { Id = "role-1" };

        bool result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        roleService.Verify(s => s.SoftDeleteAsync(It.IsAny<AppRole>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenRoleDoesNotExist()
    {
        Mock<IRoleService> roleService = new();
        roleService.Setup(s => s.GetByIdAsync("role-1", It.IsAny<CancellationToken>())).ReturnsAsync((AppRole?)null);

        DeleteRoleCommandHandler handler = new(roleService.Object);
        DeleteRoleCommand command = new() { Id = "role-1" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.Role.NotFound, exception.ErrorCode);
        roleService.Verify(s => s.SoftDeleteAsync(It.IsAny<AppRole>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
