namespace Authorization.PolicyForT.Requirements;

/// <summary>
/// A policy is a requirement or a collection of requirements... todo... todo
/// </summary>
/// <typeparam name="T">The type of the object for which this policy applies</typeparam>
public interface IPolicy<T>
{
    IRequirement Requirements { get; }
}
