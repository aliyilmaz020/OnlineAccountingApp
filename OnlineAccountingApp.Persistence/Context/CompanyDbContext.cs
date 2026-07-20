using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using OnlineAccountingApp.Domain.Entities;

namespace OnlineAccountingApp.Persistence.Context;

public sealed class CompanyDbContext : DbContext
{
    private string ConnectionString = string.Empty;

    public CompanyDbContext(Company? company)
    {
        if (company is not null)
        {
            if (string.IsNullOrEmpty(company.ServerUserId))
                ConnectionString =
                    $"Server={company.ServerName};Database={company.DatabaseName};TrustServerCertificate=True;";
            else
                ConnectionString =
                    $"Server={company.ServerName};Database={company.DatabaseName};User Id={company.ServerUserId};Password={company.ServerPassword};TrustServerCertificate=True;";
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyReference).Assembly);
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
