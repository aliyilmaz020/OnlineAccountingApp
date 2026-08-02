using Moq;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Update;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.CompanyEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.UniformChartOfAccountFeature;

public class UpdateUniformChartOfAccountCommandHandlerTests
{
    private static UniformChartOfAccount ExistingAccount() => new() { Id = "account-1", Code = "100", Name = "Old Name", Type = "Asset" };

    private static UpdateUniformChartOfAccountCommand BuildCommand() => new() { Id = "account-1", Code = "200", Name = "New Name", Type = "Liability" };

    [Fact]
    public async Task Handle_ShouldUpdate_WhenValid()
    {
        (Mock<ICompanyUnitOfWork> unitOfWork, Mock<IRepository<UniformChartOfAccount>> repository) =
            UnitOfWorkMockFactory.Create<UniformChartOfAccount, ICompanyUnitOfWork>();

        repository.Setup(r => r.GetByIdAsync("account-1", It.IsAny<CancellationToken>())).ReturnsAsync(ExistingAccount());
        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<UniformChartOfAccount, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repository.Setup(r => r.UpdateAsync(It.IsAny<UniformChartOfAccount>(), It.IsAny<CancellationToken>()))
            .Returns<UniformChartOfAccount, CancellationToken>((entity, _) => Task.FromResult(entity));

        UpdateUniformChartOfAccountCommandHandler handler = new(unitOfWork.Object);
        UpdateUniformChartOfAccountCommand command = BuildCommand();

        UniformChartOfAccountListItemDto result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("account-1", result.Id);
        Assert.Equal("200", result.Code);
        Assert.Equal("New Name", result.Name);

        repository.Verify(r => r.UpdateAsync(It.IsAny<UniformChartOfAccount>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenMissing()
    {
        (Mock<ICompanyUnitOfWork> unitOfWork, Mock<IRepository<UniformChartOfAccount>> repository) =
            UnitOfWorkMockFactory.Create<UniformChartOfAccount, ICompanyUnitOfWork>();

        repository.Setup(r => r.GetByIdAsync("account-1", It.IsAny<CancellationToken>())).ReturnsAsync((UniformChartOfAccount?)null);

        UpdateUniformChartOfAccountCommandHandler handler = new(unitOfWork.Object);
        UpdateUniformChartOfAccountCommand command = BuildCommand();

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.UniformChartOfAccount.NotFound, exception.ErrorCode);
        repository.Verify(r => r.UpdateAsync(It.IsAny<UniformChartOfAccount>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowAlreadyExists_WhenAnotherRecordHasSameCode()
    {
        (Mock<ICompanyUnitOfWork> unitOfWork, Mock<IRepository<UniformChartOfAccount>> repository) =
            UnitOfWorkMockFactory.Create<UniformChartOfAccount, ICompanyUnitOfWork>();

        repository.Setup(r => r.GetByIdAsync("account-1", It.IsAny<CancellationToken>())).ReturnsAsync(ExistingAccount());
        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<UniformChartOfAccount, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        UpdateUniformChartOfAccountCommandHandler handler = new(unitOfWork.Object);
        UpdateUniformChartOfAccountCommand command = BuildCommand();

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.UniformChartOfAccount.AlreadyExists, exception.ErrorCode);
        repository.Verify(r => r.UpdateAsync(It.IsAny<UniformChartOfAccount>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
