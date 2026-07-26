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
    }
}
