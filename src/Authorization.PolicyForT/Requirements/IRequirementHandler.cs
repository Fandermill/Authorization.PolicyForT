using Authorization.PolicyForT.Context;

namespace Authorization.PolicyForT.Requirements;

public interface IRequirementHandler<T, TRequirement>
    where TRequirement : IRequirement
{
    string Name => typeof(TRequirement).Name;

    Task<AuthorizationResult> Handle(AuthorizationContext<T> context, TRequirement requirement, CancellationToken cancellationToken);
}