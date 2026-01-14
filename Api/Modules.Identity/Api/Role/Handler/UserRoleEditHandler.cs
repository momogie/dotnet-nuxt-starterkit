using Microsoft.AspNetCore.Http;
using Modules.Identity.Api.Role.Command;
using Modules.Identity.Entities;

namespace Modules.Identity.Api.Role.Handler;

[Authorize]
[Patch("/api/identity/role/edit")]
public class UserEditHandler(AppDbContext appDb, [FromQuery] string id, [FromBody] UserRoleCommand command) : CommandHandler
{
    protected Entities.DbSchemas.Role Data;

    public override async Task<IResult> Validate()
    {
        Data = appDb.Roles.FirstOrDefault(p => p.Id == id);
        if (Data == null)
            return NotFound();

        return await Next();
    }

    [Pipeline(1)]
    public void BeginTran() => appDb.Database.BeginTransaction();

    [Pipeline(2)]
    public void Save()
    {
        Data.Name = command.Name;
        //Data.Description = command.Description;
        appDb.SaveChanges();
    }


    [Pipeline(3)]
    public void SavePrivileges()
    {
        appDb.RoleClaims.RemoveRange(p => p.RoleId == id);
        appDb.SaveChanges();

        if (command.Claims == null)
            return;

        appDb.RoleClaims.AddRange(command.Claims.Select(p => new RoleClaim
        {
            ClaimType = p.Type,
            ClaimValue = p.Value,
            RoleId = Data.Id,
        }));

        appDb.SaveChanges();
    }

    [Pipeline(4)]
    public void CommitTran() => appDb.Database.CommitTransaction();

    public override Entities.DbSchemas.Role Response() => Data;
}
