using Microsoft.AspNetCore.Identity;
using OnlineAccountingApp.Domain.Abstracts;

namespace OnlineAccountingApp.Domain.Entities.Identity;

public sealed class AppUser : IdentityUser<string>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? EditDate { get; set; }
    public bool Status { get; set; }
    public bool Deleted { get; set; }
}
