namespace Invensa.Domain.Exceptions;

public class InfrastructureException : Exception
{
    public InfrastructureException()
    {
    }

    public InfrastructureException(string message) : base(message)
    {
    }
}