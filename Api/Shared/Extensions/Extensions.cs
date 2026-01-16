using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared;

public static class Extensions
{
    public static IServiceCollection AddShared(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<Configuration>();
        services.AddHttpContextAccessor();
        services.AddControllers().AddJsonOptions(opt =>
        {
            opt.JsonSerializerOptions.PropertyNamingPolicy = null;
            //opt.JsonSerializerOptions.Converters.Add(new EpochDateTimeConverter());
        })
        .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);
        // Configure JSON options to use PascalCase
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = null;
        });

        return services;
    }

    public static void UseShared(this WebApplication app)
    {
        
    }
}
