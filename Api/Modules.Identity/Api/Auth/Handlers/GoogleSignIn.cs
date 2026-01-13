
using System;
using System.Security.Claims;

namespace Modules.Identity.Api.Auth.Handlers;

[Get("/auth/google/signin")]
public class GoogleSignIn(AppDbContext appDb, IConfiguration configuration) : CommandHandler
{
    public override IResult Response()
    {
        var props = new AuthenticationProperties
        {
            RedirectUri = configuration["Auth:Google:RedirectUri"]
        };
        return Results.Challenge(props, ["Google"]);
    }
}
