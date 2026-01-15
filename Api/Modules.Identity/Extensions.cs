using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Modules.Identity;

public static class Extensions
{
    public static IServiceCollection AddModuleIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(p => p.UseSqlServer(configuration.GetConnectionString("Identity")));
        services.AddRazorPages()
                .AddApplicationPart(typeof(Modules.Identity.Extensions).Assembly);

        // JWT
        services
            .AddAuthentication("Cookies")
            .AddCookie(options =>
            {
                options.Cookie.Name = configuration["Auth:CookieName"];
                options.LoginPath = "/auth/signin";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(Convert.ToInt32(configuration["Auth:ExpireTimeSpanMinute"]));
                options.SlidingExpiration = true;

                //options.Cookie.SameSite = SameSiteMode.None;
                //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return Task.CompletedTask;
                        }

                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    }
                };
            })
            .AddGoogle(options =>
            {
                options.CallbackPath = configuration["Auth:Google:CallbackPath"];
                options.ClientId = configuration["Auth:Google:ClientId"];
                options.ClientSecret = configuration["Auth:Google:ClientSecret"];
                options.ClaimActions.MapJsonKey("picture", "picture");
                options.UserInformationEndpoint = "https://openidconnect.googleapis.com/v1/userinfo";
                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(
                            HttpMethod.Get,
                            context.Options.UserInformationEndpoint);

                        request.Headers.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue(
                                "Bearer",
                                context.AccessToken);

                        var response = await context.Backchannel.SendAsync(
                            request,
                            HttpCompletionOption.ResponseHeadersRead,
                            context.HttpContext.RequestAborted);

                        response.EnsureSuccessStatusCode();

                        using var user = System.Text.Json.JsonDocument.Parse(
                            await response.Content.ReadAsStringAsync());

                        context.RunClaimActions(user.RootElement);
                    }
                };
            }); ;

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

        services.AddAuthorization();

        return services;
    }

    public static void UseModuleIdentity(this WebApplication app)
    {
        app.UseCookiePolicy(new CookiePolicyOptions
        {
            MinimumSameSitePolicy = SameSiteMode.None
        });
        app.MapCommandHandlers(typeof(Extensions).Assembly);
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapRazorPages();

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        db.InitializeViews();
        var name = typeof(Extensions).Namespace;
        foreach (Type r in typeof(Extensions).Assembly.GetExportedTypes().Where(p => p.GetCustomAttributes(true).Any(c => c.GetType() == typeof(SqlView))))
            ModuleDbContext.RegisterView("Idp", r, []);
    }
}
