using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Update;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Domain.Roles;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.CompanyFeature;

public class UpdateCompanyCommandHandlerTests
{
    private static (Mock<IPermissionService> PermissionService, Mock<ICompanyContext> CompanyContext) MockPermission(bool permitted = true)
    {
        Mock<IPermissionService> permissionService = new();
        Mock<ICompanyContext> companyContext = new();
        companyContext.Setup(c => c.UserId).Returns("user-1");
        permissionService
            .Setup(s => s.HasPermissionAsync("user-1", "company-1", RoleList.CompanyUpdateCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(permitted);
        return (permissionService, companyContext);
    }

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
        (Mock<IPermissionService> permissionService, Mock<ICompanyContext> companyContext) = MockPermission();

        UpdateCompanyCommandHandler handler = new(unitOfWork.Object, permissionService.Object, companyContext.Object);
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
        (Mock<IPermissionService> permissionService, Mock<ICompanyContext> companyContext) = MockPermission();

        UpdateCompanyCommandHandler handler = new(unitOfWork.Object, permissionService.Object, companyContext.Object);
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
        (Mock<IPermissionService> permissionService, Mock<ICompanyContext> companyContext) = MockPermission();

        UpdateCompanyCommandHandler handler = new(unitOfWork.Object, permissionService.Object, companyContext.Object);
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

    [Fact]
    public async Task Handle_ShouldThrowPermissionDenied_WhenUserLacksCompanyUpdatePermission()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<Company>> repository) = UnitOfWorkMockFactory.Create<Company>();

        repository.Setup(r => r.GetByIdAsync("company-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingCompany());
        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Company, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        (Mock<IPermissionService> permissionService, Mock<ICompanyContext> companyContext) = MockPermission(permitted: false);

        UpdateCompanyCommandHandler handler = new(unitOfWork.Object, permissionService.Object, companyContext.Object);
        UpdateCompanyCommand command = BuildCommand();

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.Company.PermissionDenied, exception.ErrorCode);
        repository.Verify(r => r.UpdateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUpdateCompany_WhenCallerIsSystemAdmin_EvenWithoutCompanyUpdatePermission()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<Company>> repository) = UnitOfWorkMockFactory.Create<Company>();

        repository.Setup(r => r.GetByIdAsync("company-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingCompany());
        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Company, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repository.Setup(r => r.UpdateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .Returns<Company, CancellationToken>((entity, _) => Task.FromResult(entity));
        (Mock<IPermissionService> permissionService, Mock<ICompanyContext> companyContext) = MockPermission(permitted: false);
        companyContext.Setup(c => c.IsInRole(RoleList.SystemAdmin)).Returns(true);

        UpdateCompanyCommandHandler handler = new(unitOfWork.Object, permissionService.Object, companyContext.Object);
        UpdateCompanyCommand command = BuildCommand();

        CompanyListItemDto result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("New Name", result.Name);
        repository.Verify(r => r.UpdateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotChangeDbFields_WhenCallerIsNotSystemAdmin()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<Company>> repository) = UnitOfWorkMockFactory.Create<Company>();

        repository.Setup(r => r.GetByIdAsync("company-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingCompany());
        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Company, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        Company? capturedEntity = null;
        repository.Setup(r => r.UpdateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .Callback<Company, CancellationToken>((entity, _) => capturedEntity = entity)
            .Returns<Company, CancellationToken>((entity, _) => Task.FromResult(entity));
        (Mock<IPermissionService> permissionService, Mock<ICompanyContext> companyContext) = MockPermission();

        UpdateCompanyCommandHandler handler = new(unitOfWork.Object, permissionService.Object, companyContext.Object);
        UpdateCompanyCommand command = BuildCommand();

        await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(capturedEntity);
        Assert.Equal("New Name", capturedEntity!.Name);
        Assert.Equal("localhost", capturedEntity.ServerName);
        Assert.Equal("OldDb", capturedEntity.DatabaseName);
        Assert.Equal("sa", capturedEntity.ServerUserId);
        Assert.Equal("password", capturedEntity.ServerPassword);
    }

    [Fact]
    public async Task Handle_ShouldChangeDbFields_WhenCallerIsSystemAdmin()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<Company>> repository) = UnitOfWorkMockFactory.Create<Company>();

        repository.Setup(r => r.GetByIdAsync("company-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingCompany());
        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Company, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        Company? capturedEntity = null;
        repository.Setup(r => r.UpdateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .Callback<Company, CancellationToken>((entity, _) => capturedEntity = entity)
            .Returns<Company, CancellationToken>((entity, _) => Task.FromResult(entity));
        (Mock<IPermissionService> permissionService, Mock<ICompanyContext> companyContext) = MockPermission();
        companyContext.Setup(c => c.IsInRole(RoleList.SystemAdmin)).Returns(true);

        UpdateCompanyCommandHandler handler = new(unitOfWork.Object, permissionService.Object, companyContext.Object);
        UpdateCompanyCommand command = BuildCommand();

        await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(capturedEntity);
        Assert.Equal("NewDb", capturedEntity!.DatabaseName);
    }
}
