using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.RemoveRoleFromUser;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Tests.Features.RoleFeature;

public class RemoveRoleFromUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldRemoveRole_WhenRoleAndUserExist()
    {
        Mock<IRoleService> roleService = new();
        roleService.Setup(s => s.GetByNameAsync("Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppRole { Id = "role-1", Name = "Admin", Code = "ADMIN" });
        roleService.Setup(s => s.UserExistsAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        roleService.Setup(s => s.RemoveRoleFromUserAsync("user-1", "Admin", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        RemoveRoleFromUserCommandHandler handler = new(roleService.Object);
        RemoveRoleFromUserCommand command = new() { UserId = "user-1", RoleName = "Admin" };

        bool result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        roleService.Verify(s => s.RemoveRoleFromUserAsync("user-1", "Admin", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenRoleMissing()
    {
        Mock<IRoleService> roleService = new();
        roleService.Setup(s => s.GetByNameAsync("Missing", It.IsAny<CancellationToken>())).ReturnsAsync((AppRole?)null);

        RemoveRoleFromUserCommandHandler handler = new(roleService.Object);
        RemoveRoleFromUserCommand command = new() { UserId = "user-1", RoleName = "Missing" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.Role.NotFound, exception.ErrorCode);
        roleService.Verify(s => s.RemoveRoleFromUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserMissing()
    {
        Mock<IRoleService> roleService = new();
        roleService.Setup(s => s.GetByNameAsync("Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppRole { Id = "role-1", Name = "Admin", Code = "ADMIN" });
        roleService.Setup(s => s.UserExistsAsync("missing-user", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        RemoveRoleFromUserCommandHandler handler = new(roleService.Object);
        RemoveRoleFromUserCommand command = new() { UserId = "missing-user", RoleName = "Admin" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.Role.NotFound, exception.ErrorCode);
        roleService.Verify(s => s.RemoveRoleFromUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
