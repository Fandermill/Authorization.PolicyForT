using Authorization.PolicyForT.Context;
using Authorization.PolicyForT.Tests.UnitTests.Helpers;
using FluentAssertions;
using Moq;

namespace Authorization.PolicyForT.Tests.UnitTests.Context;

public class PerTeeAuthorizationContextFactoryTests
{
    [Fact]
    public async Task Uses_tee_specific_factory_when_registered()
    {
        var tee = new TeeTypeA();
        var principalProvider = new DummyPrincipalProvider();
        var teeSpecificAuthorizationContextFactory = new TeeTypeASpecificAuthorizationContextFactorySpy(principalProvider);
        var teeSpecificContextFactoryType = typeof(IAuthorizationContextFactory<TeeTypeA>);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(p => p.GetService(teeSpecificContextFactoryType))
            .Returns(teeSpecificAuthorizationContextFactory);

        var sut = new PerTeeAuthorizationContextFactory(principalProvider, serviceProvider.Object);

        var result = await sut.CreateNewContext(tee);

        result.Tee.Should().Be(tee);
        result.Principal<DummyPrincipal>().Should().Be(principalProvider.UsedPrincipal);
        teeSpecificAuthorizationContextFactory.CreationCount.Should().Be(1);
    }

    [Fact]
    public async Task Uses_default_factory_when_no_tee_specific_factory_is_registered()
    {
        var tee = new TeeTypeA();
        var principalProvider = new DummyPrincipalProvider();
        var teeSpecificContextFactoryType = typeof(IAuthorizationContextFactory<TeeTypeA>);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(p => p.GetService(teeSpecificContextFactoryType))
            .Returns(null);

        var sut = new PerTeeAuthorizationContextFactory(principalProvider, serviceProvider.Object);

        var result = await sut.CreateNewContext(tee);

        result.Tee.Should().Be(tee);
        result.Principal<DummyPrincipal>().Should().Be(principalProvider.UsedPrincipal);
        serviceProvider.Verify(p => p.GetService(teeSpecificContextFactoryType), Times.Once());
    }



    private class TeeTypeA { }
    private class TeeTypeB { }
    private class TeeTypeASpecificAuthorizationContextFactorySpy : IAuthorizationContextFactory<TeeTypeA>
    {
        private readonly IPrincipalProvider _principalProvider;

        public int CreationCount { get; private set; }

        public TeeTypeASpecificAuthorizationContextFactorySpy(IPrincipalProvider principalProvider)
        {
            _principalProvider = principalProvider;
        }

        async Task<AuthorizationContext<TeeTypeA>> IAuthorizationContextFactory<TeeTypeA>.CreateNewContext(TeeTypeA tee)
        {
            CreationCount++;

            var principal = await _principalProvider.GetPrincipal();
            return new AuthorizationContext<TeeTypeA>(tee, principal);
        }
    }
}
