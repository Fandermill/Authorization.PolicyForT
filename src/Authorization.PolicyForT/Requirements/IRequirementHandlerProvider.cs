namespace Authorization.PolicyForT.Requirements;

public interface IRequirementHandlerProvider
{
    IEnumerable<IRequirementHandlerInvoker> GetHandlers<T>(IRequirement requirement);
}
