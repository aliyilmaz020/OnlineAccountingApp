using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Delete;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Tests.Features.CompanyFeature;

public class DeleteCompanyCommandHandlerTests
{
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

        DeleteCompanyCommandHandler handler = new(unitOfWork.Object);
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

        DeleteCompanyCommandHandler handler = new(unitOfWork.Object);
        DeleteCompanyCommand command = new() { Id = "company-1" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.Company.NotFound, exception.ErrorCode);
        repository.Verify(r => r.SoftDeleteAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
