using Moq;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.Create;
using OnlineAccountingApp.Application.Features.AppFeatures.MainRoleAndRoleRelationshipFeature.GetMainRoleAndRoleRelationships;
using OnlineAccountingApp.Application.Tests.TestHelpers;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Exceptions;
using OnlineAccountingApp.Framework.Services;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Tests.Features.MainRoleAndRoleRelationshipFeature;

public class CreateMainRoleAndRoleRelationshipCommandHandlerTests
{
    private static CreateMainRoleAndRoleRelationshipCommand BuildCommand() => new()
    {
        RoleId = "role-1",
        MainRoleId = "main-role-1"
    };

    [Fact]
    public async Task Handle_ShouldCreateRelationship_WhenPairDoesNotExist()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRoleAndRoleRelationship>> repository) =
            UnitOfWorkMockFactory.Create<MainRoleAndRoleRelationship>();

        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<MainRoleAndRoleRelationship, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repository.Setup(r => r.CreateAsync(It.IsAny<MainRoleAndRoleRelationship>(), It.IsAny<CancellationToken>()))
            .Returns<MainRoleAndRoleRelationship, CancellationToken>((entity, _) =>
            {
                entity.Id = "generated-id";
                return Task.FromResult(entity);
            });

        CreateMainRoleAndRoleRelationshipCommandHandler handler = new(unitOfWork.Object);
        CreateMainRoleAndRoleRelationshipCommand command = BuildCommand();

        MainRoleAndRoleRelationshipListItemDto result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("generated-id", result.Id);
        Assert.Equal(command.RoleId, result.RoleId);
        Assert.Equal(command.MainRoleId, result.MainRoleId);

        repository.Verify(r => r.CreateAsync(It.IsAny<MainRoleAndRoleRelationship>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenPairAlreadyExists()
    {
        (Mock<IUnitOfWork> unitOfWork, Mock<IRepository<MainRoleAndRoleRelationship>> repository) =
            UnitOfWorkMockFactory.Create<MainRoleAndRoleRelationship>();

        Expression<Func<MainRoleAndRoleRelationship, bool>>? capturedPredicate = null;
        repository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<MainRoleAndRoleRelationship, bool>>>(), It.IsAny<CancellationToken>()))
            .Callback<Expression<Func<MainRoleAndRoleRelationship, bool>>, CancellationToken>((predicate, _) => capturedPredicate = predicate)
            .ReturnsAsync(true);

        CreateMainRoleAndRoleRelationshipCommandHandler handler = new(unitOfWork.Object);
        CreateMainRoleAndRoleRelationshipCommand command = BuildCommand();

        BusinessException exception = await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(AppErrorCodes.MainRoleAndRoleRelationship.AlreadyExists, exception.ErrorCode);
        repository.Verify(r => r.CreateAsync(It.IsAny<MainRoleAndRoleRelationship>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.NotNull(capturedPredicate);
        Func<MainRoleAndRoleRelationship, bool> compiledPredicate = capturedPredicate!.Compile();
        Assert.True(compiledPredicate(new MainRoleAndRoleRelationship { RoleId = command.RoleId, MainRoleId = command.MainRoleId }));
        Assert.False(compiledPredicate(new MainRoleAndRoleRelationship { RoleId = "role-2", MainRoleId = command.MainRoleId }));
        Assert.False(compiledPredicate(new MainRoleAndRoleRelationship { RoleId = command.RoleId, MainRoleId = "main-role-2" }));
    }
}
