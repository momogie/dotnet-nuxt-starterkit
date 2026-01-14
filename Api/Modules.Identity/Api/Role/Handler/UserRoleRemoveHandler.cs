namespace Modules.Identity.Api.User.Handler;

[Authorize]
[Delete("/api/identity/role/remove")]
public class RoleRemoveHandler(AppDbContext appDb, [FromQuery] string id) : CommandHandler
{
    protected Entities.DbSchemas.Role Data { get; set; }

    public override async Task<IResult> Validate()
    {
        Data = appDb.Roles.FirstOrDefault(p => p.Id == id);
        if (Data == null)
            return NotFound();

        return await Next();
    }

    [Pipeline(1)]
    public void Save()
    {
        appDb.RoleClaims.RemoveRange(p => p.RoleId == id);
        appDb.SaveChanges();

        appDb.Roles.Remove(Data);
        appDb.SaveChanges();
    }

    public override Entities.DbSchemas.Role Response() => Data;
}

