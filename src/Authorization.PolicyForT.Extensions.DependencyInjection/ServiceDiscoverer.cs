using Authorization.PolicyForT.Context;
using Authorization.PolicyForT.Requirements;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Authorization.PolicyForT.Extensions.DependencyInjection;

internal sealed class ServiceDiscoverer
{
    private readonly TypeCollection _typeCollection;
    private readonly HashSet<Type> _teeTypes;

    internal ServiceDiscoverer(TypeCollection typeCollection, Type[] baseTeeTypes)
    {
        _typeCollection = typeCollection;
        _teeTypes = DiscoverTeeTypes(baseTeeTypes);
    }

    internal IEnumerable<ServiceDescriptor> DiscoverRequirementHandlers()
    {
        var handlerInterfaceType = typeof(IRequirementHandler<,>);
        foreach (var handlerType in _typeCollection.FindImplementationsOfOpenGenericType(handlerInterfaceType))
        {
            var closedInterfaceType = handlerType.GetInterfaces().Where(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == handlerInterfaceType).First();

            var requirementType = closedInterfaceType.GenericTypeArguments[1];
            if (requirementType.IsGenericType)
                throw new NotImplementedException("Generic requirement types are not supported");

            // If all open generics are closed, return the type
            if (!handlerType.ContainsGenericParameters)
            {
                yield return new ServiceDescriptor(
                    GetClosedInterfaceOfType(handlerType, handlerInterfaceType),
                    handlerType,
                    ServiceLifetime.Singleton);
            }
            else
            {
                var handlerTypeInfo = handlerType.GetTypeInfo();
                if (handlerTypeInfo.GenericTypeParameters.Length != 1)
                    throw new NotImplementedException("Unable to use requirement handler with no or more than 1 open generic types");

                var openTeeType = handlerTypeInfo.GenericTypeParameters[0];
                if (!openTeeType.IsGenericTypeParameter)
                    throw new NotImplementedException("Unable to register all tee types when tee type is closed");

                foreach(var teeType in _teeTypes)
                {
                    yield return new ServiceDescriptor(
                        handlerInterfaceType.MakeGenericType(teeType, requirementType),
                        handlerType.MakeGenericType(teeType),
                        ServiceLifetime.Singleton);
                }
            }
            // is generic type parameter



            

            //var teeType = handlerType.GenericTypeArguments[0];
            //var requirementType = handlerType.GenericTypeArguments[1];
            //
            //yield return new ServiceDescriptor(
            //    handlerInterfaceType.MakeGenericType(teeType, requirementType),
            //    handlerType,
            //    ServiceLifetime.Scoped);
        }
    }

    internal IEnumerable<ServiceDescriptor> DiscoverPolicies()
    {
        var policyInterfaceType = typeof(IPolicy<>);
        foreach (var policyType in _typeCollection.FindImplementationsOfOpenGenericType(policyInterfaceType))
        {

            yield return new ServiceDescriptor(
                GetClosedInterfaceOfType(policyType, policyInterfaceType),
                policyType,
                ServiceLifetime.Singleton);

            //var teeType = policyType.GenericTypeArguments[0];
            //
            //yield return new ServiceDescriptor(
            //    policyInterfaceType.MakeGenericType(teeType),
            //    policyType,
            //    ServiceLifetime.Singleton);
        }
    }
    internal IEnumerable<ServiceDescriptor> DiscoverSpecificContextFactories()
    {
        var contextFactoryInterfaceType = typeof(IAuthorizationContextFactory<>);
        foreach (var contextFactoryType in _typeCollection.FindImplementationsOfOpenGenericType(contextFactoryInterfaceType))
        {
            yield return new ServiceDescriptor(
                GetClosedInterfaceOfType(contextFactoryType, contextFactoryInterfaceType),
                contextFactoryType,
                ServiceLifetime.Scoped);

            //var teeType = contextFactoryType.GenericTypeArguments[0];
            //
            //yield return new ServiceDescriptor(
            //    contextFactoryInterfaceType.MakeGenericType(teeType),
            //    contextFactoryType,
            //    ServiceLifetime.Scoped);
        }
    }

    internal IEnumerable<ServiceDescriptor> DiscoverPrincipalProviders()
    {
        var principalProviderInterfaceType = typeof(IPrincipalProvider);
        foreach (var principalProviderType in _typeCollection.FindImplementationsOf(principalProviderInterfaceType))
        {
            yield return new ServiceDescriptor(
                principalProviderInterfaceType,
                principalProviderType,
                ServiceLifetime.Scoped);
        }
    }



    private HashSet<Type> DiscoverTeeTypes(Type[] baseTeeTypes)
    {
        return new HashSet<Type>(baseTeeTypes.SelectMany(t => DiscoverTeeTypes(t)));
    }

    private HashSet<Type> DiscoverTeeTypes(Type baseTeeType)
    {
        HashSet<Type> result = new() { baseTeeType };

        var types = _typeCollection.GetAssignableTypesFrom(baseTeeType);

        foreach (var type in types)
            result.Add(type);

        return result;
    }



    private Type GetClosedInterfaceOfType(Type type, Type openInterfaceType)
    {
        return type.GetInterfaces().Where(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == openInterfaceType)
            .First();
    }
}
