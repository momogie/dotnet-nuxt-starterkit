using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Modules.Identity;
using Modules.Logger;
using Newtonsoft.Json;
using Shared;

namespace Api;

public static class Extension
{
    public static IServiceCollection AddModules(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddCronManager();
        services.AddMailer(configuration);
        services.AddShared(configuration);
        services.AddModuleLogger(configuration);
        services.AddModuleIdentity(configuration);
        //services.AddModuleConfiguration(configuration);
        return services;
    }

    public static WebApplication UseModules(this WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        app.MapCommandHandlers(typeof(Extension).Assembly);
        app.UseCronManager(config.GetConnectionString("Default"));
        app.UseShared();
        app.UseModuleLogger();
        app.UseModuleIdentity();
        //app.UseModuleConfiguration();
        return app;
    }
}
