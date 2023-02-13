namespace Authorization.PolicyForT.Context;

public interface IAuthorizationContextFactory
{
    Task<AuthorizationContext<T>> CreateNewContext<T>(T tee);
}

public interface IAuthorizationContextFactory<T> : IAuthorizationContextFactory
{
    Task<AuthorizationContext<T>> CreateNewContext(T tee);
}
