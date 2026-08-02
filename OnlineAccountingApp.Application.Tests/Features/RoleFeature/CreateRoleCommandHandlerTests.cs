using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.Create;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Tests.Features.RoleFeature;

public class CreateRoleCommandHandlerTests
{
    private static CreateRoleCommand BuildCommand() => new() { Name = "Admin", Code = "ADMIN" };

    [Fact]
    public async Task Handle_ShouldCreateRole_WhenNameAndCodeAreUnique()
    {
        Mock<IRoleService> roleService = new();
        roleService.Setup(s => s.GetByNameAsync("Admin", It.IsAny<CancellationToken>())).ReturnsAsync((AppRole?)null);
        roleService.Setup(s => s.GetByCodeAsync("ADMIN", It.IsAny<CancellationToken>())).ReturnsAsync((AppRole?)null);
        roleService.Setup(s => s.CreateAsync(It.IsAny<AppRole>(), It.IsAny<CancellationToken>()))
            .Returns<AppRole, CancellationToken>((role, _) =>
            {
                role.Id = "generated-id";
                return Task.FromResult(role);
            });

        CreateRoleCommandHandler handler = new(roleService.Object);
        CreateRoleCommand command = BuildCommand();

        RoleListItemDto result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("generated-id", result.Id);
        Assert.Equal("Admin", result.Name);
        Assert.Equal("ADMIN", result.Code);
        roleService.Verify(s => s.CreateAsync(It.IsAny<AppRole>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowAlreadyExists_WhenNameExists()
    {
        Mock<IRoleService> roleService = new();
        roleService.Setup(s => s.GetByNameAsync("Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppRole { Id = "existing-id", Name = "Admin", Code = "ADMIN" });

        CreateRoleCommandHandler handler = new(roleService.Object);
        CreateRoleCommand command = BuildCommand();

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.Role.AlreadyExists, exception.ErrorCode);
        roleService.Verify(s => s.CreateAsync(It.IsAny<AppRole>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowAlreadyExists_WhenCodeExists()
    {
        Mock<IRoleService> roleService = new();
        roleService.Setup(s => s.GetByNameAsync("Admin", It.IsAny<CancellationToken>())).ReturnsAsync((AppRole?)null);
        roleService.Setup(s => s.GetByCodeAsync("ADMIN", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppRole { Id = "existing-id", Name = "Other", Code = "ADMIN" });

        CreateRoleCommandHandler handler = new(roleService.Object);
        CreateRoleCommand command = BuildCommand();

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.Role.AlreadyExists, exception.ErrorCode);
        roleService.Verify(s => s.CreateAsync(It.IsAny<AppRole>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
