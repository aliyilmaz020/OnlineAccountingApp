using Moq;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetById;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.CompanyEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.UniformChartOfAccountFeature;

public class GetUniformChartOfAccountByIdQueryHandlerTests
{
    private static UniformChartOfAccount ExistingAccount() => new() { Id = "account-1", Code = "100", Name = "Cash", Type = "Asset" };

    [Fact]
    public async Task Handle_ShouldReturn_WhenExists()
    {
        (Mock<ICompanyUnitOfWork> unitOfWork, Mock<IRepository<UniformChartOfAccount>> repository) =
            UnitOfWorkMockFactory.Create<UniformChartOfAccount, ICompanyUnitOfWork>();

        Expression<Func<UniformChartOfAccount, bool>>? capturedPredicate = null;
        repository.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<UniformChartOfAccount, bool>>>(),
                It.IsAny<Expression<Func<UniformChartOfAccount, object>>[]?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Callback<Expression<Func<UniformChartOfAccount, bool>>, Expression<Func<UniformChartOfAccount, object>>[]?, bool, CancellationToken>(
                (predicate, _, _, _) => capturedPredicate = predicate)
            .ReturnsAsync(ExistingAccount());

        GetUniformChartOfAccountByIdQueryHandler handler = new(unitOfWork.Object);
        GetUniformChartOfAccountByIdQuery query = new() { Id = "account-1" };

        UniformChartOfAccountListItemDto result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal("account-1", result.Id);
        Assert.Equal("100", result.Code);
        Assert.Equal("Cash", result.Name);

        Assert.NotNull(capturedPredicate);
        Func<UniformChartOfAccount, bool> compiledPredicate = capturedPredicate!.Compile();
        Assert.True(compiledPredicate(new UniformChartOfAccount { Id = "account-1", Deleted = false }));
        Assert.False(compiledPredicate(new UniformChartOfAccount { Id = "some-other-id", Deleted = false }));
        Assert.False(compiledPredicate(new UniformChartOfAccount { Id = "account-1", Deleted = true }));
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenMissing()
    {
        (Mock<ICompanyUnitOfWork> unitOfWork, Mock<IRepository<UniformChartOfAccount>> repository) =
            UnitOfWorkMockFactory.Create<UniformChartOfAccount, ICompanyUnitOfWork>();

        repository.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<UniformChartOfAccount, bool>>>(),
                It.IsAny<Expression<Func<UniformChartOfAccount, object>>[]?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((UniformChartOfAccount?)null);

        GetUniformChartOfAccountByIdQueryHandler handler = new(unitOfWork.Object);
        GetUniformChartOfAccountByIdQuery query = new() { Id = "missing-id" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(query, CancellationToken.None));

        Assert.Equal(AppErrorCodes.UniformChartOfAccount.NotFound, exception.ErrorCode);
    }
}
