namespace OnlineAccountingApp.Framework.MedatR.Common;

public interface IPagedQuery
{
    int PageNumber { get; }
    int PageSize { get; }
}
