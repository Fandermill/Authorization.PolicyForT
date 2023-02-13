using Authorization.PolicyForT.Context;

namespace Authorization.PolicyForT.Requirements;

public static class BuilderExtensions
{
	public static IRequirement AllOf<T>(this IRequirementBuilder<T> _, params IRequirement[] requirements)
		=> CreateCollectionRequirement(null, false, true, requirements);

	public static IRequirement AndAllOf<T>(this IRequirement requirement, params IRequirement[] requirements)
		=> CreateCollectionRequirement(requirement, true, true, requirements);

	public static IRequirement OrAllOf<T>(this IRequirement requirement, params IRequirement[] requirements)
		=> CreateCollectionRequirement(requirement, false, true, requirements);

	public static IRequirement AnyOf<T>(this IRequirementBuilder<T> _, params IRequirement[] requirements)
		=> CreateCollectionRequirement(null, false, false, requirements);

	public static IRequirement AndAnyOf<T>(this IRequirement requirement, params IRequirement[] requirements)
		=> CreateCollectionRequirement(requirement, true, false, requirements);

	public static IRequirement OrAnyOf<T>(this IRequirement requirement, params IRequirement[] requirements)
		=> CreateCollectionRequirement(requirement, false, false, requirements);

	private static IRequirement CreateCollectionRequirement(
		IRequirement? baseRequirement, bool requireBase, bool requireCollection,
		params IRequirement[] requirements)
	{
		IRequirement collectionRequirement = requireCollection ? new AllOfRequirement(requirements) : new AnyOfRequirement(requirements);

		if (baseRequirement is null || baseRequirement is EmptyRequirement)
			return collectionRequirement;

		return requireBase
			? new AllOfRequirement(baseRequirement, collectionRequirement)
			: new AnyOfRequirement(baseRequirement, collectionRequirement);
	}


	public static IRequirement Require<T>(this IRequirementBuilder<T> _, Func<AuthorizationContext<T>, AuthorizationResult> requirement)
	{
		// TODO - what about async?

		return new DelegateRequirement<T>(requirement);
	}
}
