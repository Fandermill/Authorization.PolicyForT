using Authorization.PolicyForT.Exceptions;
using Authorization.PolicyForT.Requirements;
using System.Collections.Concurrent;

namespace Authorization.PolicyForT;

public class AuthorizationContext<TRequest>
{
    private readonly ConcurrentDictionary<IRequirement, AuthorizationResult> _checkedRequirements;

    public TRequest Request { get; private set; }

    private readonly IPrincipal? _principal;

    public AuthorizationContext(TRequest request)
    {
        _checkedRequirements = new();
        Request = request;
    }
    public AuthorizationContext(TRequest request, IPrincipal? principal) : this(request)
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
                $"mismatches the request user type ({typeof(TPrincipal).Name})");
    }

    public AuthorizationResult Fulfil(IRequirement requirement)
    {
        return AddRequirementResult(requirement, AuthorizationResult.Success());
    }

    public AuthorizationResult Fail(IRequirement requirement)
    {
        return AddRequirementResult(requirement, AuthorizationResult.Fail());
    }

    public AuthorizationResult Fail(IRequirement requirement, string message)
    {
        return AddRequirementResult(requirement, AuthorizationResult.Fail(message));
    }

    private AuthorizationResult AddRequirementResult(IRequirement requirement, AuthorizationResult result)
    {
        _checkedRequirements.AddOrUpdate(requirement, result, (key, oldValue) => result);
        return result;
    }
}
