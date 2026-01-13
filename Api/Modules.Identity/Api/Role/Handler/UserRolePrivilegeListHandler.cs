//using Modules.Identity.Api.Role.Command;
//using PropHub.Entities;

//namespace Modules.Identity.Api.Role.Handler;

//[CommandHandler("/api/user/role/privilege-list", CommandMethod.Get, true)]
//public class UserRolePrivilegeListHandler : BaseCommandHandler<List<UserRolePrivilegeCommand>>
//{
//    [CommandPipeline]
//    public void Load()
//    {
//        Result = Privileges.PrivilegeList;
//    }
//}
