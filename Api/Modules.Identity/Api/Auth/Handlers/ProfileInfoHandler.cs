namespace Modules.Identity.Api.Auth.Handlers;

[Authorize]
[Get("/api/profile/user-info")]
public class ProfileInfoHandler(AppDbContext appDb) : CommandHandler
{
    public override IResult Response()
    {
        var user = appDb.Users.FirstOrDefault(p => p.Id == UserId);
        var roles = appDb.Roles.Where(p => appDb.UserRoles.Any(c => c.RoleId == p.Id && c.UserId == user.Id)).ToList();

        var roleIds = roles.Select(p => p.Id).ToArray();

        var roleClaims = appDb.RoleClaims.Where(p => roleIds.Any(c => c == p.RoleId)).Select(p => p.ClaimValue).Distinct().ToList(); ;
        var userClaims = appDb.UserClaims.Where(p => p.UserId == user.Id).Select(p => p.ClaimValue).Distinct().ToList();

        return Ok(new
        {
            user.Name,
            user.UserName,
            user.Email,
            user.EmailConfirmed,
            user.ImageUrl,
            Roles = roles,
            RoleClaims = roleClaims,
            UserClaims = userClaims,
        });
    }
}
