using Authorization.PolicyForT.Context;
using Authorization.PolicyForT.Requirements;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Authorization.PolicyForT.Extensions.DependencyInjection;

internal class Registrar
{
    #region Singleton instance and disposal
    private static object _lock = new object();
    private static Registrar? _registrar = new Registrar();
    internal static Registrar Instance => _registrar
        ?? throw new ObjectDisposedException("The registrar object is already disposed");
    private void Dispose()
    {
        lock (_lock)
        {
            _registrar = null;
        }
    }
    #endregion

    private HashSet<Assembly> _assemblies = new();
    private HashSet<Type> _requirementTypes = new();
    private HashSet<Type> _teeTypes = new();

    private Registrar()
    {
        _assemblies.Add(typeof(IAuthorizer<>).Assembly);
    }

    internal void AddAssemblies(IEnumerable<Assembly> assemblies)
    {
        lock (_lock)
        {
            foreach (var assembly in assemblies)
                _assemblies.Add(assembly);
        }
    }

    internal void AddTBase<TBase>() { AddTBase(typeof(TBase)); }
    internal void AddTBase(Type teeBase)
    {
        lock (_lock)
        {
            _teeTypes.Add(teeBase);
        }
    }

    internal void RegisterWithContainer(IServiceCollection services)
    {
        // TODO - Hebben we de Tee Types eigenlijk wel nodig?

        var discoverer = new ServiceDiscoverer(new TypeCollection(_assemblies.ToArray()), _teeTypes.ToArray());

        foreach (var requirementHandler in discoverer.DiscoverRequirementHandlers())
        {
            services.Add(requirementHandler);
        }

        foreach (var policy in discoverer.DiscoverPolicies())
        {
            services.Add(policy);
        }

        var specificContextFactories = discoverer.DiscoverSpecificContextFactories();
        if(specificContextFactories.Any())
        {
            services.TryAddScoped<IAuthorizationContextFactory, PerTeeAuthorizationContextFactory>();
            foreach (var specificContextFactory in specificContextFactories)
                services.Add(specificContextFactory);
        } else
        {
            services.TryAddScoped<IAuthorizationContextFactory, DefaultAuthorizationContextFactory>();
        }


        foreach (var principalProvider in discoverer.DiscoverPrincipalProviders())
        {
            services.Add(principalProvider);
        }


        services.TryAddSingleton(typeof(IRequirementEvaluator<>), typeof(RequirementEvaluator<>));
        services.TryAddSingleton<IRequirementHandlerProvider, RequirementHandlerProvider>();
        services.TryAddScoped(typeof(IAuthorizer<>), typeof(Authorizer<>));
    }
}