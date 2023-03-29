using Authorization.PolicyForT.Context;
using Authorization.PolicyForT.Requirements;
using Authorization.PolicyForT.Tests.UnitTests.Helpers;
using FluentAssertions;
using Moq;

namespace Authorization.PolicyForT.Tests.UnitTests.Requirements;

public class CollectionRequirementTests
{
    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    public async Task AllOf_requirement_only_succeeds_when_all_succeed(
        bool requirement1Succeeds, bool requirement2Succeeds, bool expectedResult)
    {
        var requirement1 = new TestRequirement();
        var requirement2 = new TestRequirement();
        var allOfRequirement = new AllOfRequirement(requirement1, requirement2);

        var context = new AuthorizationContext<Tee>(new Tee());
        var evaluatorMock = CreateEvaluatorMock(context, new[] {
            new RequirementEvaluatorResult(requirement1, requirement1Succeeds),
            new RequirementEvaluatorResult(requirement2, requirement2Succeeds)
        });

        var handler = new AllOfRequirement.Handler<Tee>(evaluatorMock.Object);


        var result = await handler.Handle(context, allOfRequirement, default);


        result.IsAuthorized.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(true, false, true)]
    public async Task AnyOf_requirement_succeeds_if_at_least_one_succeeds(
        bool requirement1Succeeds, bool requirement2Succeeds, bool expectedResult)
    {
        var requirement1 = new TestRequirement();
        var requirement2 = new TestRequirement();
        var anyOfRequirement = new AnyOfRequirement(requirement1, requirement2);

        var context = new AuthorizationContext<Tee>(new Tee());
        var evaluatorMock = CreateEvaluatorMock(context, new[] {
            new RequirementEvaluatorResult(requirement1, requirement1Succeeds),
            new RequirementEvaluatorResult(requirement2, requirement2Succeeds)
        });

        var handler = new AnyOfRequirement.Handler<Tee>(evaluatorMock.Object);


        var result = await handler.Handle(context, anyOfRequirement, default);


        result.IsAuthorized.Should().Be(expectedResult);
    }

    private Mock<IRequirementEvaluator<T>> CreateEvaluatorMock<T>(
        AuthorizationContext<T> context,
        IEnumerable<RequirementEvaluatorResult> mappings)
    {
        var evaluatorMock = new Mock<IRequirementEvaluator<T>>();
        foreach (var mapping in mappings)
        {
            evaluatorMock
            .Setup(p => p.Evaluate(context, mapping.Requirement, default))
            .Returns(Task.FromResult(new AuthorizationResult(mapping.EvaluationResult)));
        }
        return evaluatorMock;
    }

    private record RequirementEvaluatorResult(IRequirement Requirement, bool EvaluationResult);
}
