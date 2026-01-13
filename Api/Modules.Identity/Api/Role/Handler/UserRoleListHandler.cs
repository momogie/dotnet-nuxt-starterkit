//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Modules.Identity.Entities;
//using Modules.Identity.Entities.Views;

//namespace Modules.Identity.Api.Role.Handler;

//[Authorize]
//[Post("/api/identity/role/list")]
//public class UserListHandler(AppDbContext appDb, [FromBody] RequestParameter parameter) : CommandHandler
//{
//    public override DataResult<RoleView> Response()
//    {
//        return appDb.Views.Filter<RoleView>(parameter);
//    }
//}