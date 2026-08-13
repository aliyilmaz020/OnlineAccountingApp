using Moq;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.Create;
using OnlineAccountingApp.Application.Features.CompanyFeatures.UniformChartOfAccountFeature.GetList;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.CompanyEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.UniformChartOfAccountFeature;

public class CreateUniformChartOfAccountCommandHandlerTests
{
    private static CreateUniformChartOfAccountCommand BuildCommand() => new() { Code = "100", Name = "Cash", Type = "Asset" };

    [Fact]
    public async Task Handle_ShouldCreate_WhenCodeIsUnique()
    {
        (Mock<ICompanyUnitOfWork> unitOfWork, Mock<IRepository<UniformChartOfAccount>> repository) =
            UnitOfWorkMockFactory.Create<UniformChartOfAccount, ICompanyUnitOfWork>();

        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<UniformChartOfAccount, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repository.Setup(r => r.CreateAsync(It.IsAny<UniformChartOfAccount>(), It.IsAny<CancellationToken>()))
            .Returns<UniformChartOfAccount, CancellationToken>((entity, _) =>
            {
                entity.Id = "generated-id";
                return Task.FromResult(entity);
            });

        CreateUniformChartOfAccountCommandHandler handler = new(unitOfWork.Object);
        CreateUniformChartOfAccountCommand command = BuildCommand();

        UniformChartOfAccountListItemDto result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("generated-id", result.Id);
        Assert.Equal("100", result.Code);
        Assert.Equal("Cash", result.Name);

        repository.Verify(r => r.CreateAsync(It.IsAny<UniformChartOfAccount>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowAlreadyExists_WhenCodeExists()
    {
        (Mock<ICompanyUnitOfWork> unitOfWork, Mock<IRepository<UniformChartOfAccount>> repository) =
            UnitOfWorkMockFactory.Create<UniformChartOfAccount, ICompanyUnitOfWork>();

        Expression<Func<UniformChartOfAccount, bool>>? capturedPredicate = null;
        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<UniformChartOfAccount, bool>>>(), It.IsAny<CancellationToken>()))
            .Callback<Expression<Func<UniformChartOfAccount, bool>>, CancellationToken>((predicate, _) => capturedPredicate = predicate)
            .ReturnsAsync(true);

        CreateUniformChartOfAccountCommandHandler handler = new(unitOfWork.Object);
        CreateUniformChartOfAccountCommand command = BuildCommand();

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.UniformChartOfAccount.AlreadyExists, exception.ErrorCode);
        repository.Verify(r => r.CreateAsync(It.IsAny<UniformChartOfAccount>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.NotNull(capturedPredicate);
        Func<UniformChartOfAccount, bool> compiledPredicate = capturedPredicate!.Compile();
        Assert.True(compiledPredicate(new UniformChartOfAccount { Code = command.Code, Deleted = false }));
        Assert.False(compiledPredicate(new UniformChartOfAccount { Code = "DIFFERENT-CODE", Deleted = false }));
        Assert.False(compiledPredicate(new UniformChartOfAccount { Code = command.Code, Deleted = true }));
    }
}
