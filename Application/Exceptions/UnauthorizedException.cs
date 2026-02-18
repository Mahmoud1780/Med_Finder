namespace MedicineFinder.Application.Exceptions;

public class UnauthorizedException : ApiException
{
    public UnauthorizedException(string message, string code = "Unauthorized")
        : base(code, message, 401)
    {
    }
}
