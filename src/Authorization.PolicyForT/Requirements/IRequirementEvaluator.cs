namespace Authorization.PolicyForT.Requirements;

public interface IRequirementEvaluator<T>
{
    Task<AuthorizationResult> Execute(
        AuthorizationContext<T> context,
        IRequirement requirement,
        CancellationToken cancellationToken);
}

public class RequirementEvaluator<T> : IRequirementEvaluator<T>
{
    // TODO

    // I want to request a collection of some kind of object here that
    // encapsulates the actual call to the handler, making it TRequirement
    // agnostic. This class only iterates and gets the result objects.

    public Task<AuthorizationResult> Execute(AuthorizationContext<T> context, IRequirement requirement, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}