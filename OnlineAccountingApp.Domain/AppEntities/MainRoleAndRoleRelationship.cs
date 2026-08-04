using System.ComponentModel.DataAnnotations.Schema;
using OnlineAccountingApp.Domain.Abstracts;
using OnlineAccountingApp.Domain.Entities.Identity;

namespace OnlineAccountingApp.Domain.AppEntities;

public sealed class MainRoleAndRoleRelationship : BaseEntity
{
    [ForeignKey("AppRole")]
    public string RoleId { get; set; }
    public AppRole AppRole { get; set; }
    [ForeignKey("MainRole")]
    public string MainRoleId { get; set; }
    public MainRole MainRole { get; set; }
}