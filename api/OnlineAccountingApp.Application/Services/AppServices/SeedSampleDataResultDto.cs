namespace OnlineAccountingApp.Application.Services.AppServices;

public sealed class SeedSampleDataResultDto
{
    public int PermissionRolesCreated { get; set; }
    public int CompaniesCreated { get; set; }
    public int UsersCreated { get; set; }
    public int UserCompanyLinksCreated { get; set; }
    public int MainRolesCreated { get; set; }
    public int MainRoleRoleLinksCreated { get; set; }
    public int MainRoleRoleLinksRemoved { get; set; }
    public int MainRoleUserLinksCreated { get; set; }
}
