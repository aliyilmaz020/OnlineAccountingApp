using Microsoft.AspNetCore.Identity;

namespace OnlineAccountingApp.Domain.Entities.Identity;

public sealed class AppRole : IdentityRole<string>
{
    public string Code { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? EditDate { get; set; }
    public bool Status { get; set; }
    public bool Deleted { get; set; }
}
