using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Identity.Api.Role.Command;
using Modules.Identity.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Modules.Identity.Api.Role.Handler;

[Authorize]
[Post("/api/identity/role/create")]
public class UserRoleCreateHandler(AppDbContext appDb, [FromBody] UserRoleCommand command) : CommandHandler
{
    protected Entities.DbSchemas.Role Data;

    [Pipeline(1)]
    public void BeginTran() => appDb.Database.BeginTransaction();

    [Pipeline(2)]
    public void Save()
    {
        Data = new Entities.DbSchemas.Role
        {
            Id = Guid.NewGuid().UniqueId(10),
            Name = command.Name,
            //Description = command.Description,
        };

        appDb.Roles.Add(Data);
        appDb.SaveChanges();
    }

    [Pipeline(2)]
    public void SavePrivileges()
    {
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