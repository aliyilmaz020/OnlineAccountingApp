using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineAccountingApp.Domain.CompanyEntities;
using OnlineAccountingApp.Persistence.Constants;

namespace OnlineAccountingApp.Persistence.Configurations;

public sealed class UCAFConfiguration : IEntityTypeConfiguration<UniformChartOfAccount>
{
    public void Configure(EntityTypeBuilder<UniformChartOfAccount> builder)
    {
        builder.ToTable(TableNames.UniformChartOfAccounts);
        builder.HasKey(p => p.Id);
    }
}
