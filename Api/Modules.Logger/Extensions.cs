using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Logger.Entities;
using Modules.Logger.Entities.DbSchema;
using Newtonsoft.Json;
using Shared;
using System;
using System.Diagnostics;

namespace Modules.Logger;

public static class Extensions
{
    public static IServiceCollection AddModuleLogger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseContext<LogDbContext>("Logger");
        services.AddScoped<IDataLogger, DataLogger>();
        return services;
    }

    public static void UseModuleLogger(this WebApplication app)
    {
        app.MapCommandHandlers(typeof(Extensions).Assembly);

        var name = typeof(Extensions).Namespace;
        foreach (Type r in typeof(Extensions).Assembly.GetExportedTypes().Where(p => p.GetCustomAttributes(true).Any(c => c.GetType() == typeof(SqlView))))
            ModuleDbContext.RegisterView("Log", r, []);

        app.UseExceptionHandler(a => a.Run(async context =>
        {
            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature.Error;

            try
            {
                var logDb = context.RequestServices.GetService<LogDbContext>();
                var contextAccessor = context.RequestServices.GetRequiredService<IHttpContextAccessor>();

                var userName = contextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(p => p.Type == "UserName")?.Value ?? "";
                var name = contextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(p => p.Type == "Name")?.Value ?? "";

                //var allowedNamespaces = new[] { "Web", "Entities", "Logger" };
                //var filteredInnerStackTrace = new StackTrace(exception?.InnerException, true)
                //    .GetFrames()?.Where(p => p == p)
                //    //?.Where(frame =>
                //    //    allowedNamespaces.Any(ns =>
                //    //        frame.GetMethod()?.DeclaringType?.Namespace?.StartsWith(ns) == true))
                //    .Select(frame => frame.ToString())
                //    .ToArray();

                var filteredInnerStackTrace = exception;

                var filteredStackTrace = new StackTrace(exception, true)
                    .GetFrames()?
                    //?.Where(frame =>
                    //    allowedNamespaces.Any(ns =>
                    //        frame.GetMethod()?.DeclaringType?.Namespace?.StartsWith(ns) == true))
                    .Select(frame => frame.ToString())
                    .ToArray();

                var errLog = new ErrorLog
                {
                    Date = EpochDateTime.Now,
                    Message = exception?.InnerException?.Message ?? exception.Message,
                    Method = context.Request.Method,
                    UserAgent = context.Request.Headers.UserAgent.ToString(),
                    RemoteAddr = context.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                    RequestPath = context.Request.Path,
                    StackTrace = exception?.InnerException != null ?
                        string.Join(Environment.NewLine, filteredInnerStackTrace) :
                        string.Join(Environment.NewLine, filteredStackTrace),
                    UserId = context.User?.FindFirst("UserId")?.Value,
                    UserName = context.User?.FindFirst("UserName")?.Value,
                    Name = name,
                };
                logDb.ErrorLogs.Add(errLog);
                logDb.SaveChanges();
            }
#if DEBUG
            catch (Exception ex)
            {
                exception = ex;
            }
#else
            catch { }
#endif

            var result = JsonConvert.SerializeObject(new
            {
                Title = "Error",
#if DEBUG
                Message = exception?.InnerException?.Message ?? exception.Message,
                StackTrace = exception?.InnerException?.StackTrace.Split(Environment.NewLine) ?? exception.StackTrace.Split(Environment.NewLine),
#endif
                Errors = new { }
            });

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(result);
        }));
    }
}
