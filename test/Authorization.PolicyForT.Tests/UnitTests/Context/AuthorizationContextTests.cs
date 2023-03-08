using Authorization.PolicyForT.Context;
using Authorization.PolicyForT.Exceptions;
using Authorization.PolicyForT.Requirements;
using Authorization.PolicyForT.Tests.UnitTests.Helpers;
using FluentAssertions;

namespace Authorization.PolicyForT.Tests.UnitTests.Context;

public class AuthorizationContextTests
{
    [Fact]
    public void Creating_a_context_sets_tee_and_principal()
    {
        var tee = new object();
        var principal = new DummyPrincipal();
        var context = new AuthorizationContext<object>(tee, principal);

        context.Tee.Should().Be(tee);
        context.Principal<DummyPrincipal>().Should().Be(principal);
    }

    [Fact]
    public void No_principal_means_not_authenticated()
    {
        var tee = new object();

        var context = new AuthorizationContext<object>(tee);
        var act = () => context.Principal<DummyPrincipal>();

        context.IsAuthenticated.Should().BeFalse();
        act.Should().Throw<NotAuthenticatedException>();
    }

    [Fact]
    public void Trying_to_get_principal_with_wrong_type_throws_exception()
    {
        var tee = new object();

        var context = new AuthorizationContext<object>(tee, new DummyPrincipal());
        var act = () => context.Principal<WrongPrincipal>();

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Fulfilling_a_context_results_in_authorized_successfully()
    {
        var tee = new object();
        var context = new AuthorizationContext<object>(tee, new DummyPrincipal());

        var result = context.Fulfil(new DummyRequirement());

        result.IsAuthorized.Should().BeTrue();
    }

    [Fact]
    public void Failing_a_context_results_in_failed_authorization()
    {
        var tee = new object();
        var context = new AuthorizationContext<object>(tee, new DummyPrincipal());

        var result = context.Fail(new DummyRequirement(), "Some fail message");

        result.IsAuthorized.Should().BeFalse();
    }

    private class DummyRequirement : IRequirement { }
    private class WrongPrincipal : IPrincipal { }
}
