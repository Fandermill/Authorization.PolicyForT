﻿namespace Authorization.PolicyForT.Requirements;

public interface IRequirementEvaluator<T>
{
    Task<AuthorizationResult> Evaluate(AuthorizationContext<T> context, IRequirement requirement, CancellationToken cancellationToken);
}

public class RequirementEvaluator<T> : IRequirementEvaluator<T>
{
    private readonly IRequirementHandlerProvider _handlerProvider;

    public RequirementEvaluator(IRequirementHandlerProvider handlerProvider)
    {
        _handlerProvider = handlerProvider;
    }


    public async Task<AuthorizationResult> Evaluate(AuthorizationContext<T> context, IRequirement requirement, CancellationToken cancellationToken = default)
    {
        var handlers = _handlerProvider.GetHandlers<T>(requirement);
        var results = new List<AuthorizationResult>(handlers.Count());

        foreach (var handler in handlers)
        {
            var result = await handler.Invoke(context, requirement, cancellationToken);

            // return when any handler succeeds authorizing
            if (result.IsAuthorized)
                return result;

            results.Add(result);
        }

        return AuthorizationResult.Merge(results);
    }
}