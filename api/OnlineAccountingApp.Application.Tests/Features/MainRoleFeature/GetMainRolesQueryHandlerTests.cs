using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.GetMainRoles;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.MainRoleFeature;

public class GetMainRolesQueryHandlerTests
{
    private static MainRole BuildMainRole(string id, string title) => new()
    {
        Id = id,
        Title = title,
        IsRoleCreateByAdmin = false,
        CompanyId = "company-1"
    };

    [Fact]
    public async Task Handle_ShouldReturnPagedResult_WhenNoSearchTerm()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRole>> repository) = UnitOfWorkMockFactory.Create<MainRole>();

        Expression<Func<MainRole, bool>>? capturedPredicate = null;
        PagedResult<MainRole> pagedMainRoles = new()
        {
            Items = [BuildMainRole("1", "Muhasebeci"), BuildMainRole("2", "Denetci")],
            TotalCount = 2,
            PageNumber = 1,
            PageSize = 20
        };

        repository.Setup(r => r.GetPagedAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Expression<Func<MainRole, bool>>?>(),
                It.IsAny<Expression<Func<MainRole, object>>[]?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Callback<int, int, Expression<Func<MainRole, bool>>?, Expression<Func<MainRole, object>>[]?, bool, CancellationToken>(
                (_, _, predicate, _, _, _) => capturedPredicate = predicate)
            .ReturnsAsync(pagedMainRoles);

        GetMainRolesQueryHandler handler = new(unitOfWork.Object);
        GetMainRolesQuery query = new() { PageNumber = 1, PageSize = 20 };

        PagedResult<MainRoleListItemDto> result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(capturedPredicate);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(20, result.PageSize);
        Assert.Equal(2, result.Items.Count);
        Assert.Contains(result.Items, i => i.Title == "Muhasebeci");
        Assert.Contains(result.Items, i => i.Title == "Denetci");
    }

    [Fact]
    public async Task Handle_ShouldFilterBySearchTerm_WhenProvided()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRole>> repository) = UnitOfWorkMockFactory.Create<MainRole>();

        Expression<Func<MainRole, bool>>? capturedPredicate = null;

        repository.Setup(r => r.GetPagedAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Expression<Func<MainRole, bool>>?>(),
                It.IsAny<Expression<Func<MainRole, object>>[]?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Callback<int, int, Expression<Func<MainRole, bool>>?, Expression<Func<MainRole, object>>[]?, bool, CancellationToken>(
                (_, _, predicate, _, _, _) => capturedPredicate = predicate)
            .ReturnsAsync(new PagedResult<MainRole> { Items = [], TotalCount = 0, PageNumber = 1, PageSize = 20 });

        GetMainRolesQueryHandler handler = new(unitOfWork.Object);
        GetMainRolesQuery query = new() { PageNumber = 1, PageSize = 20, SearchTerm = "Muhasebe" };

        await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(capturedPredicate);
        Func<MainRole, bool> compiledPredicate = capturedPredicate!.Compile();
        Assert.True(compiledPredicate(BuildMainRole("1", "Muhasebeci")));
        Assert.False(compiledPredicate(BuildMainRole("2", "Denetci")));
    }
}
