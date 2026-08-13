using OnlineAccountingApp.Domain.Abstracts;

namespace OnlineAccountingApp.Domain.CompanyEntities;

public sealed class UniformChartOfAccount : BaseEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
}
