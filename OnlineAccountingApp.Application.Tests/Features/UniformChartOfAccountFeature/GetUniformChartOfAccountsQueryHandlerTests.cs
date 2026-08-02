using Moq;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.CompanyEntities;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.UniformChartOfAccountFeature;

public class GetUniformChartOfAccountsQueryHandlerTests
{
    private static UniformChartOfAccount BuildAccount(string id, string code, string name) => new()
    {
        Id = id,
        Code = code,
        Name = name,
        Type = "Asset"
    };

    [Fact]
    public async Task Handle_ShouldReturnPagedResult_WhenNoSearchTerm()
    {
        (Mock<ICompanyUnitOfWork> unitOfWork, Mock<IRepository<UniformChartOfAccount>> repository) =
            UnitOfWorkMockFactory.Create<UniformChartOfAccount, ICompanyUnitOfWork>();

        Expression<Func<UniformChartOfAccount, bool>>? capturedPredicate = null;
        PagedResult<UniformChartOfAccount> pagedAccounts = new()
        {
            Items = [BuildAccount("1", "100", "Cash"), BuildAccount("2", "200", "Bank")],
            TotalCount = 2,
            PageNumber = 1,
            PageSize = 20
        };

        repository.Setup(r => r.GetPagedAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Expression<Func<UniformChartOfAccount, bool>>?>(),
                It.IsAny<Expression<Func<UniformChartOfAccount, object>>[]?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Callback<int, int, Expression<Func<UniformChartOfAccount, bool>>?, Expression<Func<UniformChartOfAccount, object>>[]?, bool, CancellationToken>(
                (_, _, predicate, _, _, _) => capturedPredicate = predicate)
            .ReturnsAsync(pagedAccounts);

        GetUniformChartOfAccountsQueryHandler handler = new(unitOfWork.Object);
        GetUniformChartOfAccountsQuery query = new() { PageNumber = 1, PageSize = 20 };

        PagedResult<UniformChartOfAccountListItemDto> result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(capturedPredicate);
        Func<UniformChartOfAccount, bool> compiledPredicate = capturedPredicate!.Compile();
        Assert.True(compiledPredicate(BuildAccount("1", "100", "Cash")));

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(20, result.PageSize);
        Assert.Contains(result.Items, i => i.Name == "Cash");
        Assert.Contains(result.Items, i => i.Name == "Bank");
    }

    [Fact]
    public async Task Handle_ShouldFilterBySearchTerm_WhenProvided()
    {
        (Mock<ICompanyUnitOfWork> unitOfWork, Mock<IRepository<UniformChartOfAccount>> repository) =
            UnitOfWorkMockFactory.Create<UniformChartOfAccount, ICompanyUnitOfWork>();

        Expression<Func<UniformChartOfAccount, bool>>? capturedPredicate = null;

        repository.Setup(r => r.GetPagedAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Expression<Func<UniformChartOfAccount, bool>>?>(),
                It.IsAny<Expression<Func<UniformChartOfAccount, object>>[]?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Callback<int, int, Expression<Func<UniformChartOfAccount, bool>>?, Expression<Func<UniformChartOfAccount, object>>[]?, bool, CancellationToken>(
                (_, _, predicate, _, _, _) => capturedPredicate = predicate)
            .ReturnsAsync(new PagedResult<UniformChartOfAccount> { Items = [], TotalCount = 0, PageNumber = 1, PageSize = 20 });

        GetUniformChartOfAccountsQueryHandler handler = new(unitOfWork.Object);
        GetUniformChartOfAccountsQuery query = new() { PageNumber = 1, PageSize = 20, SearchTerm = "Cash" };

        await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(capturedPredicate);
        Func<UniformChartOfAccount, bool> compiledPredicate = capturedPredicate!.Compile();
        Assert.True(compiledPredicate(BuildAccount("1", "100", "Cash")));
        Assert.False(compiledPredicate(BuildAccount("2", "200", "Bank")));
    }
}
