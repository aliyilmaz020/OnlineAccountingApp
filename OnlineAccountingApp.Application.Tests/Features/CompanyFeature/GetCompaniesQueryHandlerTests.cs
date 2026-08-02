using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.CompanyFeature;

public class GetCompaniesQueryHandlerTests
{
    private static Company BuildCompany(string id, string name) => new()
    {
        Id = id,
        Name = name,
        Address = "Address",
        IdentityNumber = "1234567890",
        TaxDepartment = "Central",
        PhoneNumber = "5551234567",
        Email = $"{name}@example.com"
    };

    [Fact]
    public async Task Handle_ShouldReturnPagedResult_WhenNoSearchTerm()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<Company>> repository) = UnitOfWorkMockFactory.Create<Company>();

        Expression<Func<Company, bool>>? capturedPredicate = null;
        PagedResult<Company> pagedCompanies = new()
        {
            Items = [BuildCompany("1", "Acme"), BuildCompany("2", "Globex")],
            TotalCount = 2,
            PageNumber = 1,
            PageSize = 20
        };

        repository.Setup(r => r.GetPagedAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Expression<Func<Company, bool>>?>(),
                It.IsAny<Expression<Func<Company, object>>[]?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Callback<int, int, Expression<Func<Company, bool>>?, Expression<Func<Company, object>>[]?, bool, CancellationToken>(
                (_, _, predicate, _, _, _) => capturedPredicate = predicate)
            .ReturnsAsync(pagedCompanies);

        GetCompaniesQueryHandler handler = new(unitOfWork.Object);
        GetCompaniesQuery query = new() { PageNumber = 1, PageSize = 20 };

        PagedResult<CompanyListItemDto> result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(capturedPredicate);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(20, result.PageSize);
        Assert.Equal(2, result.Items.Count);
        Assert.Contains(result.Items, i => i.Name == "Acme");
        Assert.Contains(result.Items, i => i.Name == "Globex");
    }

    [Fact]
    public async Task Handle_ShouldFilterBySearchTerm_WhenProvided()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<Company>> repository) = UnitOfWorkMockFactory.Create<Company>();

        Expression<Func<Company, bool>>? capturedPredicate = null;

        repository.Setup(r => r.GetPagedAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Expression<Func<Company, bool>>?>(),
                It.IsAny<Expression<Func<Company, object>>[]?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Callback<int, int, Expression<Func<Company, bool>>?, Expression<Func<Company, object>>[]?, bool, CancellationToken>(
                (_, _, predicate, _, _, _) => capturedPredicate = predicate)
            .ReturnsAsync(new PagedResult<Company> { Items = [], TotalCount = 0, PageNumber = 1, PageSize = 20 });

        GetCompaniesQueryHandler handler = new(unitOfWork.Object);
        GetCompaniesQuery query = new() { PageNumber = 1, PageSize = 20, SearchTerm = "Acme" };

        await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(capturedPredicate);
        Func<Company, bool> compiledPredicate = capturedPredicate!.Compile();
        Assert.True(compiledPredicate(BuildCompany("1", "Acme Holdings")));
        Assert.False(compiledPredicate(BuildCompany("2", "Globex")));
    }
}
