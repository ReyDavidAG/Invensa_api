namespace Invensa.Domain.Exceptions;

public class UnAuthorizedException : Exception
{
    public UnAuthorizedException()
    {
    }

    public UnAuthorizedException(string message) : base(message)
    {
    }
}