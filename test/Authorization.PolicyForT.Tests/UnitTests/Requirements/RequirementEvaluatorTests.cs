using Authorization.PolicyForT.Context;
using Authorization.PolicyForT.Requirements;
using Authorization.PolicyForT.Tests.UnitTests.Helpers;
using FluentAssertions;
using Moq;

namespace Authorization.PolicyForT.Tests.UnitTests.Requirements;

public class RequirementEvaluatorTests
{
    private class TestRequirement : IRequirement { }
    private class Tee { }

    [Fact]
    public async Task Evaluating_at_least_one_successfull_handler_results_in_success()
    {
        var requirement = new TestRequirement();
        var failingHandler = new FixedResultRequirementHandlerInvokerSpy(false);
        var succeedingHandler = new FixedResultRequirementHandlerInvokerSpy(true);
        var requirementHandlerProvider = new Mock<IRequirementHandlerProvider>();
        requirementHandlerProvider
            .Setup(p => p.GetHandlers<Tee>(requirement))
            .Returns(new IRequirementHandlerInvoker[] { failingHandler, succeedingHandler });
        var context = new AuthorizationContext<Tee>(new Tee(), new DummyPrincipal());
        var evaluator = new RequirementEvaluator<Tee>(requirementHandlerProvider.Object);

        var result = await evaluator.Evaluate(context, requirement);

        result.IsAuthorized.Should().BeTrue();
        failingHandler.InvokeCount.Should().Be(1);
        succeedingHandler.InvokeCount.Should().Be(1);
    }
}
