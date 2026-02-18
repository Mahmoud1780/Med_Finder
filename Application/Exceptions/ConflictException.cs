namespace MedicineFinder.Application.Exceptions;

public class ConflictException : ApiException
{
    public ConflictException(string message, string code = "Conflict")
        : base(code, message, 409)
    {
    }
}
