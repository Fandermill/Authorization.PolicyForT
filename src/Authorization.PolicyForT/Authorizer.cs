using Authorization.PolicyForT.Context;
using Authorization.PolicyForT.Requirements;

namespace Authorization.PolicyForT;

public sealed class Authorizer<T> : IAuthorizer<T>
{
	private readonly IAuthorizationContextFactory _contextFactory;
	private readonly IEnumerable<IPolicy<T>> _policies;
	private IRequirementEvaluator<T> _evaluator;

	public Authorizer(
		IAuthorizationContextFactory contextFactory,
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
			return new AuthorizationResult(true);

		var context = await _contextFactory.CreateNewContext(tee);

		var results = new List<AuthorizationResult>(_policies.Count());
		foreach (var policy in _policies)
		{
			var result = await _evaluator.Evaluate(context, policy.Requirements, cancellationToken);

			results.Add(result);

			if (result.IsAuthorized)
				break;
        }

		foreach(var checkedRequirement in context.RequirementResults)
		{
			System.Diagnostics.Debug.WriteLine(checkedRequirement.ToString());
		}

		return AuthorizationResult.Merge(results);
	}
}
