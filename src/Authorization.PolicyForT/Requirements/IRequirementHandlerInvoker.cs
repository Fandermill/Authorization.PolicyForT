using Authorization.PolicyForT.Context;

namespace Authorization.PolicyForT.Requirements;

public interface IRequirementHandlerInvoker
{
    Task<AuthorizationResult> Invoke<T>(
        AuthorizationContext<T> context, IRequirement requirement, CancellationToken cancellationToken);
}
