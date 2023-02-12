namespace Authorization.PolicyForT;

public interface IRequestAuthorizer<T>
{
	Task<AuthorizationResult> Authorize(T request, CancellationToken cancellationToken);
}
