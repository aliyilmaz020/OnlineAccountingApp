using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using OnlineAccountingApp.Domain.Abstracts;
using OnlineAccountingApp.Domain.Entities;
using OnlineAccountingApp.Domain.Entities.Identity;

namespace OnlineAccountingApp.Persistence.Context;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser, AppRole, string>(options)
{
    public DbSet<Company> Companies { get; set; }
    public DbSet<UserCompany> UserCompanies { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreateDate = DateTime.Now;
                    break;
                case EntityState.Modified:
                    entry.Entity.EditDate = DateTime.Now;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

     public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=AccountingMasterDb;User Id=sa;Password=Password1;TrustServerCertificate=True;");
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
