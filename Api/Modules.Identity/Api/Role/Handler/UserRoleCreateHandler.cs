using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Identity.Api.Role.Command;
using Modules.Identity.Entities;
using Modules.Logger;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Modules.Identity.Api.Role.Handler;

[Authorize]
[Post("/api/identity/role/create")]
public class UserRoleCreateHandler(AppDbContext appDb, [FromBody] UserRoleCommand command, IDataLogger dataLogger) : CommandHandler
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

    [Pipeline(10)]
    public void SaveLog()
    {
        dataLogger.SaveDataLog(new DataLogDto
        {
            DocumentType = "User Role",
            Action = DataChangeAction.Create,
            EntityId = Data.Id,
            ReferenceId = Data.Name,
            After = CaptureLog(),
        });
    }

    private RoleLog CaptureLog()
    {
        var log = appDb.Roles.FirstOrDefault(p => p.Id == Data.Id);
        return new RoleLog
        {
            Name = log.Name,
            Claims = [..appDb.RoleClaims.Where(p => p.RoleId == Data.Id).Select(p => new RoleClaimLog
            {
                Type = p.ClaimType,
                Value = p.ClaimValue
            })]
        };
    }

}