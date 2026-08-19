namespace OnlineAccountingApp.Domain.Exceptions;

/// <summary>
/// Fixed application error codes. Format: {2-digit service code}{3-digit HTTP status code}.
/// </summary>
public static class AppErrorCodes
{
    public static class Common
    {
        private const string ServiceCode = "00";

        public const string ValidationFailed = ServiceCode + "400";
    }

    public static class Company
    {
        private const string ServiceCode = "01";

        public const string NotFound = ServiceCode + "404";
        public const string AlreadyExists = ServiceCode + "409";
        public const string ValidationFailed = ServiceCode + "400";
        public const string PermissionDenied = ServiceCode + "403";
    }

    public static class UniformChartOfAccount
    {
        private const string ServiceCode = "02";

        public const string NotFound = ServiceCode + "404";
        public const string AlreadyExists = ServiceCode + "409";
        public const string ValidationFailed = ServiceCode + "400";
    }

    public static class Tenant
    {
        private const string ServiceCode = "03";

        public const string CompanyNotSpecified = ServiceCode + "400";
        public const string CompanyNotFound = ServiceCode + "404";
    }

    public static class Auth
    {
        private const string ServiceCode = "04";

        public const string ValidationFailed = ServiceCode + "400";
        public const string InvalidCredentials = ServiceCode + "401";
        public const string InvalidRefreshToken = ServiceCode + "401";
        public const string CompanyAccessDenied = ServiceCode + "403";
        public const string UserAlreadyExists = ServiceCode + "409";
    }

    public static class Role
    {
        private const string ServiceCode = "05";

        public const string NotFound = ServiceCode + "404";
        public const string AlreadyExists = ServiceCode + "409";
        public const string ValidationFailed = ServiceCode + "400";
    }

    public static class MainRole
    {
        private const string ServiceCode = "06";

        public const string NotFound = ServiceCode + "404";
        public const string AlreadyExists = ServiceCode + "409";
        public const string ValidationFailed = ServiceCode + "400";
    }

    public static class MainRoleAndRoleRelationship
    {
        private const string ServiceCode = "07";

        public const string NotFound = ServiceCode + "404";
        public const string AlreadyExists = ServiceCode + "409";
        public const string ValidationFailed = ServiceCode + "400";
    }

    public static class MainRoleAndUserRelationship
    {
        private const string ServiceCode = "08";

        public const string NotFound = ServiceCode + "404";
        public const string AlreadyExists = ServiceCode + "409";
        public const string ValidationFailed = ServiceCode + "400";
    }
}
