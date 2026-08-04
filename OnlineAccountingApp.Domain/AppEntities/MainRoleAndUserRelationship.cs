using System.ComponentModel.DataAnnotations.Schema;
using OnlineAccountingApp.Domain.Abstracts;
using OnlineAccountingApp.Domain.Entities.Identity;

namespace OnlineAccountingApp.Domain.AppEntities;

public sealed class MainRoleAndUserRelationship : BaseEntity
{
    [ForeignKey("AppUser")]
    public string UserId { get; set; }
    public AppUser AppUser { get; set; }
    [ForeignKey("MainRole")]
    public string MainRoleId { get; set; }
    public MainRole MainRole { get; set; }
    [ForeignKey("Company")]
    public string CompanyId { get; set; }
    public Company Company { get; set; }
}