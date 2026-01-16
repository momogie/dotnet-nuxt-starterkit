using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseContext<T>(this IServiceCollection services, string connectionName) where T : ModuleDbContext
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var connectionString = config.GetConnectionString(connectionName);
        services.AddMSSQL<T>(connectionString);
        return services;
    }

    private static IServiceCollection AddMSSQL<T>(this IServiceCollection services, string connectionString) where T : ModuleDbContext
    {
        services.AddDbContext<T>(m => m.UseSqlServer(connectionString, e => e.MigrationsAssembly(typeof(T).Assembly.FullName)));
        using var scope = services.BuildServiceProvider().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<T>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        try
        {
            dbContext.Database.Migrate();
        }
#if DEBUG
        catch(Exception ex) { 
            Console.WriteLine(ex); 
        }
#else
        catch { }
#endif
        //dbContext.SetDbPrefix(config["DbPrefix"]);
        dbContext.InitializeViews();
        return services;
    }
}