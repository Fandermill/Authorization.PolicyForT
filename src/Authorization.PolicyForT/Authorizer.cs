using Authorization.PolicyForT.Requirements;

namespace Authorization.PolicyForT;

public class Authorizer<T> : IAuthorizer<T>
{
	private readonly IAuthorizationContextFactory<T> _contextFactory;
	private readonly IEnumerable<IPolicy<T>> _policies;
	private IRequirementEvaluator<T> _evaluator;

	public Authorizer(
		IAuthorizationContextFactory<T> contextFactory,
		IEnumerable<IPolicy<T>> policies,
		IRequirementEvaluator<T> evaluator)
	{
		_contextFactory = contextFactory;
		_policies = policies;
        _evaluator = evaluator;
	}

	public async Task<AuthorizationResult> Authorize(T tee, CancellationToken cancellationToken)
	{
		if (!_policies.Any())
			return AuthorizationResult.Success();

		var context = await _contextFactory.CreateNewContext(tee);

		var results = new List<AuthorizationResult>(_policies.Count());
		foreach (var policy in _policies)
		{
			var result = await _evaluator.Evaluate(context, policy.Requirements, cancellationToken);

			if (result.IsAuthorized)
				return result;

			results.Add(result);
		}

		return AuthorizationResult.Merge(results);
	}
}
