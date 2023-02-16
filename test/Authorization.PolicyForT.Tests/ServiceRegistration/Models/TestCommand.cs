using Authorization.PolicyForT.Requirements;

namespace Authorization.PolicyForT.Tests.ServiceRegistration.Models;

public class TestCommand : ITee
{
    public int Id { get; set; }
}

public class TestPolicy : AbstractPolicy<TestCommand>
{
    public TestPolicy() => Requirements = new RequirementA();
}