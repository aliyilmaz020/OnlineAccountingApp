using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineAccountingApp.Application.Services.AppServices;
using OnlineAccountingApp.Domain.AppEntities;
using OnlineAccountingApp.Domain.Entities;
using OnlineAccountingApp.Domain.Entities.Identity;
using OnlineAccountingApp.Domain.Roles;
using OnlineAccountingApp.Persistence.Context;
using DomainValidationException = OnlineAccountingApp.Domain.Exceptions.ValidationException;

namespace OnlineAccountingApp.Persistence.Services.AppServices;

/// <summary>
/// Fills the master DB with realistic, idempotent development data: the static UCAF
/// permission roles, two sample companies, two users per company, and the MainRole
/// scaffolding that links them. Re-running it never duplicates rows - every step
/// checks by natural key first. Tenant (CompanyDbContext) data is intentionally out
/// of scope, since it would need a reachable per-company SQL Server connection.
/// </summary>
public sealed class SeedService(AppDbContext context, UserManager<AppUser> userManager, IRoleService roleService) : ISeedService
{
    private const string SamplePassword = "Sample!2024";

    public async Task<SeedSampleDataResultDto> SeedSampleDataAsync(CancellationToken cancellationToken = default)
    {
        var result = new SeedSampleDataResultDto();

        Dictionary<string, AppRole> permissionRoles = await SeedStaticRolesAsync(result, cancellationToken);

        foreach (SeedCompanySpec spec in BuildSampleCompanies())
        {
            Company company = await GetOrCreateCompanyAsync(spec.Company, result, cancellationToken);
            List<AppUser> users = await GetOrCreateUsersAsync(company, spec.UserEmails, result, cancellationToken);
            await SeedMainRolesAndRelationshipsAsync(company, users, permissionRoles, result, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
        return result;
    }

    private async Task<Dictionary<string, AppRole>> SeedStaticRolesAsync(SeedSampleDataResultDto result, CancellationToken cancellationToken)
    {
        var roles = new Dictionary<string, AppRole>();
        foreach (AppRole role in RoleList.GetStaticRoles())
        {
            AppRole? existingRole = await roleService.GetByCodeAsync(role.Code, cancellationToken);
            if (existingRole is not null)
            {
                roles[role.Code] = existingRole;
                continue;
            }

            role.Id = Guid.NewGuid().ToString();
            role.CreateDate = DateTime.UtcNow;
            role.Status = true;
            role.Deleted = false;

            roles[role.Code] = await roleService.CreateAsync(role, cancellationToken);
            result.PermissionRolesCreated++;
        }

        return roles;
    }

    private async Task<Company> GetOrCreateCompanyAsync(Company template, SeedSampleDataResultDto result, CancellationToken cancellationToken)
    {
        Company? company = await context.Companies.FirstOrDefaultAsync(
            c => c.Name == template.Name && !c.Deleted, cancellationToken);
        if (company is not null)
        {
            return company;
        }

        context.Companies.Add(template);
        result.CompaniesCreated++;
        return template;
    }

    private async Task<List<AppUser>> GetOrCreateUsersAsync(
        Company company, IReadOnlyList<string> emails, SeedSampleDataResultDto result, CancellationToken cancellationToken)
    {
        var users = new List<AppUser>();
        foreach (string email in emails)
        {
            AppUser? user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                user = new AppUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    Status = true
                };
                ThrowIfFailed(await userManager.CreateAsync(user, SamplePassword));
                result.UsersCreated++;
            }

            users.Add(user);

            bool linked = await context.UserCompanies.AnyAsync(
                uc => uc.AppUserId == user.Id && uc.CompanyId == company.Id, cancellationToken);
            if (!linked)
            {
                context.UserCompanies.Add(new UserCompany { AppUserId = user.Id, CompanyId = company.Id });
                result.UserCompanyLinksCreated++;
            }
        }

        return users;
    }

