using Authorization.PolicyForT.Context;
using Authorization.PolicyForT.Requirements;

namespace Authorization.PolicyForT.Tests.UnitTests.Requirements;

public class RequirementHandlerProviderTests
{
    [Fact]
    public void todo()
    {
        var handler = new TestRequirementHandler();

        // TODO - Test the RequirementHandlerProvider by setting up IServiceProvider to
        // return some handlers, or null. The Provider should invoke the RequirementHandlerInvoker
        // classes as well (internal constructors).
    }



    private class Tee { }
    private class TestRequirement : IRequirement { }
    private class TestRequirementHandler : IRequirementHandler<Tee, TestRequirement>
    {
        public Task<AuthorizationResult> Handle(AuthorizationContext<Tee> context, TestRequirement requirement, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
