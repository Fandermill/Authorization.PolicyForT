using Authorization.PolicyForT;
using Authorization.PolicyForT.Context;

namespace ApplicationLayerLibA.Security;

public class PrincipalProvider : IPrincipalProvider
{
    public Task<IPrincipal?> GetPrincipal()
    {
        return Task.FromResult((IPrincipal?)new LibAUser(1, "TestUser", Permission.CanCreateCustomer));
    }
}
