using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Identity.Entities;
using Modules.Identity.Entities.DbSchemas;

namespace Modules.Identity;

public static class Extensions
{
    public static IServiceCollection AddModuleIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(p => p.UseSqlServer(configuration.GetConnectionString("Default")));
        services.AddRazorPages()
                .AddApplicationPart(typeof(Modules.Identity.Extensions).Assembly);

        // Add services to the container.
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", options => { })
            .AddCookie(options =>
            {
                options.Cookie.Name = configuration["Auth:CookieName"];
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });
        services.AddAuthorization();

        return services;
    }

    public static void UseModuleIdentity(this WebApplication app)
    {
        //app.UseApiDocumentation();
        app.MapCommandHandlers(typeof(Extensions).Assembly);
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapRazorPages();

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var name = typeof(Extensions).Namespace;
        //foreach (Type r in typeof(Extensions).Assembly.GetExportedTypes().Where(p => p.GetCustomAttributes(true).Any(c => c.GetType() == typeof(SqlView))))
        //    cherryDb.RegisterView(db.Schema, r);

    }
}
