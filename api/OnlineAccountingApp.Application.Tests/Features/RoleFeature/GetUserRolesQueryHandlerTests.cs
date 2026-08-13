using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetUserRoles;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Tests.Features.RoleFeature;

public class GetUserRolesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnUserRoles_WhenUserExists()
    {
        Mock<IRoleService> roleService = new();
        roleService.Setup(s => s.UserExistsAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        roleService.Setup(s => s.GetRolesByUserIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((IList<AppRole>)[new AppRole { Id = "1", Name = "Admin", Code = "ADMIN" }]);

        GetUserRolesQueryHandler handler = new(roleService.Object);
        GetUserRolesQuery query = new() { UserId = "user-1" };

        List<RoleListItemDto> result = await handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Admin", result[0].Name);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserDoesNotExist()
    {
        Mock<IRoleService> roleService = new();
        roleService.Setup(s => s.UserExistsAsync("missing-user", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        GetUserRolesQueryHandler handler = new(roleService.Object);
        GetUserRolesQuery query = new() { UserId = "missing-user" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(query, CancellationToken.None));

        Assert.Equal(AppErrorCodes.Role.NotFound, exception.ErrorCode);
        roleService.Verify(s => s.GetRolesByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
