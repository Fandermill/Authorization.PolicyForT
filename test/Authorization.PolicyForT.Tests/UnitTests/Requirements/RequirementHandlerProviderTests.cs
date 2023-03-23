using Authorization.PolicyForT.Context;
using Authorization.PolicyForT.Requirements;
using Authorization.PolicyForT.Tests.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization.PolicyForT.Tests.UnitTests.Requirements;

public class RequirementHandlerProviderTests
{
    [Fact]
    public void Requirement_handler_provider_returns_matching_handlers()
    {
        var handlerProvider = new RequirementHandlerProvider(CreateServiceProviderWithRequirementHandler());

        var handlers = handlerProvider.GetHandlers<Tee>(new TestRequirement());

        handlers.Should().ContainSingle();
    }

    [Fact]
    public void When_no_matching_requirement_handler_is_found_an_exception_is_thrown()
    {
        var handlerProvider = new RequirementHandlerProvider(CreateEmptyServiceProvider());

        var act = () => handlerProvider.GetHandlers<Tee>(new TestRequirement());

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public async Task Can_invoke_a_handler_from_the_requirement_handler_provider()
    {
        var requirement = new TestRequirement();
        var context = new AuthorizationContext<Tee>(new Tee());
        var handlerProvider = new RequirementHandlerProvider(CreateServiceProviderWithRequirementHandler());
        var handler = handlerProvider.GetHandlers<Tee>(new TestRequirement()).First();

        var result = await handler.Invoke(context, requirement, default);

        result.IsAuthorized.Should().BeTrue();
    }


    private IServiceProvider CreateEmptyServiceProvider()
    {
        return new ServiceCollection().BuildServiceProvider();
    }

    private IServiceProvider CreateServiceProviderWithRequirementHandler()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<IRequirementHandler<Tee, TestRequirement>, TestRequirementHandler>();
        return serviceCollection.BuildServiceProvider();
    }

    private class TestRequirementHandler : IRequirementHandler<Tee, TestRequirement>
    {
        public Task<AuthorizationResult> Handle(AuthorizationContext<Tee> context, TestRequirement requirement, CancellationToken cancellationToken)
        {
            return Task.FromResult(context.Fulfil(requirement));
        }
    }
}
