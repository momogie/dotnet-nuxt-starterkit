using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Reporting.Entities;
using Shared;

namespace Modules.Logger;

public static class Extensions
{
    public static IServiceCollection AddModuleReporting(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseContext<AppDbContext>("Default");
        return services;
    }

    public static void UseModuleReporting(this WebApplication app)
    {
        app.MapCommandHandlers(typeof(Extensions).Assembly);

        var name = typeof(Extensions).Namespace;
        foreach (Type r in typeof(Extensions).Assembly.GetExportedTypes().Where(p => p.GetCustomAttributes(true).Any(c => c.GetType() == typeof(SqlView))))
            ModuleDbContext.RegisterView("Rpt", r, []);
    }
}
