using Authorization.PolicyForT.Context;
using Authorization.PolicyForT.Requirements;

namespace Authorization.PolicyForT.Tests.ServiceRegistration.Models;

public class RequirementA : IRequirement
{
    public class Handler<T> : IRequirementHandler<T, RequirementA> where T : ITee
    {
        public Task<AuthorizationResult> Handle(AuthorizationContext<T> context, RequirementA requirement, CancellationToken cancellationToken)
        {
            return Task.FromResult(context.Fail(requirement, "Testing requirement"));
        }
    }
}
