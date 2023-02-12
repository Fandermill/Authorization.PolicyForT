namespace Authorization.PolicyForT.Requirements;

/// <summary>
/// Interface on which extension methods are attached
/// to enable some level of fluent declaring the
/// requirements of a policy for T.
/// </summary>
/// <typeparam name="T">The type of the object for which this policy applies</typeparam>
public interface IRequirementBuilder<T> { }