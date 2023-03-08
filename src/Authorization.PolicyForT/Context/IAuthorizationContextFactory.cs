namespace Authorization.PolicyForT.Context;

public interface IAuthorizationContextFactory
{
    Task<AuthorizationContext<T>> CreateNewContext<T>(T tee);
}

public interface IAuthorizationContextFactory<T>
{
    Task<AuthorizationContext<T>> CreateNewContext(T tee);
}

/*public abstract class AbstractAuthorizationContextFactory<T> : IAuthorizationContextFactory<T>
{
    public abstract Task<AuthorizationContext<T>> CreateNewContext(T tee);

    public sealed async Task<AuthorizationContext<TTee>> CreateNewContext<TTee>(TTee tee)
    {
        if (tee is T t)
            return await CreateNewContext(t);
        throw new ArgumentException($"Given object is not of type {typeof(T).Name}", nameof(tee));
       
    }
}
*/