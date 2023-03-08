using Authorization.PolicyForT.Context;

namespace Authorization.PolicyForT.Tests.UnitTests.Helpers;

internal class DummyPrincipalProvider : IPrincipalProvider
{
    internal DummyPrincipal UsedPrincipal { get; init; } = new DummyPrincipal();

    public Task<IPrincipal?> GetPrincipal()
    {
        return Task.FromResult((IPrincipal?)UsedPrincipal);
    }
}
