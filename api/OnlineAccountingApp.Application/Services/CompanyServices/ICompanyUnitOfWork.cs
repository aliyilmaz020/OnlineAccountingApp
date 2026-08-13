using OnlineAccountingApp.Framework.Services;

namespace OnlineAccountingApp.Application.Services.CompanyServices;

/// <summary>
/// Unit of work bound to the current company's own database, as opposed to
/// <see cref="IUnitOfWork"/> which always works against the master database.
/// </summary>
public interface ICompanyUnitOfWork : IUnitOfWork
{
}
