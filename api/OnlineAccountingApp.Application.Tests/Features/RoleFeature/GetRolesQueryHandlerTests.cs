using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.RoleFeature.GetRoles;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Tests.Features.RoleFeature;

public class GetRolesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnPagedResult_WhenNoSearchTerm()
    {
        Mock<IRoleService> roleService = new();
        PagedResult<AppRole> pagedRoles = new()
        {
            Items = [new AppRole { Id = "1", Name = "Admin", Code = "ADMIN" }, new AppRole { Id = "2", Name = "Viewer", Code = "VIEWER" }],
            TotalCount = 2,
            PageNumber = 1,
            PageSize = 20
        };

        roleService.Setup(s => s.GetPagedAsync(1, 20, null, It.IsAny<CancellationToken>())).ReturnsAsync(pagedRoles);

        GetRolesQueryHandler handler = new(roleService.Object);
        GetRolesQuery query = new() { PageNumber = 1, PageSize = 20 };

        PagedResult<RoleListItemDto> result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(20, result.PageSize);
        Assert.Contains(result.Items, i => i.Name == "Admin");
        Assert.Contains(result.Items, i => i.Name == "Viewer");
        roleService.Verify(s => s.GetPagedAsync(1, 20, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPassSearchTermThrough_WhenProvided()
    {
        Mock<IRoleService> roleService = new();
        roleService.Setup(s => s.GetPagedAsync(1, 20, "Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<AppRole> { Items = [], TotalCount = 0, PageNumber = 1, PageSize = 20 });

        GetRolesQueryHandler handler = new(roleService.Object);
        GetRolesQuery query = new() { PageNumber = 1, PageSize = 20, SearchTerm = "Admin" };

        await handler.Handle(query, CancellationToken.None);

        roleService.Verify(s => s.GetPagedAsync(1, 20, "Admin", It.IsAny<CancellationToken>()), Times.Once);
    }
}
