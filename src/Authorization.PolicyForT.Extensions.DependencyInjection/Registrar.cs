using Authorization.PolicyForT.Requirements;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Authorization.PolicyForT.Extensions.DependencyInjection;

internal class Registrar
{
    private Assembly[] _assemblies; // todo

    public Registrar(object someOptions)
    {
        // opties; per tee of global(base) context factory
        // opties; welke assemblies, welke namespace, welke base type for tee

        // ALWAYS register policyfort assembly itself? .TryAdd
    }

    private Type[] GetAllRequirementImplementations()
    {
        var requirementType = typeof(IRequirement);
        return GetAllImplementationsOfInterface(requirementType);
    }

    private Type[] GetAllImplementationsOfInterface(Type interfaceType)
    {
        return _assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => 
                interfaceType.IsAssignableFrom(t)
                && !t.IsInterface && !t.IsAbstract)
            .ToArray();
    }
}