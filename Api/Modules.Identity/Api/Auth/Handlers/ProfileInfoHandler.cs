using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Modules.Identity.Entities;
using Shared;

namespace Modules.Identity.Api.Auth.Handlers;

[Authorize]
[Get("/api/auth/v1/profile-info")]
public class ProfileInfoHandler(AppDbContext appDb) : CommandHandler
{
    public override IResult Response()
    {
        return Ok(new
        {

        });
    }
}
