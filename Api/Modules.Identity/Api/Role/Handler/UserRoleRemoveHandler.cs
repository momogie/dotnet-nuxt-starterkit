//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Modules.Identity.Entities;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

//namespace Modules.Identity.Api.User.Handler;

//[Authorize]
//[Delete("/api/identity/role/remove")]
//public class RoleRemoveHandler(AppDbContext appDb, [FromQuery] int id) : CommandHandler
//{
//    protected Entities.DbSchemas.Role Data { get; set; }

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
//        appDb.Roles.Remove(Data);
//        appDb.SaveChanges();
//    }

//    public override Entities.DbSchemas.Role Response() => Data;
//}

