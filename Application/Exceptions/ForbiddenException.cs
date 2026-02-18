namespace MedicineFinder.Application.Exceptions;

public class ForbiddenException : ApiException
{
    public ForbiddenException(string message, string code = "Forbidden")
        : base(code, message, 403)
    {
    }
}
