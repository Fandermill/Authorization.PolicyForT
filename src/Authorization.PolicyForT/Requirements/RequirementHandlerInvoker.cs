using System.Reflection;

namespace Authorization.PolicyForT.Requirements;

public class RequirementHandlerInvoker
{
    private object _handler;
    private MethodInfo _method;

    internal RequirementHandlerInvoker(object handler, MethodInfo method)
    {
        _handler = handler;
        _method = method;
    }

    public async Task<AuthorizationResult> Invoke<T>(AuthorizationContext<T> context, IRequirement requirement, CancellationToken cancellationToken = default)
    {
        var handlerTask = _method.Invoke(_handler, new object[] { context, requirement, cancellationToken }) as Task<AuthorizationResult>;
        if (handlerTask is null) throw new InvalidOperationException($"Invoked handler did not return a {nameof(Task<AuthorizationResult>)}");

        return await handlerTask;
    }
}