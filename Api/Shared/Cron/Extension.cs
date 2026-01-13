using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using System.Reflection;
using System.Text;

namespace Shared;

public static class CronExtension
{
    public static void AddCronJobs(this IServiceCollection services)
    {
        //services.AddHangfire(p => p.SetDataCompatibilityLevel(CompatibilityLevel.Version_170));
        //services.AddHangfireServer();

        //services.AddScoped<CronManager>();
        services.AddCronJobs(typeof(CronJob).Assembly);
        //using var scope = services.BuildServiceProvider().CreateScope();

        //var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        //// Load config dari appsettings.json
        //var hangfireConfig = new HangfireSettings();
        //config.GetSection("Hangfire").Bind(hangfireConfig);

        //services.AddHangfire(p => p.SetDataCompatibilityLevel(CompatibilityLevel.Version_170));
        //if (hangfireConfig == null)
        //{
        //    services.AddHangfireServer();
        //}
        //else
        //{
        //    // Tambahkan HangfireServer untuk tiap queue
        //    foreach (var queue in hangfireConfig.Queues)
        //    {
        //        services.AddHangfireServer(options =>
        //        {
        //            options.WorkerCount = queue.WorkerCount;
        //            options.Queues = [queue.Name];
        //        });
        //    }
        //}


        //services.AddScoped<CronManager>();
        //var list = typeof(CronJob).Assembly.ExportedTypes.Where(p => p.GetCustomAttribute<CronJob>() != null && p.BaseType == typeof(BaseCronJob)).ToList();
        //foreach (var r in list)
        //    services.AddScoped(r);
    }

    public static void AddCronManager(this IServiceCollection services)
    {
        using var scope = services.BuildServiceProvider().CreateScope();

        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        // Load config dari appsettings.json
        var hangfireConfig = new HangfireSettings();
        config.GetSection("Hangfire").Bind(hangfireConfig);

        services.AddHangfire(p => p.SetDataCompatibilityLevel(CompatibilityLevel.Version_170));
        if (hangfireConfig == null)
        {
            services.AddHangfireServer();
        }
        else
        {
            // Tambahkan HangfireServer untuk tiap queue
            foreach (var queue in hangfireConfig.Queues)
            {
                services.AddHangfireServer(options =>
                {
                    options.WorkerCount = queue.WorkerCount;
                    options.Queues = [queue.Name];
                });
            }
        }


        services.AddScoped<CronManager>();
    }

    public static void AddCronJobs(this IServiceCollection services, Assembly assembly)
    {
        services.AddHangfire(p => p.SetDataCompatibilityLevel(CompatibilityLevel.Version_170));
        services.AddHangfireServer();

        services.AddScoped<CronManager>();
        var list = assembly.ExportedTypes.Where(p => p.GetCustomAttribute<CronJob>() != null && p.BaseType == typeof(BaseCronJob)).ToList();
        foreach (var r in list)
            services.AddScoped(r);
    }

    //public static void RunCronJobs(this WebApplication app, DbContext dbContext)
    //{
    //    app.RunCronJobs(dbContext);
    //    //using var scope = app.Services.CreateScope();

    //    //var config = scope.ServiceProvider.GetService<IConfiguration>();

    //    ////var hfDbName = $"{dbContext.Database.GetDbConnection().Database}.Hangfire";

    //    ////var cnnStr = dbContext.Database.GetConnectionString().Replace(dbContext.Database.GetDbConnection().Database, hfDbName);
    //    //var cnnStr = config.GetConnectionString("Hangfire");

    //    ////dbContext.Database.ExecuteSqlRaw($"IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '{hfDbName}') BEGIN CREATE DATABASE [{hfDbName}] END");
    //    //GlobalConfiguration.Configuration
    //    //    .UseSimpleAssemblyNameTypeSerializer()
    //    //    .UseRecommendedSerializerSettings()
    //    //    .UseSqlServerStorage(cnnStr);


    //    //var cronManager = scope.ServiceProvider.GetService<CronManager>();
    //    //cronManager.RunJobs();

    //    //app.UseHangfireDashboard("/hangfire", new DashboardOptions
    //    //{
    //    //    //Authorization = [new BasicAuthAuthorizationFilter("momogie", "Momogie@123@")]
    //    //});

    //}

    public static void UseCronManager(this WebApplication app, string connectionString)
    {
        using var scope = app.Services.CreateScope();

        var config = scope.ServiceProvider.GetService<IConfiguration>();

        //var cnnStr = config.GetConnectionString("Hangfire");

        //dbContext.Database.ExecuteSqlRaw($"IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '{hfDbName}') BEGIN CREATE DATABASE [{hfDbName}] END");
        GlobalConfiguration.Configuration
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString);

        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            //Authorization = [new BasicAuthAuthorizationFilter("momogie", "Momogie@123@")]
        });
    }

    public static void RunCronJobs(this WebApplication app, Assembly assembly, DbContext dbContext)
    {
        using var scope = app.Services.CreateScope();

        var config = scope.ServiceProvider.GetService<IConfiguration>();

        //var hfDbName = $"{dbContext.Database.GetDbConnection().Database}.Hangfire";

        //var cnnStr = dbContext.Database.GetConnectionString().Replace(dbContext.Database.GetDbConnection().Database, hfDbName);
        var cnnStr = config.GetConnectionString("Hangfire");

        //dbContext.Database.ExecuteSqlRaw($"IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '{hfDbName}') BEGIN CREATE DATABASE [{hfDbName}] END");
        GlobalConfiguration.Configuration
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(cnnStr);


        var cronManager = scope.ServiceProvider.GetService<CronManager>();
        cronManager.RunJobs(assembly);

        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            //Authorization = [new BasicAuthAuthorizationFilter("momogie", "Momogie@123@")]
        });
        //app.UseHangfireDashboard("/hangfire", new DashboardOptions
        //{
        //    //Authorization = [new BasicAuthAuthorizationFilter("momogie", "Momogie@123@")]
        //});

    }

}

public class BasicAuthAuthorizationFilter(string username, string password) : IDashboardAuthorizationFilter
{
    private readonly string _username = username;
    private readonly string _password = password;

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Check if the request contains an Authorization header
        string authHeader = httpContext.Request.Headers.Authorization;
        if (authHeader != null && authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            // Decode the base64 encoded credentials
            var encodedUsernamePassword = authHeader["Basic ".Length..].Trim();
            var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

            // Split the username and password
            var parts = decodedUsernamePassword.Split(':');
            if (parts.Length == 2)
            {
                var username = parts[0];
                var password = parts[1];

                // Validate the username and password
                if (username == _username && password == _password)
                {
                    return true; // Allow access if the credentials are valid
                }
            }
        }

        // If the Authorization header is missing or invalid, prompt for authentication
        //httpContext.Response.Headers.WWWAuthenticate = "Basic realm=\"Hangfire Dashboard\"";
        //httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return false; // Deny access if the credentials are invalid
    }
}

