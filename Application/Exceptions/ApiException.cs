namespace MedicineFinder.Application.Exceptions;

public class ApiException : Exception
{
    public ApiException(string code, string message, int statusCode, IReadOnlyList<string>? details = null)
        : base(message)
    {
        Code = code;
        StatusCode = statusCode;
        Details = details ?? Array.Empty<string>();
    }

    public string Code { get; }
    public int StatusCode { get; }
    public IReadOnlyList<string> Details { get; }
}
