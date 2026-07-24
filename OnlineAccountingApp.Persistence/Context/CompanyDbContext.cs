using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using OnlineAccountingApp.Domain.Entities;
using OnlineAccountingApp.Persistence.Configurations;

namespace OnlineAccountingApp.Persistence.Context;

public sealed class CompanyDbContext : DbContext
{
    private string ConnectionString = string.Empty;

    public CompanyDbContext(Company? company)
    {
        if (company is not null)
        {
            if (company != null)
            {
                ConnectionString = $"" +
                    $"Data Source={company.ServerName};" +
                    $"Initial Catalog={company.DatabaseName};" +
                    $"User Id={company.ServerUserId};" +
                    $"Password={company.ServerPassword};" +
                    $"Connect Timeout=30;" +
                    $"Encrypt=False;" +
                    $"TrustServerCertificate=False;" +
                    $"ApplicationIntent=ReadWrite;" +
                    $"MultiSubnetFailover=False";
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UCAFConfiguration());
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(ConnectionString);
    }
    public class CompanyDbContextFactory : IDesignTimeDbContextFactory<CompanyDbContext>
    {
        public CompanyDbContext CreateDbContext(string[] args)
        {
            return new CompanyDbContext(null);
        }
    }
}
