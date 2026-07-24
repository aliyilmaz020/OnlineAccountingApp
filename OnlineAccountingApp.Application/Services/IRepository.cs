using OnlineAccountingApp.Domain.Abstracts;
using System.Linq.Expressions;

namespace OnlineAccountingApp.Application.Services;

public interface IRepository<T> where T : BaseEntity
{
    Task<bool> CreateAsync(T entity);
    Task<bool> DeleteAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<T?> GetAsync(Expression<Func<T,bool>> predicate);
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T,bool>> predicate);
}
