using Authorization.PolicyForT.Requirements;

namespace Authorization.PolicyForT;

public class RequestAuthorizer<T> : IRequestAuthorizer<T>
{
	private readonly IAuthorizationContextFactory<T> _contextFactory;
	private readonly IEnumerable<IPolicy<T>> _policies;
	private IRequirementEvaluator<T> _executor;

	public RequestAuthorizer(
		IAuthorizationContextFactory<T> contextFactory,
		IEnumerable<IPolicy<T>> policies,
		IRequirementEvaluator<T> executor)
	{
		_contextFactory = contextFactory;
		_policies = policies;
		_executor = executor;
	}

	public async Task<AuthorizationResult> Authorize(T request, CancellationToken cancellationToken)
	{
		if (!_policies.Any())
			return AuthorizationResult.Success();

		var context = await _contextFactory.CreateNewContext(request);

		var results = new List<AuthorizationResult>(_policies.Count());
		foreach (var policy in _policies)
		{
			var result = await _executor.Execute(context, policy.Requirements, cancellationToken);
			if (result.IsAuthorized)
				return result;
			results.Add(result);
		}

		return AuthorizationResult.Merge(results);
	}
}
