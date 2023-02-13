namespace Authorization.PolicyForT.Context;

public class DefaultAuthorizationContextFactory : IAuthorizationContextFactory
{
    public virtual Task<AuthorizationContext<T>> CreateNewContext<T>(T tee)
    {
        return Task.FromResult(new AuthorizationContext<T>(tee));
    }
}
