namespace Authorization.PolicyForT.Context;

public interface IPrincipalProvider
{
    Task<IPrincipal?> GetPrincipal();
}
