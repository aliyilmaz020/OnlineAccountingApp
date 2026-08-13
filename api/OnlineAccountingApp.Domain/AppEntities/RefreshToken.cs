using OnlineAccountingApp.Domain.Abstracts;
using OnlineAccountingApp.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineAccountingApp.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    public string Token { get; set; }

    [ForeignKey("AppUser")]
    public string AppUserId { get; set; }
    public AppUser User { get; set; }

    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public bool IsActive(DateTime utcNow) => RevokedAt is null && !Deleted && ExpiresAt > utcNow;
}
