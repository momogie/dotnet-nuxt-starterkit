//using Microsoft.AspNetCore.Http;
//using Modules.Identity.Entities;
//using Modules.Identity.Entities.Views;

//namespace Modules.Identity.Api.Role.Handler;

//[Authorize]
//[Get("/api/identity/role/detail")]
//public class RoleDetailHandler(AppDbContext appDb, [FromQuery] int id) : CommandHandler
//{
//    protected RoleView Data;

//    public override async Task<IResult> Validate()
//    {
//        Data = await appDb.Views.FirstOrDefaultAsync<RoleView>(new { Id = id });
//        if (Data == null)
//            return NotFound();

//        return await Next();
//    }

//    public override RoleView Response() => Data;
//}