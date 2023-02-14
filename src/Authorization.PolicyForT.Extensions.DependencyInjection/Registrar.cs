using Authorization.PolicyForT.Context;
using Authorization.PolicyForT.Requirements;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Authorization.PolicyForT.Extensions.DependencyInjection;

internal class Registrar
{

    private static object _lock = new object();
    private static Registrar? _registrar = new Registrar();
    internal static Registrar Instance => _registrar
        ?? throw new ObjectDisposedException("The registrar object is already disposed");



    private HashSet<Assembly> _assemblies = new();
    private HashSet<Type> _requirementTypes = new();
    private Dictionary<Type, HashSet<Type>> _teeTypes = new();
    private Dictionary<Type, HashSet<Type>> _policyTypes = new();
    private HashSet<Type> _specificAuthorizationContextFactoryTypesForT = new();
    private HashSet<Type> _principalProviderTypes = new();

    private Registrar()
    {
        _assemblies.Add(typeof(IAuthorizer<>).Assembly);
    }

    internal void AddAssemblies(IEnumerable<Assembly> assemblies)
    {
        lock(_lock)
        {
            foreach (var assembly in assemblies)
                _assemblies.Add(assembly);
        }
    }

    internal void AddTBase<TBase>() { AddTBase(typeof(TBase)); }
    internal void AddTBase(Type baseType)
    {
        lock (_lock)
        {
            if (!_teeTypes.ContainsKey(baseType))
                _teeTypes.Add(baseType, new HashSet<Type>());

            if (!_policyTypes.ContainsKey(baseType))
                _policyTypes.Add(baseType, new HashSet<Type>());
        }
    }


    

    private void DiscoverRequirements()
    {
        foreach (var type in GetAllImplementationsOf(typeof(IRequirement)))
            _requirementTypes.Add(type);
    }

    private void DiscoverRequirementHandlers()
    {
        var requirementHandlerType = typeof(IRequirementHandler<,>); // <T, TRequirement>
        
        // todo
    }

    private void DiscoverTees()
    {
        foreach(var baseTypes in _teeTypes.Keys)
            DiscoverTees(baseTypes);
    }

    private void DiscoverTees(Type baseType)
    {
        foreach (var type in FindAllTeeImplementations(baseType))
            _teeTypes[baseType].Add(type);
    }

    private void DiscoverPolicies()
    {
        foreach(var teeType in _teeTypes.SelectMany(t => t.Value)) // !! Ofcourse, this value does not exist as key in _policyTypes
        {
            var policyTypes = GetAllImplementationsOf(typeof(IPolicy<>).MakeGenericType(teeType));
            foreach (var policyType in policyTypes)
                _policyTypes[teeType].Add(policyType);
        }
    }

    private void DiscoverSpecificAuthorizationContextFactoryTypes()
    {
        var specificTypes = GetAllImplementationsOf(typeof(IAuthorizationContextFactory<>));
        foreach (var type in specificTypes)
            _specificAuthorizationContextFactoryTypesForT.Add(type);
    }

    private void DiscoverPrincipalProviders()
    {
        foreach (var type in GetAllImplementationsOf(typeof(IPrincipalProvider)))
        {
            _principalProviderTypes.Add(type);
        }
    }

    internal void RegisterWithContainer(IServiceCollection services)
    {
        lock (_lock)
        {
            DiscoverRequirements();
            DiscoverRequirementHandlers();
            DiscoverTees();
            DiscoverPolicies();

            DiscoverSpecificAuthorizationContextFactoryTypes();
            DiscoverPrincipalProviders();
        }

        // register requirement handlers
        // ... todo

        // Register policies
        //           (what about generic policies?)
        foreach(var teeType in _policyTypes.Keys)
        {
            var policyInterfaceType = typeof(IPolicy<>).MakeGenericType(teeType);
            foreach (var policyType in _policyTypes[teeType])
                services.TryAddSingleton(policyInterfaceType, policyType);
        }

        // more types to register
        // x AuthoContextFactory
        // x IRequirementEvaluator
        // x IRequirementHandlerProvider
        // x IAuthorizer




        if (_specificAuthorizationContextFactoryTypesForT.Any())
            services.TryAddScoped<IAuthorizationContextFactory, DefaultAuthorizationContextFactory>();
        else
        {
            services.TryAddScoped<IAuthorizationContextFactory, PerTeeAuthorizationContextFactory>();
            foreach(var specificType in _specificAuthorizationContextFactoryTypesForT)
            {
                services.TryAddScoped(typeof(IAuthorizationContextFactory<>), specificType);
            }
        }

        // todo - could be Tee specific, or partly.(bounded context)
        foreach(var principalProviderType in _principalProviderTypes)
            services.TryAddScoped(typeof(IPrincipalProvider), principalProviderType);
        


        services.TryAddSingleton(typeof(IRequirementEvaluator<>), typeof(RequirementEvaluator<>));
        services.TryAddSingleton<IRequirementHandlerProvider, RequirementHandlerProvider>();

        services.TryAddScoped(typeof(IAuthorizer<>), typeof(Authorizer<>));

        Dispose();
    }

    private void Dispose()
    {
        lock (_lock)
        {
            _registrar = null;
        }
    }





    /*internal void RegisterGenericPolicy<TGenericPolicy>()
    {
        var type = typeof(TGenericPolicy);
        var interfaceType = typeof(IPolicy<>);
        if (!type.IsAssignableTo(interfaceType))
            throw new ArgumentException($"Given type {type.Name} cannot be used as an {interfaceType.Name}");

        _services!.AddSingleton(type, interfaceType);
    }*/

    internal void RegisterGenericPolicy(Type genericPolicyType)
    {
        // services.
    }






    private IEnumerable<Type> FindAllTeeImplementations(Type baseType)
    {
        if (!baseType.IsInterface && !baseType.IsAbstract)
            yield return baseType;

        foreach (var type in GetAllImplementationsOf(baseType))
            yield return type;
    }

    private Type[] GetAllImplementationsOf(Type interfaceType)
    {
        return _assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                interfaceType.IsAssignableFrom(t)
                && !t.IsInterface && !t.IsAbstract)
            .ToArray();
    }
}