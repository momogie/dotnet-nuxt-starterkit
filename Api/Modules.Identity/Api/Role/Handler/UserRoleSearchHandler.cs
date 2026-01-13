//using Modules.Identity.Entities;

//namespace Modules.Identity.Api.Role.Handler;

//[Authorize]
//[Get("/api/identity/role/search")]
//public class RoleSearchHandler(AppDbContext appDb
//    , [FromQuery] string q
//    , [FromQuery] int[] ids
//    , [FromQuery] int c = 25) : CommandHandler
//{
//    public override List<RoleSearchResult> Response()
//    {
//        ids ??= [];
//        var list = from p in appDb.Users
//                   where (
//                    string.IsNullOrWhiteSpace(q)
//                    || p.Name.Contains(q)
//                    || p.Name.Equals(q)
//                   )
//                   && (ids.Length == 0 || ids.Contains(p.Id))
//                   orderby p.Name
//                   select new RoleSearchResult
//                   {
//                       Id = p.Id,
//                       Name = p.Name
//                   };

//        return [.. list.Take(c)];
//    }
//}

//public class RoleSearchResult
//{
//    public int Id { get; set; }
//    public string Name { get; set; }
//}