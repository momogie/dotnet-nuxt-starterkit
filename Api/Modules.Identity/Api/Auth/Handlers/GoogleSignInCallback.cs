
using System.Security.Claims;

namespace Modules.Identity.Api.Auth.Handlers;

[Get("/auth/google/callbackx")]
public class GoogleSignInCallback(AppDbContext appDb, IConfiguration configuration) : CommandHandler
{
    public override IResult Response()
    {
        var external = HttpContext.AuthenticateAsync("Google").Result;

        if(!external.Succeeded)
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = configuration["Auth:Google:RedirectUri"]
            };
            return Results.Challenge(props, ["Google"]);
        }

        //var googleId = external.Principal.FindFirst("sub")?.Value;
        var googleId = external.Principal.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
        var fullName = external.Principal.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value ?? "";
        var firstName = external.Principal.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Surname)?.Value ?? "";
        var givenName = external.Principal.Claims.FirstOrDefault(p => p.Type == ClaimTypes.GivenName)?.Value ?? "";
        var email = external.Principal.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email)?.Value ?? "";
        var imageUrl = external.Principal.Claims.FirstOrDefault(p => p.Type == "picture")?.Value ?? "";

        var userLogin = appDb.UserLogins.FirstOrDefault(p => p.ProviderKey == googleId);
        if (userLogin != null)
        {
            var user1 = appDb.Users.FirstOrDefault(p => p.Id == userLogin.UserId);
            user1.Name = fullName;
            if(string.IsNullOrWhiteSpace(user1.ImageUrl))
                user1.ImageUrl = imageUrl;
            appDb.SaveChanges();
            Login(user1);
            return Results.Redirect("/");
        }

        var user = new Entities.DbSchemas.User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = Guid.NewGuid().UniqueId(10),
            Email = email,
            Name = fullName,
            ImageUrl = imageUrl,
            //Email =  
            EmailConfirmed = true,
        };

        appDb.Users.Add(user);
        appDb.SaveChanges();

        appDb.UserLogins.Add(new UserLogin
        {
            UserId = user.Id,
            LoginProvider = "Google",
            ProviderDisplayName = "Google",
            ProviderKey = googleId
        });

        appDb.SaveChanges();

        Login(user);

        return Results.Redirect("/");
    }

    private void Login(Entities.DbSchemas.User user)
    {

        // Create user claims
        var claims = new List<Claim>
        {
            new("UserId", user.Id.ToString()),
            new("Email", user.Email ?? "-"),
            new("UserName", user.UserName),
            //new("Name", user.Name),
        };

        // Create identity and sign in
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
        {
            IsPersistent = true, // Ensure cookie persists
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(Convert.ToInt32(configuration["Auth:ExpireTimeSpanMinute"])) // Set expiration if needed
        }).Wait();

        Results.Redirect("/");
    }
}
