using System.Reflection;

namespace Authorization.PolicyForT.Extensions.DependencyInjection;

public class TypeCollection
{
    private Type[] _types;

    public TypeCollection(params Assembly[] assemblies)
    {
        _types = assemblies.SelectMany(a => a.GetTypes()).ToArray();
    }

    public IEnumerable<Type> GetAssignableTypesFrom(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return _types.Where(t => t.IsAssignableTo(type));
    }

    public IEnumerable<Type> FindImplementationsOf(Type interfaceType)
    {
        ArgumentNullException.ThrowIfNull(interfaceType);
        if (!interfaceType.IsInterface)
            throw new ArgumentException("Given type is not an interface", nameof(interfaceType));

        if (interfaceType.IsGenericType)
            return FindImplementationsOfOpenGenericType(interfaceType);

        var implementations = _types.Where(t =>
                interfaceType.IsAssignableFrom(t)
                && !t.IsInterface && !t.IsAbstract);
        return implementations.ToArray();
    }
    // https://stackoverflow.com/questions/8645430/get-all-types-implementing-specific-open-generic-type

    public IEnumerable<Type> FindImplementationsOfOpenGenericType(Type openGenericType)
    {
        if (!openGenericType.IsGenericType)
            throw new ArgumentException("Given type is not an open generic type", nameof(openGenericType));

        bool IsTypeAssignableFromOpenInterface(Type openGenericType, Type interfaceType)
        {
            if(interfaceType.IsGenericType)
            {
                var genericTypeDef = interfaceType.GetGenericTypeDefinition();
                return openGenericType.IsAssignableFrom(genericTypeDef);
            }
            return false;
        }

        var a = _types.Where(t => !t.IsInterface && !t.IsAbstract &&
            
            (t.IsGenericType && openGenericType.IsAssignableFrom(t.GetGenericTypeDefinition()) ||

            t.GetInterfaces().Any(it => IsTypeAssignableFromOpenInterface(openGenericType, it))));

        return a;
        // tweede nodig?
         //   ;
         //   return from x in _types
         //          from z in x.GetInterfaces()
         //          let y = x.BaseType
         //          where
         //          (y != null && y.IsGenericType &&
         //          openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition())) ||
         //          (z.IsGenericType &&
         //          openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()))
         //          select x;
         //
    }

    //public IEnumerable<Type> FindSubClassesOf(Type baseType)
    //{
    //    ArgumentNullException.ThrowIfNull(baseType);
    //
    //    var implementations = _types.Where(t =>
    //        t.IsSubclassOf(baseType));
    //            interfaceType.IsAssignableFrom(t)
    //            && !t.IsInterface && !t.IsAbstract);
    //    return implementations.ToArray();
    //}


}
