using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoleById;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Tests.Features.RoleFeature;

public class GetRoleByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnRole_WhenExists()
    {
        Mock<IRoleService> roleService = new();
        roleService.Setup(s => s.GetByIdAsync("role-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppRole { Id = "role-1", Name = "Admin", Code = "ADMIN" });

        GetRoleByIdQueryHandler handler = new(roleService.Object);
        GetRoleByIdQuery query = new() { Id = "role-1" };

        RoleListItemDto result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal("role-1", result.Id);
        Assert.Equal("Admin", result.Name);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenMissing()
    {
        Mock<IRoleService> roleService = new();
        roleService.Setup(s => s.GetByIdAsync("missing-id", It.IsAny<CancellationToken>())).ReturnsAsync((AppRole?)null);

        GetRoleByIdQueryHandler handler = new(roleService.Object);
        GetRoleByIdQuery query = new() { Id = "missing-id" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(query, CancellationToken.None));

        Assert.Equal(AppErrorCodes.Role.NotFound, exception.ErrorCode);
    }
}
