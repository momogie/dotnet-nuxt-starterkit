using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Modules.Identity;
using Newtonsoft.Json;
using Shared;

namespace Api;

public static class Extension
{
    public static void UseAltheaErrorHandler(this WebApplication app)
    {
        app.UseExceptionHandler(a => a.Run(async context =>
        {
            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature.Error;

            try
            {
                //var logDb = context.RequestServices.GetRequiredService<LogDbContext>();
                //var auth = context.RequestServices.GetRequiredService<Auth>();

                //var errLog = new SystemError
                //{
                //    Date = EpochDateTime.Now,
                //    Message = exception?.InnerException?.Message ?? exception.Message,
                //    Method = context.Request.Method,
                //    UserAgent = context.Request.Headers.UserAgent.ToString(),
                //    RemoteAddr = context.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                //    RequestPath = context.Request.Path,
                //    StackTrace = exception?.InnerException?.StackTrace ?? exception.StackTrace,
                //    UserId = auth?.User?.Id,
                //    UserName = auth?.User?.UserName,
                //};
                //logDb.SystemErrors.Add(errLog);
                //logDb.SaveChanges();
            }
            catch { }

            var result = JsonConvert.SerializeObject(new
            {
                Title = "Error",
#if DEBUG
                Message = app.Environment.IsDevelopment() ? (exception?.InnerException?.Message ?? exception.Message) : null,
                StackTrace = app.Environment.IsDevelopment() ? (exception?.InnerException?.StackTrace ?? exception.StackTrace) : null,
#endif
                Errors = new { }
            });

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(result);
        }));
    }

    public static IServiceCollection AddModules(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddCronManager();
        services.AddMailer(configuration);
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
        app.UseModuleIdentity();
        //app.UseModuleConfiguration();
        return app;
    }
}
