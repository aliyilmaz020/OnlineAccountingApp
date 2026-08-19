using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Delete;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Application.Services.CompanyServices;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Domain.Roles;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Tests.Features.CompanyFeature;

public class DeleteCompanyCommandHandlerTests
{
    private static (Mock<IPermissionService> PermissionService, Mock<ICompanyContext> CompanyContext) MockPermission(bool permitted = true)
    {
        Mock<IPermissionService> permissionService = new();
        Mock<ICompanyContext> companyContext = new();
        companyContext.Setup(c => c.UserId).Returns("user-1");
        permissionService
            .Setup(s => s.HasPermissionAsync("user-1", "company-1", RoleList.CompanyDeleteCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(permitted);
        return (permissionService, companyContext);
    }

    private static Company ExistingCompany() => new()
    {
        Id = "company-1",
        Name = "Acme Corp",
        Address = "Main Street 1",
        IdentityNumber = "1234567890",
        TaxDepartment = "Central",
        PhoneNumber = "5551234567",
        Email = "acme@example.com",
        ServerName = "localhost",
        DatabaseName = "AcmeDb",
        ServerUserId = "sa",
        ServerPassword = "password"
    };

    [Fact]
    public async Task Handle_ShouldSoftDeleteCompany_WhenExists()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<Company>> repository) = UnitOfWorkMockFactory.Create<Company>();

        repository.Setup(r => r.GetByIdAsync("company-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingCompany());
        repository.Setup(r => r.SoftDeleteAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        (Mock<IPermissionService> permissionService, Mock<ICompanyContext> companyContext) = MockPermission();

        DeleteCompanyCommandHandler handler = new(unitOfWork.Object, permissionService.Object, companyContext.Object);
        DeleteCompanyCommand command = new() { Id = "company-1" };

        bool result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        repository.Verify(r => r.SoftDeleteAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenCompanyDoesNotExist()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<Company>> repository) = UnitOfWorkMockFactory.Create<Company>();

        repository.Setup(r => r.GetByIdAsync("company-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Company?)null);
        (Mock<IPermissionService> permissionService, Mock<ICompanyContext> companyContext) = MockPermission();

        DeleteCompanyCommandHandler handler = new(unitOfWork.Object, permissionService.Object, companyContext.Object);
        DeleteCompanyCommand command = new() { Id = "company-1" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.Company.NotFound, exception.ErrorCode);
        repository.Verify(r => r.SoftDeleteAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowPermissionDenied_WhenUserLacksCompanyDeletePermission()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<Company>> repository) = UnitOfWorkMockFactory.Create<Company>();

        repository.Setup(r => r.GetByIdAsync("company-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingCompany());
        (Mock<IPermissionService> permissionService, Mock<ICompanyContext> companyContext) = MockPermission(permitted: false);

        DeleteCompanyCommandHandler handler = new(unitOfWork.Object, permissionService.Object, companyContext.Object);
        DeleteCompanyCommand command = new() { Id = "company-1" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.Company.PermissionDenied, exception.ErrorCode);
        repository.Verify(r => r.SoftDeleteAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldDeleteCompany_WhenCallerIsSystemAdmin_EvenWithoutCompanyDeletePermission()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<Company>> repository) = UnitOfWorkMockFactory.Create<Company>();

        repository.Setup(r => r.GetByIdAsync("company-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingCompany());
        repository.Setup(r => r.SoftDeleteAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        (Mock<IPermissionService> permissionService, Mock<ICompanyContext> companyContext) = MockPermission(permitted: false);
        companyContext.Setup(c => c.IsInRole(RoleList.SystemAdmin)).Returns(true);

        DeleteCompanyCommandHandler handler = new(unitOfWork.Object, permissionService.Object, companyContext.Object);
        DeleteCompanyCommand command = new() { Id = "company-1" };

        bool result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        repository.Verify(r => r.SoftDeleteAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
