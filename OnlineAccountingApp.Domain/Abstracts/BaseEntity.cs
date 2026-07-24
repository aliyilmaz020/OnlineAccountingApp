namespace OnlineAccountingApp.Domain.Abstracts;

public abstract class BaseEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreateDate { get; set; }
    public DateTime? EditDate { get; set; }
    public bool Status { get; set; }
    public bool Deleted { get; set; }
}
