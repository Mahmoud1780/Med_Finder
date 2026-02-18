namespace MedicineFinder.Application.Exceptions;

public class NotFoundException : ApiException
{
    public NotFoundException(string message, string code = "NotFound")
        : base(code, message, 404)
    {
    }
}
