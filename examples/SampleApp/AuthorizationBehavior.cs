using Authorization.PolicyForT;
using Authorization.PolicyForT.Exceptions;
using MediatR;

namespace SampleApp;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IAuthorizer<TRequest> _authorizer;
    private readonly ILogger<AuthorizationBehavior<TRequest, TResponse>> _logger;


    public AuthorizationBehavior(IAuthorizer<TRequest> authorizer, ILogger<AuthorizationBehavior<TRequest, TResponse>> logger)
    {
        _authorizer = authorizer;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Authorizing...");

        var authorizationResult = await _authorizer.Authorize(request, cancellationToken);
        if(authorizationResult.IsAuthorized)
        {
            _logger.LogInformation("Authorized successfully!");
            return await next();
        }

        _logger.LogWarning("Authorization failed: {message}", authorizationResult.Message);
        throw new NotAuthorizedException(authorizationResult);
    }
}

