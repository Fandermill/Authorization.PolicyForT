namespace Authorization.PolicyForT.Exceptions;

public class NotAuthenticatedException : Exception
{
    public NotAuthenticatedException(string message) : base(message) { }
}
