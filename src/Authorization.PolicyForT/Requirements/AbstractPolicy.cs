namespace Authorization.PolicyForT.Requirements;

/// <summary>
/// This is a base class for a policy. It contains a handy
/// <c cref="IRequirementBuilder{T}">IRequirementBuilder{T}</c>
/// IRequirementBuilder<typeparamref name="T"/> property 
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class AbstractPolicy<T> : IPolicy<T>
{
    protected IRequirementBuilder<T> Require { get; } = null!;
    public IRequirement Requirements { get; protected set; } = new EmptyRequirement();

    protected AbstractPolicy() { }
}