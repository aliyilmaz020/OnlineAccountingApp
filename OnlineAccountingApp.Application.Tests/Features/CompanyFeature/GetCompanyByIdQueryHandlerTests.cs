using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanyById;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.Entities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.CompanyFeature;

public class GetCompanyByIdQueryHandlerTests
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
    public async Task Handle_ShouldReturnCompany_WhenExists()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<Company>> repository) = UnitOfWorkMockFactory.Create<Company>();

        repository.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Company, bool>>>(),
                It.IsAny<Expression<Func<Company, object>>[]?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingCompany());

        GetCompanyByIdQueryHandler handler = new(unitOfWork.Object);
        GetCompanyByIdQuery query = new() { Id = "company-1" };

        CompanyListItemDto result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal("company-1", result.Id);
        Assert.Equal("Acme Corp", result.Name);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenMissing()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<Company>> repository) = UnitOfWorkMockFactory.Create<Company>();

        repository.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Company, bool>>>(),
                It.IsAny<Expression<Func<Company, object>>[]?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Company?)null);

        GetCompanyByIdQueryHandler handler = new(unitOfWork.Object);
        GetCompanyByIdQuery query = new() { Id = "missing-id" };

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(query, CancellationToken.None));

        Assert.Equal(AppErrorCodes.Company.NotFound, exception.ErrorCode);
    }
}
