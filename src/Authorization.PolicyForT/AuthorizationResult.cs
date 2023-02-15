using Authorization.PolicyForT.Requirements;

namespace Authorization.PolicyForT;

public sealed class AuthorizationResult
{
    public bool IsAuthorized { get; private set; }
    public string? Message { get; private set; }

    internal IRequirement? Requirement { get; private set; }

    private AuthorizationResult(bool isAuthorized, string? message, IRequirement? requirement)
    {
        IsAuthorized = isAuthorized;
        if(!string.IsNullOrEmpty(message))
            Message = message;
        Requirement = requirement;
    }

    public AuthorizationResult(IRequirement requirement, bool isAuthorized) : this(isAuthorized, null, requirement) { }
    public AuthorizationResult(IRequirement requirement, bool isAuthorized, string? message) : this(isAuthorized, message, requirement) { }
    public AuthorizationResult(bool isAuthorized) : this(isAuthorized, null, null) { }
    public AuthorizationResult(bool isAuthorized, string? message) : this(isAuthorized, message, null) { }

    public override string ToString()
    {
        return
            "Authorization " +
            (Requirement is not null ? "for requirement " + Requirement.GetType().Name + " " : "") +
            (IsAuthorized ? "SUCCEEDED" : "failed") +
            (Message is null ? "" : ": " + Message);
    }

    public static AuthorizationResult Merge(IEnumerable<AuthorizationResult> results)
    {
        var authorizedRequirement = results.FirstOrDefault(r => r.IsAuthorized);
        if (authorizedRequirement is not null)
            return authorizedRequirement;

        var message = string.Join(Environment.NewLine,
            results.Where(r => !r.IsAuthorized && !string.IsNullOrEmpty(r.Message)));

        return new AuthorizationResult(false, message);
    }
}


public static class IRequirementExtensions
{
    public static AuthorizationResult Succeeded(this IRequirement requirement)
    {
        return new AuthorizationResult(requirement, true);
    }
    public static AuthorizationResult Failed(this IRequirement requirement, string? message = null)
    {
        return new AuthorizationResult(requirement, false, message);
    }
}