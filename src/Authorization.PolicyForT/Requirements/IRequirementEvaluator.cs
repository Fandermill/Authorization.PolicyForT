using Authorization.PolicyForT.Context;

namespace Authorization.PolicyForT.Requirements;

public interface IRequirementEvaluator<T>
{
    Task<AuthorizationResult> Evaluate(AuthorizationContext<T> context, IRequirement requirement, CancellationToken cancellationToken);
}
