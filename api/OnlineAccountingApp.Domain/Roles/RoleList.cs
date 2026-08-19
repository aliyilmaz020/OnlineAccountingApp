using OnlineAccountingApp.Domain.Entities.Identity;

namespace OnlineAccountingApp.Domain.Roles;

public sealed class RoleList
{
    #region Static Roles

    public static List<AppRole> GetStaticRoles()
    {
        var roles = new List<AppRole>
        {
            new(
                title: UCAF,
                code: UCAFCreateCode,
                name: UCAFCreateName),
            new(
                title: UCAF,
                code: UCAFUpdateCode,
                name: UCAFUpdateName),
            new(
                title: UCAF,
                code: UCAFDeleteCode,
                name: UCAFDeleteName),
            new(
                title: UCAF,
                code: UCAFReadCode,
                name: UCAFReadName),
            new(
                title: COMPANY,
                code: CompanyReadCode,
                name: CompanyReadName),
            new(
                title: COMPANY,
                code: CompanyUpdateCode,
                name: CompanyUpdateName),
            new(
                title: COMPANY,
                code: CompanyDeleteCode,
                name: CompanyDeleteName),
            new(
                title: SystemAdmin,
                code: SystemAdmin,
                name: SystemAdmin)
        };
        return roles;
    }

    #endregion

    #region UniformChartOfAccount (UCAF)

    public const string UCAF = "Chart Of Account";
    public const string UCAFCreateCode = "UCAF.Create";
    public const string UCAFCreateName = "Chart Of Account Create";
    public const string UCAFUpdateCode = "UCAF.Update";
    public const string UCAFUpdateName = "Chart Of Account Update";
    public const string UCAFDeleteCode = "UCAF.Delete";
    public const string UCAFDeleteName = "Chart Of Account Delete";
    public const string UCAFReadCode = "UCAF.Read";
    public const string UCAFReadName = "Chart Of Account Read";

    #endregion

    #region Company

    // No Company.Create code: creating a brand-new company happens before any MainRole for it
    // exists, so there is no company id yet to check a permission against.
    public const string COMPANY = "Company";
    public const string CompanyReadCode = "Company.Read";
    public const string CompanyReadName = "Company Read";
    public const string CompanyUpdateCode = "Company.Update";
    public const string CompanyUpdateName = "Company Update";
    public const string CompanyDeleteCode = "Company.Delete";
    public const string CompanyDeleteName = "Company Delete";

    #endregion

    #region System

    /// <summary>
    /// A global Identity role (assigned directly via UserManager, not through the per-company
    /// MainRole chain) that grants system-wide access - unlike a company's own "Yönetici"
    /// MainRole, which only ever applies within that one company.
    /// </summary>
    public const string SystemAdmin = "Admin";

    #endregion
}
