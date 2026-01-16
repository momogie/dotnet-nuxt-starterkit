using Modules.Logger;

namespace Modules.Identity.Api.Role;

public class RoleLog : IDataLog
{
    public string Name { get; set; }

    public List<RoleClaimLog> Claims { get; set; }
}

public class RoleClaimLog
{
    public string Type { get; set; }
    public string Value { get; set; }
}
