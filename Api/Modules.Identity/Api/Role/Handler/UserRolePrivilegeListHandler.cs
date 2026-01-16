using Modules.Identity.Api.Role.Command;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Modules.Identity.Api.Role.Handler;

//[Authorize]
[Get("/api/identity/role/permissions")]
public class UserRolePrivilegeListHandler : CommandHandler
{
    public override List<AppModule> Response() => Configuration.Modules;
}
