using OnlineAccountingApp.Domain.Abstracts;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineAccountingApp.Domain.Entities;

public class UserCompany : BaseEntity
{
    [ForeignKey("AppUser")]
    public string AppUserId { get; set; }
    public AppUser User { get; set; }

    [ForeignKey("Company")]
    public string CompanyId { get; set; }
    public Company Company { get; set; }
}
