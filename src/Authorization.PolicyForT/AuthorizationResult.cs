namespace Authorization.PolicyForT;

public sealed class AuthorizationResult
{
    public bool IsAuthorized { get; private set; }
    public string? Message { get; private set; }

    private AuthorizationResult(bool isAuthorized)
    {
        IsAuthorized = isAuthorized;
    }
    private AuthorizationResult(bool isAuthorized, string? message) : this(isAuthorized)
    {
        Message = message;
    }

    public static AuthorizationResult Success()
    {
        return new AuthorizationResult(true);
    }

    public static AuthorizationResult Fail()
    {
        return new AuthorizationResult(false);
    }

    public static AuthorizationResult Fail(string message)
    {
        return new AuthorizationResult(false, message);
    }

    public static AuthorizationResult Merge(IEnumerable<AuthorizationResult> results)
    {
        if (results.Any(r => r.IsAuthorized))
            return Success();

        var message = string.Join(Environment.NewLine,
            results.Where(r => !r.IsAuthorized && !string.IsNullOrEmpty(r.Message)));

        if (!string.IsNullOrEmpty(message))
            return Fail(message);
        else
            return Fail();
    }
}
