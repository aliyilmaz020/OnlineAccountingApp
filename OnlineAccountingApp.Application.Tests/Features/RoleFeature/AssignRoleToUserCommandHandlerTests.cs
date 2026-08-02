using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.AssignRoleToUser;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Tests.Features.RoleFeature;

public class AssignRoleToUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldAssignRole_WhenRoleAndUserExist()
    {
        Mock<IRoleService> roleService = new();
        roleService.Setup(s => s.GetByCodeAsync("ADMIN", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppRole { Id = "role-1", Name = "Admin", Code = "ADMIN" });
        roleService.Setup(s => s.UserExistsAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        roleService.Setup(s => s.AssignRoleToUserAsync("user-1", "Admin", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        AssignRoleToUserCommandHandler handler = new(roleService.Object);
        AssignRoleToUserCommand command = new() { UserId = "user-1", RoleCode = "ADMIN" };

        bool result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        roleService.Verify(s => s.AssignRoleToUserAsync("user-1", "Admin", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenRoleMissing()
    {
        Mock<IRoleService> roleService = new();
        roleService.Setup(s => s.GetByCodeAsync("MISSING", It.IsAny<CancellationToken>())).ReturnsAsync((AppRole?)null);

        AssignRoleToUserCommandHandler handler = new(roleService.Object);
        AssignRoleToUserCommand command = new() { UserId = "user-1", RoleCode = "MISSING" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.Role.NotFound, exception.ErrorCode);
        roleService.Verify(s => s.AssignRoleToUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserMissing()
    {
        Mock<IRoleService> roleService = new();
        roleService.Setup(s => s.GetByCodeAsync("ADMIN", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppRole { Id = "role-1", Name = "Admin", Code = "ADMIN" });
        roleService.Setup(s => s.UserExistsAsync("missing-user", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        AssignRoleToUserCommandHandler handler = new(roleService.Object);
        AssignRoleToUserCommand command = new() { UserId = "missing-user", RoleCode = "ADMIN" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.Role.NotFound, exception.ErrorCode);
        roleService.Verify(s => s.AssignRoleToUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
