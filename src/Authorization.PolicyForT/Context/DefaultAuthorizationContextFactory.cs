namespace Authorization.PolicyForT.Context;

public class DefaultAuthorizationContextFactory : IAuthorizationContextFactory
{
    private readonly IPrincipalProvider _principalProvider;

    public DefaultAuthorizationContextFactory(IPrincipalProvider principalProvider)
    {
        _principalProvider = principalProvider;
    }

    public virtual async Task<AuthorizationContext<T>> CreateNewContext<T>(T tee)
    {
        var principal = await _principalProvider.GetPrincipal();
        return new AuthorizationContext<T>(tee, principal);
    }
}
