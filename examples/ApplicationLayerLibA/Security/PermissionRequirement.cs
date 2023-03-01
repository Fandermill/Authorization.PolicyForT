using Authorization.PolicyForT;
using Authorization.PolicyForT.Context;
using Authorization.PolicyForT.Requirements;

namespace ApplicationLayerLibA.Security;

public class PermissionRequirement : IRequirement
{
    internal Permission[] Permissions { get; private set; }

    internal PermissionRequirement(params Permission[] permissions)
    {
        Permissions = permissions;
    }

    public class Handler<T> : IRequirementHandler<T, PermissionRequirement> where T : MediatR.IBaseRequest
    {
        public Task<AuthorizationResult> Handle(AuthorizationContext<T> context, PermissionRequirement requirement, CancellationToken cancellationToken)
        {
            AuthorizationResult result;
            if (!context.IsAuthenticated) result = requirement.Failed("Not authenticated");
            else
            {
                var principalPermissions = context.Principal<LibAUser>().Permissions;
                var missingPermissions = requirement.Permissions.Where(p => !principalPermissions.Contains(p));
                if (!missingPermissions.Any())
                    result = requirement.Succeeded();
                else
                    result = requirement.Failed(
                        $"Principal lacks permissions: {string.Join(", ", missingPermissions.Select(p => p.ToString()))}");
            }

            return Task.FromResult(result);
        }
    }
}



internal static class IRequirementBuilderExtensions
{
    internal static PermissionRequirement Permissions<T>(this IRequirementBuilder<T> _, params Permission[] permissions)
    {
        return new PermissionRequirement(permissions);
    }
}
