using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Authorization.PolicyForT.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddPolicyForT<TBase>(this IServiceCollection services, Assembly assembly)
    {
        // authcontextfactory<T>, stateless, singleton




        var registrar = new Registrar(new { someOptions = "yes" });
        //registrar.RegisterBaseTees
        //registrar.RegisterPolicies
        //registrar.RegisterRequirementHandlers
        //etc etc

        // register with TryAdd so we can simply run the command again for
        // different assemblies and types and prevent duplicates
        throw new NotImplementedException();
    }

    public static IServiceCollection AddPolicyForT<TBase>(this IServiceCollection services, Assembly assembly, string inNamespace)
    {
        throw new NotImplementedException();
    }
}