using Authorization.PolicyForT.Context;
using Authorization.PolicyForT.Tests.UnitTests.Helpers;
using FluentAssertions;

namespace Authorization.PolicyForT.Tests.UnitTests.Context;

public class DefaultAuthorizationContextFactoryTests
{
    [Fact]
    public async Task Factory_sets_principal_and_tee_in_context()
    {
        var tee = new object();
        var principalProvider = new DummyPrincipalProvider();
        var sut = new DefaultAuthorizationContextFactory(principalProvider);

        var result = await sut.CreateNewContext(tee);

        result.Principal<DummyPrincipal>().Should().Be(principalProvider.UsedPrincipal);
        result.Tee.Should().Be(tee);
    }
}
