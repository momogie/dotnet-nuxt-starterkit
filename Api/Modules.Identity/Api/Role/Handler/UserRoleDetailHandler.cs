namespace Modules.Identity.Api.Role.Handler;

[Authorize]
[Get("/api/identity/role/detail")]
public class RoleDetailHandler(AppDbContext appDb, [FromQuery] string id) : CommandHandler
{
    protected RoleView Data;

    public override async Task<IResult> Validate()
    {
        Data = await appDb.Views.FirstOrDefaultAsync<RoleView>(new { Id = id });
        if (Data == null)
            return NotFound();

        Data.Claims = [..appDb.RoleClaims.Where(p => p.RoleId == id).Select(p => new RoleClaimView
        {
            Type = p.ClaimType,
            Value = p.ClaimValue,
        })];

        return await Next();
    }

    public override RoleView Response() => Data;
}