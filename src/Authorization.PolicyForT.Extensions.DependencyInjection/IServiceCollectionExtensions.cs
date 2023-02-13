using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Authorization.PolicyForT.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddPolicyForT<TBase>(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services.AddPolicyForT<TBase>(false, assemblies);
    }

    public static IServiceCollection AddPolicyForT<TBase>(
        this IServiceCollection services,
        bool isChain, params Assembly[] assemblies)
    {
        return services.AddPolicyForT(new[] { typeof(TBase) }, isChain, assemblies);
    }

    public static IServiceCollection AddPolicyForT(
        this IServiceCollection services,
        IEnumerable<Type> baseTypesOfT,
        bool isChain, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
            assemblies = new Assembly[] { Assembly.GetCallingAssembly() };

        Registrar.Instance.AddAssemblies(assemblies);
        foreach(var baseTypeOfT in baseTypesOfT)
            Registrar.Instance.AddTBase(baseTypeOfT);

        if (!isChain)
            Registrar.Instance.RegisterWithContainer(services);

        return services;
    }
}