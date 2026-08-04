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
                name: UCAFReadName)
        };
        return roles;
    }

    #endregion

    #region UniformChartOfAccount (UCAF)
    
    public static string UCAF => "Chart Of Account";
    public static string UCAFCreateCode => "UCAF.Create";
    public static string UCAFCreateName => "Chart Of Account Create";
    public static string UCAFUpdateCode => "UCAF.Update";
    public static string UCAFUpdateName => "Chart Of Account Update";
    public static string UCAFDeleteCode => "UCAF.Delete";
    public static string UCAFDeleteName => "Chart Of Account Delete";
    public static string UCAFReadCode => "UCAF.Read";
    public static string UCAFReadName => "Chart Of Account Read";

    #endregion
}
