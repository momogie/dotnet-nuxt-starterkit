using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Modules.Identity.Api.Auth.Handlers;

[Get("/auth/logout")]
public class LogoutHandler(IHttpContextAccessor httpContextAccessor) : CommandHandler
{
    public override async Task<IResult> Validate()
    {
        // sign out self
        await httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }

    public override IResult Response() => Redirect("/");
}
