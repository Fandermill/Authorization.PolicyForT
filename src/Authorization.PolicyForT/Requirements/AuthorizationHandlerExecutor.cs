using System.Collections.Concurrent;

namespace Authorization.PolicyForT.Requirements;

public class AuthorizationHandlerExecutor<T> : IRequirementEvaluator<T>
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly ConcurrentDictionary<object, object> _cachedTypes = new(); // todo

    public AuthorizationHandlerExecutor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<AuthorizationResult> Execute(
        AuthorizationContext<T> context,
        IRequirement requirement,
        CancellationToken cancellationToken)
    {
        // nog zonder T !
        var requirementType = requirement.GetType();
        var handlerType = typeof(IRequirementHandler<,>).MakeGenericType(typeof(T), requirementType);

        var methodInfo = handlerType.GetMethod(nameof(IRequirementHandler<T, IRequirement>.Handle));
        if (methodInfo is null)
            throw new InvalidOperationException($"There is no handle method on the handler type for requirement {requirementType.Name}");

        var handlers = _serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(handlerType)) as IEnumerable<object>;
        if (handlers is null)
            throw new InvalidOperationException($"No IAuthorizationHandler found for requirement of type {requirementType.Name}");

        

        var results = new List<AuthorizationResult>(handlers.Count());

        foreach (var handler in handlers)
        {
            var handlerTask = methodInfo.Invoke(handler, new object[] { context, requirement, cancellationToken }) as Task<AuthorizationResult>;
            if (handlerTask is null) throw new InvalidOperationException($"Invoked handler did not return a {nameof(Task<AuthorizationResult>)}");

            var result = await handlerTask;

            // return when any handler succeeds authorizing
            if (result.IsAuthorized)
                return result;

            results.Add(result);
        }

        return AuthorizationResult.Merge(results);
    }
}