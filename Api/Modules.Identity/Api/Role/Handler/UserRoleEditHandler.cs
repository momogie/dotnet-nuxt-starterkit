//using Microsoft.AspNetCore.Http;
//using Modules.Identity.Api.Role.Command;
//using Modules.Identity.Entities;

//namespace Modules.Identity.Api.Role.Handler;

//[Authorize]
//[Patch("/api/identity/role/edit")]
//public class UserEditHandler(AppDbContext appDb, [FromQuery] int id, [FromBody] UserRoleCommand command) : CommandHandler
//{
//    protected Entities.DbSchemas.Role Data;

//    public override async Task<IResult> Validate()
//    {
//        Data = appDb.Roles.FirstOrDefault(p => p.Id == id);
//        if (Data == null)
//            return NotFound();

//        return await Next();
//    }

//    [Pipeline(1)]
//    public void Save()
//    {
//        Data.Name = command.Name;
//        Data.Description = command.Description;
//        appDb.SaveChanges();
//    }


//    [Pipeline(1)]
//    public void SavePrivileges()
//    {
//        //AppDb.UserRolePrivileges.RemoveRange(p => p.RoleId == Parameter.Id);
//        //AppDb.SaveChanges();

//        //if (Command.Privileges == null)
//        //    return;

//        //foreach (var r in Command.Privileges)
//        //{
//        //    var priv = Privileges.PrivilegeList.FirstOrDefault(p => p.Id == r);
//        //    if (priv == null)
//        //        continue;

//        //    var data = new UserRolePrivilege
//        //    {
//        //        FeatureId = r,
//        //        FeatureGroup = priv.Group,
//        //        RoleId = Parameter.Id,
//        //    };
//        //    appDb.UserRolePrivileges.Add(data);
//        //}

//        //appDb.SaveChanges();
//    }
//    public override Entities.DbSchemas.Role Response() => Data;
//}
