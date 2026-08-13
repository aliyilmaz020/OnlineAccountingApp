using Moq;
using OnlineAccountingApp.Domain.Abstracts;
using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Tests.TestHelpers;

public static class UnitOfWorkMockFactory
{
    public static (Mock<IUnitOfWork> UnitOfWork, Mock<IRepository<TEntity>> Repository) Create<TEntity>()
        where TEntity : BaseEntity
        => Create<TEntity, IUnitOfWork>();

    public static (Mock<TUnitOfWork> UnitOfWork, Mock<IRepository<TEntity>> Repository) Create<TEntity, TUnitOfWork>()
        where TEntity : BaseEntity
        where TUnitOfWork : class, IUnitOfWork
    {
        Mock<IRepository<TEntity>> repository = new();

        Mock<TUnitOfWork> unitOfWork = new();
        unitOfWork.Setup(u => u.Repository<TEntity>()).Returns(repository.Object);
        unitOfWork.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        unitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        unitOfWork.Setup(u => u.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        return (unitOfWork, repository);
    }
}
