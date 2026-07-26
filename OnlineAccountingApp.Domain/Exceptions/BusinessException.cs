namespace OnlineAccountingApp.Domain.Exceptions;

public class BusinessException : Exception
{
    public string ErrorCode { get; }
    public int HttpStatusCode { get; }

    public BusinessException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
        HttpStatusCode = int.Parse(errorCode[^3..]);
    }
}
