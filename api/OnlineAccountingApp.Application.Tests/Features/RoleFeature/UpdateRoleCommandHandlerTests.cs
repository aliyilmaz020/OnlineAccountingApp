using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.Update;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Tests.Features.RoleFeature;

public class UpdateRoleCommandHandlerTests
{
    private static AppRole ExistingRole() => new() { Id = "role-1", Name = "Old Name", Code = "OLD" };

    private static UpdateRoleCommand BuildCommand() => new() { Id = "role-1", Name = "New Name", Code = "NEW", Status = true };

    [Fact]
    public async Task Handle_ShouldUpdateRole_WhenValid()
    {
        Mock<IRoleService> roleService = new();
        roleService.Setup(s => s.GetByIdAsync("role-1", It.IsAny<CancellationToken>())).ReturnsAsync(ExistingRole());
        roleService.Setup(s => s.GetByNameAsync("New Name", It.IsAny<CancellationToken>())).ReturnsAsync((AppRole?)null);
        roleService.Setup(s => s.GetByCodeAsync("NEW", It.IsAny<CancellationToken>())).ReturnsAsync((AppRole?)null);
        roleService.Setup(s => s.UpdateAsync(It.IsAny<AppRole>(), It.IsAny<CancellationToken>()))
            .Returns<AppRole, CancellationToken>((role, _) => Task.FromResult(role));

        UpdateRoleCommandHandler handler = new(roleService.Object);
        UpdateRoleCommand command = BuildCommand();

        RoleListItemDto result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("role-1", result.Id);
        Assert.Equal("New Name", result.Name);
        Assert.Equal("NEW", result.Code);
        roleService.Verify(s => s.UpdateAsync(It.IsAny<AppRole>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenRoleDoesNotExist()
    {
        Mock<IRoleService> roleService = new();
        roleService.Setup(s => s.GetByIdAsync("role-1", It.IsAny<CancellationToken>())).ReturnsAsync((AppRole?)null);

        UpdateRoleCommandHandler handler = new(roleService.Object);
        UpdateRoleCommand command = BuildCommand();

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.Role.NotFound, exception.ErrorCode);
        roleService.Verify(s => s.UpdateAsync(It.IsAny<AppRole>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowAlreadyExists_WhenAnotherRoleHasSameNameOrCode()
    {
        Mock<IRoleService> roleService = new();
        roleService.Setup(s => s.GetByIdAsync("role-1", It.IsAny<CancellationToken>())).ReturnsAsync(ExistingRole());
        roleService.Setup(s => s.GetByNameAsync("New Name", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppRole { Id = "role-2", Name = "New Name", Code = "OTHER" });

        UpdateRoleCommandHandler handler = new(roleService.Object);
        UpdateRoleCommand command = BuildCommand();

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.Role.AlreadyExists, exception.ErrorCode);
        roleService.Verify(s => s.UpdateAsync(It.IsAny<AppRole>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
