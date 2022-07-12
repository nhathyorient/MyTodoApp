namespace Todos.Domain.Exception;

public class UnauthorizedDomainException : DomainException
{
    public UnauthorizedDomainException(string errorMessage) : base(errorMessage)
    {
    }
}