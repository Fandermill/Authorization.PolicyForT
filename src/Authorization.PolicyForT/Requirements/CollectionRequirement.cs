using Authorization.PolicyForT.Context;

namespace Authorization.PolicyForT.Requirements;

public abstract class CollectionRequirement : IRequirement
{
	internal IEnumerable<IRequirement> Requirements { get; init; }

	public CollectionRequirement(IEnumerable<IRequirement> requirements)
	{
		Requirements = requirements;
	}



	public abstract class Handler<T, TCollectionRequirement> : IRequirementHandler<T, TCollectionRequirement>
		where TCollectionRequirement : CollectionRequirement
	{
		private IRequirementEvaluator<T> _evaluator;
		private bool _requireAll;

		public Handler(IRequirementEvaluator<T> evaluator, bool requireAll)
		{
			_evaluator = evaluator;
			_requireAll = requireAll;
		}

		public async Task<AuthorizationResult> Handle(
			AuthorizationContext<T> context,
			TCollectionRequirement requirement,
			CancellationToken cancellationToken)
		{
			if (!requirement.Requirements.Any())
				return requirement.Failed("No requirements in collectino");

			var results = new List<AuthorizationResult>(requirement.Requirements.Count());
			foreach (var innerRequirement in requirement.Requirements)
			{
				var result = await _evaluator.Evaluate(context, innerRequirement, cancellationToken);
				if (!_requireAll && result.IsAuthorized)
					return result;
				results.Add(result);
			}

			return AuthorizationResult.Merge(results);
		}
	}
}

public class AllOfRequirement : CollectionRequirement
{
	public AllOfRequirement(params IRequirement[] requirements) : base(requirements) { }



	public sealed class Handler<T> : Handler<T, AllOfRequirement>
	{
		public Handler(IRequirementEvaluator<T> executor) : base(executor, true) { }
	}
}

public class AnyOfRequirement : CollectionRequirement
{
	public AnyOfRequirement(params IRequirement[] requirements) : base(requirements) { }


	public sealed class Handler<T> : Handler<T, AnyOfRequirement>
	{
		public Handler(IRequirementEvaluator<T> executor) : base(executor, false) { }
	}
}