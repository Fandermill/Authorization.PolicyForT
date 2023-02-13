namespace Authorization.PolicyForT.Requirements;

public interface IRequirementHandlerProvider
{
    IEnumerable<RequirementHandlerInvoker> GetHandlers<T>(IRequirement requirement);
}
