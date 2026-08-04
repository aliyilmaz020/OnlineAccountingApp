using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using OnlineAccountingApp.Domain.Abstracts;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Entities;
using OnlineAccountingApp.Domain.Entities.Identity;

namespace OnlineAccountingApp.Persistence.Context;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser, AppRole, string>(options)
{
    public DbSet<Company> Companies { get; set; }
    public DbSet<UserCompany> UserCompanies { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<MainRole> MainRoles { get; set; }
    public DbSet<MainRoleAndRoleRelationship> MainRoleAndRoleRelationships { get; set; }
    public DbSet<MainRoleAndUserRelationship> MainRoleAndUserRelationships { get; set; }

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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<AppUser>().ToTable("Users");
        builder.Entity<AppRole>().ToTable("Roles");
        builder.Entity<AppRole>().HasIndex(role => role.Code).IsUnique();
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Ignore<IdentityUserLogin<string>>();
        builder.Ignore<IdentityUserClaim<string>>();
        builder.Ignore<IdentityUserToken<string>>();
        builder.Ignore<IdentityRoleClaim<string>>();

        builder.Entity<MainRoleAndUserRelationship>()
            .HasOne(r => r.Company)
            .WithMany()
            .HasForeignKey(r => r.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
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
