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

        Assert.NotNull(service1);
        Assert.NotNull(service2);
    }
}
