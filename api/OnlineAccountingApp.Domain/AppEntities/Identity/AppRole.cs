using Microsoft.AspNetCore.Identity;

namespace OnlineAccountingApp.Domain.Entities.Identity;

public sealed class AppRole : IdentityRole<string>
{
    public AppRole()
    {
        
    }
    public AppRole(string title, string code, string name)
    {
        Id = Guid.NewGuid().ToString();
        Title = title;
        Code = code;
        Name = name;
    }
    public string Code { get; set; }
    public string Title { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? EditDate { get; set; }
    public bool Status { get; set; }
    public bool Deleted { get; set; }
}
