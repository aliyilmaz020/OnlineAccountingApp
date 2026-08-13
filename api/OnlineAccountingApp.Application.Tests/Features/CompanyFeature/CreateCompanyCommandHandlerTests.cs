using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.Create;
using OnlineAccountingApp.Application.Features.AppFeatures.CompanyFeature.GetCompanies;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.CompanyFeature;

public class CreateCompanyCommandHandlerTests
{
    private static CreateCompanyCommand BuildCommand() => new()
    {
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
    public async Task Handle_ShouldCreateCompany_WhenNameDoesNotExist()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<Company>> repository) = UnitOfWorkMockFactory.Create<Company>();

        repository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Company, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repository.Setup(r => r.CreateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .Returns<Company, CancellationToken>((entity, _) =>
            {
                entity.Id = "generated-id";
                return Task.FromResult(entity);
            });

        CreateCompanyCommandHandler handler = new(unitOfWork.Object);
        CreateCompanyCommand command = BuildCommand();

        CompanyListItemDto result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("generated-id", result.Id);
        Assert.Equal(command.Name, result.Name);
        Assert.Equal(command.Email, result.Email);

        repository.Verify(r => r.CreateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenCompanyNameAlreadyExists()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<Company>> repository) = UnitOfWorkMockFactory.Create<Company>();

        Expression<Func<Company, bool>>? capturedPredicate = null;
        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Company, bool>>>(), It.IsAny<CancellationToken>()))
            .Callback<Expression<Func<Company, bool>>, CancellationToken>((predicate, _) => capturedPredicate = predicate)
            .ReturnsAsync(true);

        CreateCompanyCommandHandler handler = new(unitOfWork.Object);
        CreateCompanyCommand command = BuildCommand();

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.Company.AlreadyExists, exception.ErrorCode);
        repository.Verify(r => r.CreateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.NotNull(capturedPredicate);
        Func<Company, bool> compiledPredicate = capturedPredicate!.Compile();
        Assert.True(compiledPredicate(new Company { Name = command.Name }));
        Assert.False(compiledPredicate(new Company { Name = "A Completely Different Name" }));
    }
}
