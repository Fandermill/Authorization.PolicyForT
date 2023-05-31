using Authorization.PolicyForT.Context;
using Authorization.PolicyForT.Requirements;
using Authorization.PolicyForT.Tests.ServiceRegistration.Models;
using Authorization.PolicyForT.Tests.UnitTests.Helpers;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Authorization.PolicyForT.Tests.UnitTests;

public class AuthorizerTests
{
    [Fact]
    public async Task No_policies_will_result_in_successful_authorization()
    {
        var contextFactory = new Mock<IAuthorizationContextFactory>();
        var policies = new IPolicy<Tee>[0];
        var evaluator = new Mock<IRequirementEvaluator<Tee>>();
        var sut = new Authorizer<Tee>(contextFactory.Object, policies, evaluator.Object);

        var result = await sut.Authorize(new Tee(), default);

        result.IsAuthorized.Should().BeTrue();
    }

    [Fact]
    public async Task Authorizer_succeeds_when_policy_succeeds()
    {
        // TODO - way to cluttery

        var tee = new Tee();
        var context = new AuthorizationContext<Tee>(tee);
        var contextFactory = new Mock<IAuthorizationContextFactory>();
        contextFactory
            .Setup(p => p.CreateNewContext(tee))
            .ReturnsAsync(context);
        var policy = new PolicyA();
        
        var succeedingHandler = new FixedResultRequirementHandlerInvokerSpy(true);
        var requirementHandlerProvider = new Mock<IRequirementHandlerProvider>();
        requirementHandlerProvider
            .Setup(p => p.GetHandlers<Tee>(policy.Requirements))
            .Returns(new IRequirementHandlerInvoker[] { succeedingHandler });
        var evaluator = new RequirementEvaluator<Tee>(requirementHandlerProvider.Object);

        var sut = new Authorizer<Tee>(
            contextFactory.Object,
            new[] { policy },
            evaluator);

        var result = await sut.Authorize(tee, default);

        result.IsAuthorized.Should().BeTrue();
        contextFactory.Verify(p=>p.CreateNewContext(tee), Times.Once());
        requirementHandlerProvider.Verify(p=>p.GetHandlers<Tee>(policy.Requirements), Times.Once());
        //context.RequirementResults.Should().HaveCount(1);
    }

    private class PolicyA : AbstractPolicy<Tee>
    {
        public PolicyA()
        {
            Requirements = new TestRequirement();
        }
    }
}
