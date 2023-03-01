using Authorization.PolicyForT.Extensions.DependencyInjection;
using Authorization.PolicyForT.Requirements;
using Authorization.PolicyForT.Tests.ServiceRegistration.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization.PolicyForT.Tests.ServiceRegistration;

public class ServiceCollectionTests
{
    [Fact]
    public void Can_register_and_resolve_requirement_handlers()
    {
        var services = new ServiceCollection();

        services.AddSingleton<
            IRequirementHandler<TestCommand, RequirementA>,
            RequirementA.Handler<TestCommand>>();

        var provider = services.BuildServiceProvider();

        var requiredServiceType = typeof(IRequirementHandler<,>)
            .MakeGenericType(typeof(TestCommand), typeof(RequirementA));
        var service = provider.GetRequiredService(requiredServiceType);

        Assert.NotNull(service);
    }

    [Fact]
    public void Can_register_and_resolve_enumerable_of_requirement_handlers()
    {
        var services = new ServiceCollection();

        //new ServiceDescriptor()
        //services.AddSingleton<
        //    IRequirementHandler<TestCommand, RequirementA>,
        //    RequirementA.Handler<TestCommand>>();

        var provider = services.BuildServiceProvider();

        var requiredServiceType = typeof(IRequirementHandler<,>)
            .MakeGenericType(typeof(TestCommand), typeof(RequirementA));

        var collectionOfHandlersType = typeof(IEnumerable<>).MakeGenericType(requiredServiceType);
        var service = provider.GetRequiredService(collectionOfHandlersType) as IEnumerable<object>;

        Assert.NotNull(service);
    }

    [Fact]
    public void Can_use_registrar()
    {
        var services = new ServiceCollection();
        services.AddPolicyForT<ITee>();
        var provider = services.BuildServiceProvider();

        var requiredServiceType = typeof(IRequirementHandler<,>)
            .MakeGenericType(typeof(TestCommand), typeof(RequirementA));
        var service1 = provider.GetRequiredService(requiredServiceType);

        var requiredGenericHandlerType = typeof(IRequirementHandler<,>)
            .MakeGenericType(typeof(TestCommand), typeof(AllOfRequirement));
        var service2 = provider.GetRequiredService(requiredGenericHandlerType);



        var collectionOfHandlersType = typeof(IEnumerable<>).MakeGenericType(requiredServiceType);
        var serviceEnumerable = provider.GetRequiredService(collectionOfHandlersType) as IEnumerable<object>;

        Assert.NotNull(service1);
        Assert.NotNull(service2);

        Assert.NotNull(serviceEnumerable);
    }

    [Fact]
    public void Can_use_requirement_handler_provider()
    {
        var services = new ServiceCollection();
        services.AddPolicyForT<ITee>();
        var provider = services.BuildServiceProvider();

        var requirementHandlerProvider = provider.GetRequiredService<IRequirementHandlerProvider>();
        var handlers = requirementHandlerProvider.GetHandlers<TestCommand>(new RequirementA());

        Assert.Single(handlers);
    }
}
