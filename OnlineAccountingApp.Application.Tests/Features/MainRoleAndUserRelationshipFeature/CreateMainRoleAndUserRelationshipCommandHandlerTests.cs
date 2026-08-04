using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.Create;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndUserRelationshipFeature.GetMainRoleAndUserRelationships;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.MainRoleAndUserRelationshipFeature;

public class CreateMainRoleAndUserRelationshipCommandHandlerTests
{
    private static CreateMainRoleAndUserRelationshipCommand BuildCommand() => new()
    {
        UserId = "user-1",
        MainRoleId = "main-role-1",
        CompanyId = "company-1"
    };

    [Fact]
    public async Task Handle_ShouldCreateRelationship_WhenTripleDoesNotExist()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRoleAndUserRelationship>> repository) =
            UnitOfWorkMockFactory.Create<MainRoleAndUserRelationship>();

        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<MainRoleAndUserRelationship, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repository.Setup(r => r.CreateAsync(It.IsAny<MainRoleAndUserRelationship>(), It.IsAny<CancellationToken>()))
            .Returns<MainRoleAndUserRelationship, CancellationToken>((entity, _) =>
            {
                entity.Id = "generated-id";
                return Task.FromResult(entity);
            });

        CreateMainRoleAndUserRelationshipCommandHandler handler = new(unitOfWork.Object);
        CreateMainRoleAndUserRelationshipCommand command = BuildCommand();

        MainRoleAndUserRelationshipListItemDto result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("generated-id", result.Id);
        Assert.Equal(command.UserId, result.UserId);
        Assert.Equal(command.MainRoleId, result.MainRoleId);
        Assert.Equal(command.CompanyId, result.CompanyId);

        repository.Verify(r => r.CreateAsync(It.IsAny<MainRoleAndUserRelationship>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenTripleAlreadyExists()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRoleAndUserRelationship>> repository) =
            UnitOfWorkMockFactory.Create<MainRoleAndUserRelationship>();

        Expression<Func<MainRoleAndUserRelationship, bool>>? capturedPredicate = null;
        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<MainRoleAndUserRelationship, bool>>>(), It.IsAny<CancellationToken>()))
            .Callback<Expression<Func<MainRoleAndUserRelationship, bool>>, CancellationToken>((predicate, _) => capturedPredicate = predicate)
            .ReturnsAsync(true);

        CreateMainRoleAndUserRelationshipCommandHandler handler = new(unitOfWork.Object);
        CreateMainRoleAndUserRelationshipCommand command = BuildCommand();

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.MainRoleAndUserRelationship.AlreadyExists, exception.ErrorCode);
        repository.Verify(r => r.CreateAsync(It.IsAny<MainRoleAndUserRelationship>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.NotNull(capturedPredicate);
        Func<MainRoleAndUserRelationship, bool> compiledPredicate = capturedPredicate!.Compile();
        Assert.True(compiledPredicate(new MainRoleAndUserRelationship
        {
            UserId = command.UserId,
            MainRoleId = command.MainRoleId,
            CompanyId = command.CompanyId
        }));
        Assert.False(compiledPredicate(new MainRoleAndUserRelationship
        {
            UserId = "user-2",
            MainRoleId = command.MainRoleId,
            CompanyId = command.CompanyId
        }));
        Assert.False(compiledPredicate(new MainRoleAndUserRelationship
        {
            UserId = command.UserId,
            MainRoleId = command.MainRoleId,
            CompanyId = "company-2"
        }));
    }
}
