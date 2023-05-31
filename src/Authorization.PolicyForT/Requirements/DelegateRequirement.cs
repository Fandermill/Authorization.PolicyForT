using Authorization.PolicyForT.Context;

namespace Authorization.PolicyForT.Requirements;

//internal sealed class DelegateRequirement<T> : IRequirement
//{
//	internal Func<AuthorizationContext<T>, AuthorizationResult> Evaluator { get; private set; }
//
//	public DelegateRequirement(Func<AuthorizationContext<T>, AuthorizationResult> evaluator)
//	{
//		Evaluator = evaluator;
//	}
//
//	public class Handler : IRequirementHandler<T, DelegateRequirement<T>>
//	{
//		Task<AuthorizationResult> IRequirementHandler<T, DelegateRequirement<T>>.Handle(
//			AuthorizationContext<T> context, DelegateRequirement<T> requirement, CancellationToken cancellationToken)
//		{
//			return Task.FromResult(requirement.Evaluator(context));
//		}
//	}
//}
