//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Modules.Identity.Api.Role.Command;
//using Modules.Identity.Entities;

//namespace Modules.Identity.Api.Role.Handler;

//[Authorize]
//[Post("/api/identity/user/role/create")]
//public class UserRoleCreateHandler(AppDbContext appDb, [FromBody] UserRoleCommand command) : CommandHandler
//{
//    protected Entities.DbSchemas.Role Data;

//    [Pipeline(1)]
//    public void Save()
//    {
//        Data = new Entities.DbSchemas.Role
//        {
//            Name = command.Name,
//            Description = command.Description,
//        };

//        appDb.Roles.Add(Data);
//        appDb.SaveChanges();
//    }

//    [Pipeline(2)]
//    public void SavePrivileges()
//    {
//        if (command.Privileges == null)
//            return;

//        //foreach (var r in command.Privileges)
//        //{
//        //    var priv = Privileges.PrivilegeList.FirstOrDefault(p => p.Id == r);

//        //    if (priv == null)
//        //        continue;

//        //    var data = new UserRolePrivilege
//        //    {
//        //        FeatureId = r,
//        //        FeatureGroup = priv.Group,
//        //        RoleId = Result.Id,
//        //    };

//        //    AppDb.UserRolePrivileges.Add(data);
//        //}

//        //AppDb.SaveChanges();
//    }

//    public override Entities.DbSchemas.Role Response() => Data;

//}