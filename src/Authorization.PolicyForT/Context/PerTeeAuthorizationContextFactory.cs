namespace Authorization.PolicyForT.Context;

public class PerTeeAuthorizationContextFactory : DefaultAuthorizationContextFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PerTeeAuthorizationContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override Task<AuthorizationContext<T>> CreateNewContext<T>(T tee)
    {
        var factory = _serviceProvider.GetService(typeof(IAuthorizationContextFactory<T>)) as IAuthorizationContextFactory<T>;
        if (factory is null)
        {
            // TODO cache the service fetch

            return base.CreateNewContext(tee);

            //throw new InvalidOperationException($"No registered {nameof(IAuthorizationContextFactory)} found for type {typeof(T).Name}");
        }

        return factory.CreateNewContext(tee);
    }
}
