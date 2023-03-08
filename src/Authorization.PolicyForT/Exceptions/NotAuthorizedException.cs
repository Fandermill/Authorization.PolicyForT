namespace Authorization.PolicyForT.Exceptions;

public class NotAuthorizedException : Exception
{
    public AuthorizationResult? AuthorizationResult { get; private set; }

    public NotAuthorizedException() : base() { }
    public NotAuthorizedException(string message) : base(message) { }
    public NotAuthorizedException(AuthorizationResult authorizationResult)
        : this(authorizationResult.Message ?? "Authorization failed")
    {
        AuthorizationResult = authorizationResult;
    }
}