    private async Task SeedMainRolesAndRelationshipsAsync(
        Company company,
        List<AppUser> users,
        IReadOnlyDictionary<string, AppRole> permissionRoles,
        SeedSampleDataResultDto result,
        CancellationToken cancellationToken)
    {
        (string Title, bool IsAdmin, AppUser User)[] mainRoleSpecs =
        [
            ("Yönetici", true, users[0]),
            ("Muhasebeci", false, users[1])
        ];

        foreach (var (title, isAdmin, user) in mainRoleSpecs)
        {
            MainRole mainRole = await GetOrCreateMainRoleAsync(company, title, isAdmin, result, cancellationToken);

            foreach (AppRole permissionRole in permissionRoles.Values)
            {
                bool linked = await context.MainRoleAndRoleRelationships.AnyAsync(
                    x => x.MainRoleId == mainRole.Id && x.RoleId == permissionRole.Id, cancellationToken);
                if (!linked)
                {
                    context.MainRoleAndRoleRelationships.Add(new MainRoleAndRoleRelationship
                    {
                        MainRoleId = mainRole.Id,
                        RoleId = permissionRole.Id
                    });
                    result.MainRoleRoleLinksCreated++;
                }
            }

            bool userLinked = await context.MainRoleAndUserRelationships.AnyAsync(
                x => x.UserId == user.Id && x.MainRoleId == mainRole.Id && x.CompanyId == company.Id, cancellationToken);
            if (!userLinked)
            {
                context.MainRoleAndUserRelationships.Add(new MainRoleAndUserRelationship
                {
                    UserId = user.Id,
                    MainRoleId = mainRole.Id,
                    CompanyId = company.Id
                });
                result.MainRoleUserLinksCreated++;
            }
        }
    }

    private async Task<MainRole> GetOrCreateMainRoleAsync(
        Company company, string title, bool isCreatedByAdmin, SeedSampleDataResultDto result, CancellationToken cancellationToken)
    {
        MainRole? mainRole = await context.MainRoles.FirstOrDefaultAsync(
            mr => mr.CompanyId == company.Id && mr.Title == title, cancellationToken);
        if (mainRole is not null)
        {
            return mainRole;
        }

        mainRole = new MainRole
        {
            Title = title,
            IsRoleCreateByAdmin = isCreatedByAdmin,
            CompanyId = company.Id
        };
        context.MainRoles.Add(mainRole);
        result.MainRolesCreated++;
        return mainRole;
    }

    private static List<SeedCompanySpec> BuildSampleCompanies() =>
    [
        new SeedCompanySpec(
            new Company
            {
                Name = "Anadolu Ticaret A.Ş.",
                Address = "Bağdat Cad. No:120, Kadıköy/İstanbul",
                IdentityNumber = "1234567890",
                TaxDepartment = "Kadıköy Vergi Dairesi",
                PhoneNumber = "0216 555 01 23",
                Email = "info@anadoluticaret.com.tr",
                ServerName = "localhost",
                DatabaseName = "AnadoluTicaretDb",
                ServerUserId = "sa",
                ServerPassword = SamplePassword
            },
            ["ahmet.yilmaz@anadoluticaret.com.tr", "ayse.demir@anadoluticaret.com.tr"]),
        new SeedCompanySpec(
            new Company
            {
                Name = "Ege Gıda San. ve Tic. Ltd. Şti.",
                Address = "Cumhuriyet Bulvarı No:45, Konak/İzmir",
                IdentityNumber = "9876543210",
                TaxDepartment = "Konak Vergi Dairesi",
                PhoneNumber = "0232 555 44 55",
                Email = "info@egegida.com.tr",
                ServerName = "localhost",
                DatabaseName = "EgeGidaDb",
                ServerUserId = "sa",
                ServerPassword = SamplePassword
            },
            ["mehmet.kaya@egegida.com.tr", "elif.sahin@egegida.com.tr"])
    ];

    private sealed record SeedCompanySpec(Company Company, string[] UserEmails);

    /// <summary>Surfaces Identity's own failures through the app's validation error shape.</summary>
    private static void ThrowIfFailed(IdentityResult result)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = result.Errors
            .GroupBy(error => error.Code)
            .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray());

        throw new DomainValidationException(errors);
    }
}
