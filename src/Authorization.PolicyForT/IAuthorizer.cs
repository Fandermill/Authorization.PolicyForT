namespace Authorization.PolicyForT;

public interface IAuthorizer<T>
{
	Task<AuthorizationResult> Authorize(T tee, CancellationToken cancellationToken);
}
