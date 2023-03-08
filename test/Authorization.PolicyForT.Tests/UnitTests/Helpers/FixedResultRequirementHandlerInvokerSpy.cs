using Authorization.PolicyForT.Context;
using Authorization.PolicyForT.Requirements;

namespace Authorization.PolicyForT.Tests.UnitTests.Helpers;

internal class FixedResultRequirementHandlerInvokerSpy : IRequirementHandlerInvoker
{
    internal bool Succeeds { get; private init; }
    internal int InvokeCount { get; private set; }

    public FixedResultRequirementHandlerInvokerSpy(bool succeeds)
    {
        Succeeds = succeeds;
    }

    public Task<AuthorizationResult> Invoke<T>(AuthorizationContext<T> context, IRequirement requirement, CancellationToken cancellationToken)
    {
        InvokeCount++;

        var result = Succeeds
            ? context.Fulfil(requirement)
            : context.Fail(requirement, "Fixed failing result by handler invoker");

        return Task.FromResult(result);
    }
}
