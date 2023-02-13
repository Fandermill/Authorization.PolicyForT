namespace Authorization.PolicyForT;

public interface IAuthorizationContextFactory<T>
{
	Task<AuthorizationContext<T>> CreateNewContext(T tee);
}
