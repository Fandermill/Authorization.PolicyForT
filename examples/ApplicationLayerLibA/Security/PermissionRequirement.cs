using Authorization.PolicyForT.Requirements;

namespace ApplicationLayerLibA.Security;

public class PermissionRequirement : IRequirement
{
    internal Permission[] Permissions { get; private set; }

    internal PermissionRequirement(params Permission[] permissions)
    {
        Permissions = permissions;
    }
}



internal static class IRequirementBuilderExtensions
{
    internal static PermissionRequirement Permissions<T>(this IRequirementBuilder<T> _, params Permission[] permissions)
    {
        return new PermissionRequirement(permissions);
    }
}
