using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.UserFeature.GetUsers;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Domain.Roles;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Tests.Features.UserFeature;

public class GetUsersQueryHandlerTests
{
    private static Mock<ICompanyContext> BuildCompanyContext(bool isAdmin, string? companyId = "company-1")
    {
        Mock<ICompanyContext> companyContext = new();
        companyContext.Setup(c => c.IsInRole(RoleList.SystemAdmin)).Returns(isAdmin);
        companyContext.Setup(c => c.CompanyId).Returns(companyId);
        return companyContext;
    }

    [Fact]
    public async Task Handle_ShouldListEveryUser_WhenCallerIsSystemAdmin()
    {
        Mock<IUserService> userService = new();
        PagedResult<AppUser> pagedUsers = new()
        {
            Items = [new AppUser { Id = "1", UserName = "admin", Email = "admin@test.com" }, new AppUser { Id = "2", UserName = "viewer", Email = "viewer@test.com" }],
            TotalCount = 2,
            PageNumber = 1,
            PageSize = 20
        };
        userService.Setup(s => s.GetPagedAsync(1, 20, null, null, It.IsAny<CancellationToken>())).ReturnsAsync(pagedUsers);

        GetUsersQueryHandler handler = new(userService.Object, BuildCompanyContext(isAdmin: true).Object);
        GetUsersQuery query = new() { PageNumber = 1, PageSize = 20 };

        PagedResult<UserListItemDto> result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, result.TotalCount);
        Assert.Contains(result.Items, i => i.UserName == "admin");
        Assert.Contains(result.Items, i => i.UserName == "viewer");
        userService.Verify(s => s.GetPagedAsync(1, 20, null, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPassSearchTermThrough_WhenCallerIsSystemAdmin()
    {
        Mock<IUserService> userService = new();
        userService.Setup(s => s.GetPagedAsync(1, 20, "admin", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<AppUser> { Items = [], TotalCount = 0, PageNumber = 1, PageSize = 20 });

        GetUsersQueryHandler handler = new(userService.Object, BuildCompanyContext(isAdmin: true).Object);
        GetUsersQuery query = new() { PageNumber = 1, PageSize = 20, SearchTerm = "admin" };

        await handler.Handle(query, CancellationToken.None);

        userService.Verify(s => s.GetPagedAsync(1, 20, "admin", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldScopeToSelectedCompany_WhenCallerIsNotSystemAdmin()
    {
        Mock<IUserService> userService = new();
        userService.Setup(s => s.GetPagedAsync(1, 20, null, "company-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<AppUser>
            {
                Items = [new AppUser { Id = "1", UserName = "muhasebeci", Email = "muhasebeci@test.com" }],
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 20
            });

        GetUsersQueryHandler handler = new(userService.Object, BuildCompanyContext(isAdmin: false, companyId: "company-1").Object);
        GetUsersQuery query = new() { PageNumber = 1, PageSize = 20 };

        PagedResult<UserListItemDto> result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(1, result.TotalCount);
        userService.Verify(s => s.GetPagedAsync(1, 20, null, "company-1", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenCallerIsNotSystemAdminAndNoCompanySelected()
    {
        Mock<IUserService> userService = new();
        GetUsersQueryHandler handler = new(userService.Object, BuildCompanyContext(isAdmin: false, companyId: null).Object);
        GetUsersQuery query = new() { PageNumber = 1, PageSize = 20 };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(query, CancellationToken.None));

        Assert.Equal(AppErrorCodes.Tenant.CompanyNotSpecified, exception.ErrorCode);
        userService.Verify(s => s.GetPagedAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
