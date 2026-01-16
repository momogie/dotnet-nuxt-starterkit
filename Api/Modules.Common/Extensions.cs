using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Logger.Entities;
using Newtonsoft.Json;
using Shared;
using System.Diagnostics;

namespace Modules.Logger;

public static class Extensions
{
    public static IServiceCollection AddModuleCommon(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseContext<AppDbContext>("Default");
        return services;
    }

    public static void UseModuleCommon(this WebApplication app)
    {
        app.MapCommandHandlers(typeof(Extensions).Assembly);
    }
}
