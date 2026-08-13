using Moq;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Delete;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.CompanyEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Tests.Features.UniformChartOfAccountFeature;

public class DeleteUniformChartOfAccountCommandHandlerTests
{
    private static UniformChartOfAccount ExistingAccount() => new() { Id = "account-1", Code = "100", Name = "Cash", Type = "Asset" };

    [Fact]
    public async Task Handle_ShouldSoftDelete_WhenExists()
    {
        (Mock<ICompanyUnitOfWork> unitOfWork, Mock<IRepository<UniformChartOfAccount>> repository) =
            UnitOfWorkMockFactory.Create<UniformChartOfAccount, ICompanyUnitOfWork>();

        repository.Setup(r => r.GetByIdAsync("account-1", It.IsAny<CancellationToken>())).ReturnsAsync(ExistingAccount());
        repository.Setup(r => r.SoftDeleteAsync(It.IsAny<UniformChartOfAccount>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        DeleteUniformChartOfAccountCommandHandler handler = new(unitOfWork.Object);
        DeleteUniformChartOfAccountCommand command = new() { Id = "account-1" };

        bool result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        repository.Verify(r => r.SoftDeleteAsync(It.IsAny<UniformChartOfAccount>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenMissing()
    {
        (Mock<ICompanyUnitOfWork> unitOfWork, Mock<IRepository<UniformChartOfAccount>> repository) =
            UnitOfWorkMockFactory.Create<UniformChartOfAccount, ICompanyUnitOfWork>();

        repository.Setup(r => r.GetByIdAsync("account-1", It.IsAny<CancellationToken>())).ReturnsAsync((UniformChartOfAccount?)null);

        DeleteUniformChartOfAccountCommandHandler handler = new(unitOfWork.Object);
        DeleteUniformChartOfAccountCommand command = new() { Id = "account-1" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.UniformChartOfAccount.NotFound, exception.ErrorCode);
        repository.Verify(r => r.SoftDeleteAsync(It.IsAny<UniformChartOfAccount>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
