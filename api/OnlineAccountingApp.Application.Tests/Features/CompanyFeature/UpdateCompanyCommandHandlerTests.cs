using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Update;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.CompanyFeature;

public class UpdateCompanyCommandHandlerTests
{
    private static Company ExistingCompany() => new()
    {
        Id = "company-1",
        Name = "Old Name",
        Address = "Old Address",
        IdentityNumber = "1234567890",
        TaxDepartment = "Central",
        PhoneNumber = "5551234567",
        Email = "old@example.com",
        ServerName = "localhost",
        DatabaseName = "OldDb",
        ServerUserId = "sa",
        ServerPassword = "password"
    };

    private static UpdateCompanyCommand BuildCommand() => new()
    {
        Id = "company-1",
        Name = "New Name",
        Address = "New Address",
        IdentityNumber = "1234567890",
        TaxDepartment = "Central",
        PhoneNumber = "5559876543",
        Email = "new@example.com",
        ServerName = "localhost",
        DatabaseName = "NewDb",
        ServerUserId = "sa",
        ServerPassword = "password"
    };

    [Fact]
    public async Task Handle_ShouldUpdateCompany_WhenValid()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<Company>> repository) = UnitOfWorkMockFactory.Create<Company>();

        repository.Setup(r => r.GetByIdAsync("company-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingCompany());
        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Company, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repository.Setup(r => r.UpdateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .Returns<Company, CancellationToken>((entity, _) => Task.FromResult(entity));

        UpdateCompanyCommandHandler handler = new(unitOfWork.Object);
        UpdateCompanyCommand command = BuildCommand();

        CompanyListItemDto result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("company-1", result.Id);
        Assert.Equal("New Name", result.Name);
        Assert.Equal("new@example.com", result.Email);

        repository.Verify(r => r.UpdateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenCompanyDoesNotExist()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<Company>> repository) = UnitOfWorkMockFactory.Create<Company>();

        repository.Setup(r => r.GetByIdAsync("company-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Company?)null);

        UpdateCompanyCommandHandler handler = new(unitOfWork.Object);
        UpdateCompanyCommand command = BuildCommand();

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.Company.NotFound, exception.ErrorCode);
        repository.Verify(r => r.UpdateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowConflict_WhenAnotherCompanyHasSameName()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<Company>> repository) = UnitOfWorkMockFactory.Create<Company>();

        repository.Setup(r => r.GetByIdAsync("company-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingCompany());

        Expression<Func<Company, bool>>? capturedPredicate = null;
        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Company, bool>>>(), It.IsAny<CancellationToken>()))
            .Callback<Expression<Func<Company, bool>>, CancellationToken>((predicate, _) => capturedPredicate = predicate)
            .ReturnsAsync(true);

        UpdateCompanyCommandHandler handler = new(unitOfWork.Object);
        UpdateCompanyCommand command = BuildCommand();

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.Company.AlreadyExists, exception.ErrorCode);
        repository.Verify(r => r.UpdateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.NotNull(capturedPredicate);
        Func<Company, bool> compiledPredicate = capturedPredicate!.Compile();
        Assert.True(compiledPredicate(new Company { Id = "company-2", Name = command.Name }));
        Assert.False(compiledPredicate(new Company { Id = "company-1", Name = command.Name }));
        Assert.False(compiledPredicate(new Company { Id = "company-2", Name = "A Completely Different Name" }));
    }
}
