using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.Create;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleFeature.GetMainRoles;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.MainRoleFeature;

public class CreateMainRoleCommandHandlerTests
{
    private static CreateMainRoleCommand BuildCommand() => new()
    {
        Title = "Muhasebeci",
        IsRoleCreateByAdmin = true,
        CompanyId = "company-1"
    };

    [Fact]
    public async Task Handle_ShouldCreateMainRole_WhenTitleDoesNotExistForCompany()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRole>> repository) = UnitOfWorkMockFactory.Create<MainRole>();

        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<MainRole, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repository.Setup(r => r.CreateAsync(It.IsAny<MainRole>(), It.IsAny<CancellationToken>()))
            .Returns<MainRole, CancellationToken>((entity, _) =>
            {
                entity.Id = "generated-id";
                return Task.FromResult(entity);
            });

        CreateMainRoleCommandHandler handler = new(unitOfWork.Object);
        CreateMainRoleCommand command = BuildCommand();

        MainRoleListItemDto result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("generated-id", result.Id);
        Assert.Equal(command.Title, result.Title);
        Assert.Equal(command.CompanyId, result.CompanyId);

        repository.Verify(r => r.CreateAsync(It.IsAny<MainRole>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenTitleAlreadyExistsForCompany()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRole>> repository) = UnitOfWorkMockFactory.Create<MainRole>();

        Expression<Func<MainRole, bool>>? capturedPredicate = null;
        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<MainRole, bool>>>(), It.IsAny<CancellationToken>()))
            .Callback<Expression<Func<MainRole, bool>>, CancellationToken>((predicate, _) => capturedPredicate = predicate)
            .ReturnsAsync(true);

        CreateMainRoleCommandHandler handler = new(unitOfWork.Object);
        CreateMainRoleCommand command = BuildCommand();

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.MainRole.AlreadyExists, exception.ErrorCode);
        repository.Verify(r => r.CreateAsync(It.IsAny<MainRole>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.NotNull(capturedPredicate);
        Func<MainRole, bool> compiledPredicate = capturedPredicate!.Compile();
        Assert.True(compiledPredicate(new MainRole { Title = command.Title, CompanyId = command.CompanyId }));
        Assert.False(compiledPredicate(new MainRole { Title = "Different Title", CompanyId = command.CompanyId }));
        Assert.False(compiledPredicate(new MainRole { Title = command.Title, CompanyId = "company-2" }));
    }
}
