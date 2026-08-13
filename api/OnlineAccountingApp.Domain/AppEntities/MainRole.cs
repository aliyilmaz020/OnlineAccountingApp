using System.ComponentModel.DataAnnotations.Schema;
using OnlineAccountingApp.Domain.Abstracts;

namespace OnlineAccountingApp.Domain.AppEntities;

public sealed class MainRole : BaseEntity
{
    public string Title { get; set; }
    public bool IsRoleCreateByAdmin { get; set; }
    [ForeignKey("Company")]
    public string CompanyId { get; set; }
    public Company? Company { get; set; }
}