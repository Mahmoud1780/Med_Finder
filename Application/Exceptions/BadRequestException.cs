namespace MedicineFinder.Application.Exceptions;

public class BadRequestException : ApiException
{
    public BadRequestException(string message, string code = "BadRequest", IReadOnlyList<string>? details = null)
        : base(code, message, 400, details)
    {
    }
}
