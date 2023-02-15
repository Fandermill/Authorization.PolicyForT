using Authorization.PolicyForT.Exceptions;
using Authorization.PolicyForT.Requirements;
using System.Collections.Concurrent;

namespace Authorization.PolicyForT.Context;

public class AuthorizationContext<T>
{
    private readonly ConcurrentDictionary<IRequirement, AuthorizationResult> _checkedRequirements;
    internal IEnumerable<AuthorizationResult> RequirementResults => _checkedRequirements.Values;

    public T Tee { get; private set; }

    private readonly IPrincipal? _principal;

    public AuthorizationContext(T tee)
    {
        _checkedRequirements = new();
        Tee = tee;
    }
    public AuthorizationContext(T tee, IPrincipal? principal) : this(tee)
    {
        _principal = principal;
    }

    public bool IsAuthenticated => _principal != null;

    public TPrincipal Principal<TPrincipal>() where TPrincipal : class, IPrincipal
    {
        if (!IsAuthenticated)
            throw new NotAuthenticatedException("No principal set in context");

        return _principal as TPrincipal
            ?? throw new ArgumentException(
                $"The user type in the context ({_principal!.GetType().Name}) " +
                $"mismatches the tee user type ({typeof(TPrincipal).Name})");
    }

    public AuthorizationResult Fulfil(IRequirement requirement)
    {
        return AddRequirementResult(requirement, requirement.Succeeded());
    }

    public AuthorizationResult Fail(IRequirement requirement)
    {
        return AddRequirementResult(requirement, requirement.Failed());
    }

    public AuthorizationResult Fail(IRequirement requirement, string message)
    {
        return AddRequirementResult(requirement, requirement.Failed(message));
    }

    private AuthorizationResult AddRequirementResult(IRequirement requirement, AuthorizationResult result)
    {
        _checkedRequirements.AddOrUpdate(requirement, result, (key, oldValue) => result);
        return result;
    }
}
