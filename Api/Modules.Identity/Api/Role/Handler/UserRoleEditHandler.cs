using Microsoft.AspNetCore.Http;
using Modules.Identity.Api.Role.Command;
using Modules.Identity.Entities;
using Modules.Logger;

namespace Modules.Identity.Api.Role.Handler;

[Authorize]
[Patch("/api/identity/role/edit")]
public class UserEditHandler(AppDbContext appDb, [FromQuery] string id, [FromBody] UserRoleCommand command, IDataLogger dataLogger) : CommandHandler
{
    protected Entities.DbSchemas.Role Data;
    protected RoleLog LogBefore { get; set; }

    public override async Task<IResult> Validate()
    {
        Data = appDb.Roles.FirstOrDefault(p => p.Id == id);
        if (Data == null)
            return NotFound();

        LogBefore = CaptureLog();

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

    [Pipeline(10)]
    public void SaveLog()
    {
        dataLogger.SaveDataLog(new DataLogDto
        {
            DocumentType = "User Role",
            Action = DataChangeAction.Update,
            EntityId = Data.Id,
            ReferenceId = Data.Name,
            After = CaptureLog(),
            Before = LogBefore,
        });
    }

    private RoleLog CaptureLog()
    {
        var log = appDb.Roles.FirstOrDefault(p => p.Id == id);
        return new RoleLog
        {
            Name = log.Name,
            Claims = [..appDb.RoleClaims.Where(p => p.RoleId == id).Select(p => new RoleClaimLog
            {
                Type = p.ClaimType,
                Value = p.ClaimValue
            })]
        };
    }
}
