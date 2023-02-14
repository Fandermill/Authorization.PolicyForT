using Authorization.PolicyForT;

namespace ApplicationLayerLibA.Security;

public class LibAUser : IPrincipal
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    internal Permission[] Permissions { get; private set; }

    internal LibAUser(int id, string name, params Permission[] permissions)
    {
        Id = id;
        Name = name;
        Permissions = permissions;
    }
}
