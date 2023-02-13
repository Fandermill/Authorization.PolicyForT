namespace Authorization.PolicyForT.Requirements;

public sealed class RequirementHandlerProvider : IRequirementHandlerProvider
{
    private readonly IServiceProvider _serviceProvider;

    public RequirementHandlerProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /*
     *  TODO - cache the reflection results
     *  TODO - accessability as private as possible
     */

    public IEnumerable<RequirementHandlerInvoker> GetHandlers<T>(IRequirement requirement)
    {
        var requirementType = requirement.GetType();
        var handlerType = typeof(IRequirementHandler<,>).MakeGenericType(typeof(T), requirementType);

        var methodInfo = handlerType.GetMethod(nameof(IRequirementHandler<T, IRequirement>.Handle));
        if (methodInfo is null)
            throw new InvalidOperationException($"There is no handle method on the handler type for requirement {requirementType.Name}");

        var handlers = _serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(handlerType)) as IEnumerable<object>;
        if (handlers is null)
            throw new InvalidOperationException($"No IAuthorizationHandler found for requirement of type {requirementType.Name}");

        var handlerInvokes = new List<RequirementHandlerInvoker>(handlers.Count());
        foreach (var handler in handlers)
            handlerInvokes.Add(new RequirementHandlerInvoker(handler, methodInfo));
        return handlerInvokes;
    }
}
