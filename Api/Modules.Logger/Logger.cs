using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Logger.Entities;
using Modules.Logger.Entities.DbSchema;
using Newtonsoft.Json;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Modules.Logger;

public enum DataChangeAction
{
    Create, Update, Delete
}

public interface IDataLogger
{
    void SaveDataLog(DataLogDto data);
}

public interface IDataLog { }

public class DataLogger(AppDbContext db, IHttpContextAccessor contextAccessor) : IDataLogger
{
    protected HttpContext Context => contextAccessor.HttpContext;

    public void SaveDataLog(DataLogDto data)
    {
        var json = JsonConvert.SerializeObject(Compare(data.Before, data.After));

        var userName = Context.User.Claims.FirstOrDefault(p => p.Type == "UserName")?.Value ?? "";
        var name = Context.User.Claims.FirstOrDefault(p => p.Type == "Name")?.Value ?? "";
        var userId = Context.User.Claims.FirstOrDefault(p => p.Type == "UserId")?.Value;

        var log = new DataChangeLog
        {
            Action = data.Action.ToString(),
            DocumentType = data.DocumentType,
            ReferenceId = data.ReferenceId,
            EntityId = data.EntityId,
            Date = DateTime.Now,
            Data = json,
            Method = Context.Request.Method,
            RequestPath = Context.Request.Path,
            RemoteAddr = Context.Connection.RemoteIpAddress.MapToIPv4().ToString(),
            UserAgent = Context.Request.Headers.UserAgent.ToString(),
            UserId = userId,
            UserName = userName,
            Name = name,
        };

        db.DataChangeLogs.Add(log);
        db.SaveChanges();
    }

    protected static IDictionary<string, IDictionary<string, object>> Compare(object before, object after)
    {
        if (before != null && after != null && before.GetType() != after.GetType())
            throw new Exception("The before and after data type do not match!");

        var bef = new Dictionary<string, object>();
        var aft = new Dictionary<string, object>();

        PropertyInfo[] props = before?.GetType().GetProperties() ?? after?.GetType().GetProperties() ?? [];

        foreach (var r in props)
        {
            object v1 = before == null ? null : r.GetValue(before);
            object v2 = after == null ? null : r.GetValue(after);

            if (v1 == null && v2 == null)
                continue;

            if (v1 == null || v2 == null)
            {
                if (v1 == null)
                {
                    bef.Add(r.GetDefaultName(), "");
                    if (r.PropertyType == typeof(bool) || r.PropertyType == typeof(bool?))
                    {
                        aft.Add(r.GetDefaultName(), (bool?)v2 == true ? "Yes" : "No");
                    }
                    else if (r.PropertyType == typeof(double) || r.PropertyType == typeof(double?))
                    {
                        var aftVal = (double?)v2 ?? 0;
                        aft.Add(r.GetDefaultName(), aftVal.ToString("N0"));
                    }
                    else if (r.PropertyType == typeof(decimal) || r.PropertyType == typeof(decimal?))
                    {
                        var aftVal = (decimal?)v2 ?? 0;
                        aft.Add(r.GetDefaultName(), aftVal.ToString("N0"));
                    }
                    else if (r.PropertyType == typeof(float) || r.PropertyType == typeof(float?))
                    {
                        var aftVal = (float?)v2 ?? 0;
                        aft.Add(r.GetDefaultName(), aftVal.ToString("N0"));
                    }
                    else if (r.PropertyType == typeof(long) || r.PropertyType == typeof(long?))
                    {
                        var aftVal = (long?)v2 ?? 0;
                        aft.Add(r.GetDefaultName(), aftVal.ToString("N0"));
                    }
                    else
                    {
                        aft.Add(r.GetDefaultName(), r.GetValue(after));
                    }
                }
                if (v2 == null)
                {
                    if (r.PropertyType == typeof(bool) || r.PropertyType == typeof(bool?))
                    {
                        bef.Add(r.GetDefaultName(), (bool?)v2 == true ? "Yes" : "No");
                    }
                    else if (r.PropertyType == typeof(double) || r.PropertyType == typeof(double?))
                    {
                        var befVal = (double?)v2 ?? 0;
                        bef.Add(r.GetDefaultName(), befVal.ToString("N0"));
                    }
                    else if (r.PropertyType == typeof(decimal) || r.PropertyType == typeof(decimal?))
                    {
                        var befVal = (decimal?)v2 ?? 0;
                        bef.Add(r.GetDefaultName(), befVal.ToString("N0"));
                    }
                    else if (r.PropertyType == typeof(float) || r.PropertyType == typeof(float?))
                    {
                        var befVal = (float?)v2 ?? 0;
                        bef.Add(r.GetDefaultName(), befVal.ToString("N0"));
                    }
                    else if (r.PropertyType == typeof(long) || r.PropertyType == typeof(long?))
                    {
                        var befVal = (long?)v2 ?? 0;
                        bef.Add(r.GetDefaultName(), befVal.ToString("N0"));
                    }
                    else
                    {
                        bef.Add(r.GetDefaultName(), r.GetValue(before));
                    }
                    aft.Add(r.GetDefaultName(), "");
                }
            }
            else
            {
                Type[] types = [
                    typeof(bool), typeof(bool?),
                    typeof(string),
                    typeof(long), typeof(long?),
                    typeof(int), typeof(int?),
                    typeof(decimal), typeof(decimal?),
                    typeof(float), typeof(float?),
                    typeof(double), typeof(double?),
                    typeof(DateTime), typeof(DateTime?),
                    typeof(DateOnly), typeof(DateOnly?),
                ];

                if (!types.Contains(r.PropertyType))
                {
                    if (r.PropertyType.IsAssignableTo(typeof(IList)))
                    {
                        var v1List = v1 as IList;
                        var v2List = v2 as IList;

                        //IList list = v1List.Count > v2List.Count ? v1List : v2List;

                        //var list1 = new List<object>();
                        //var list2 = new List<object>();

                        //for (int i = 0; i < list.Count; i++)
                        //{
                        //    var x = Compare(v1List.Count - 1 < i ? null : v1List[i], v2List.Count - 1 < i ? null : v2List[i]);

                        //    list1.Add(x["Before"]);
                        //    list2.Add(x["After"]);
                        //}

                        bef.Add(r.GetDefaultName(), v1List);
                        aft.Add(r.GetDefaultName(), v2List);
                    }
                    else
                    {
                        var x = Compare(v1, v2);
                        bef.Add(r.GetDefaultName(), x["Before"]);
                        aft.Add(r.GetDefaultName(), x["After"]);
                    }
                }
                else
                {
                    if (v1.ToString() != v2.ToString())
                    {
                        if (r.PropertyType == typeof(bool) || r.PropertyType == typeof(bool?))
                        {
                            bef.Add(r.GetDefaultName(), (bool?)r.GetValue(before) == true ? "Yes" : "No");
                            aft.Add(r.GetDefaultName(), (bool?)r.GetValue(after) == true ? "Yes" : "No");
                        }
                        else if (r.PropertyType == typeof(double) || r.PropertyType == typeof(double?))
                        {
                            var befVal = (double?)r.GetValue(before) ?? 0;
                            var aftVal = (double?)r.GetValue(after) ?? 0;
                            bef.Add(r.GetDefaultName(), befVal.ToString("N0"));
                            aft.Add(r.GetDefaultName(), aftVal.ToString("N0"));
                        }
                        else if (r.PropertyType == typeof(decimal) || r.PropertyType == typeof(decimal?))
                        {
                            var befVal = (decimal?)r.GetValue(before) ?? 0;
                            var aftVal = (decimal?)r.GetValue(after) ?? 0;
                            bef.Add(r.GetDefaultName(), befVal.ToString("N0"));
                            aft.Add(r.GetDefaultName(), aftVal.ToString("N0"));
                        }
                        else if (r.PropertyType == typeof(float) || r.PropertyType == typeof(float?))
                        {
                            var befVal = (float?)r.GetValue(before) ?? 0;
                            var aftVal = (float?)r.GetValue(after) ?? 0;
                            bef.Add(r.GetDefaultName(), befVal.ToString("N0"));
                            aft.Add(r.GetDefaultName(), aftVal.ToString("N0"));
                        }
                        else if (r.PropertyType == typeof(long) || r.PropertyType == typeof(long?))
                        {
                            var befVal = (long?)r.GetValue(before) ?? 0;
                            var aftVal = (long?)r.GetValue(after) ?? 0;
                            bef.Add(r.GetDefaultName(), befVal.ToString("N0"));
                            aft.Add(r.GetDefaultName(), aftVal.ToString("N0"));
                        }
                        else
                        {
                            bef.Add(r.GetDefaultName(), r.GetValue(before));
                            aft.Add(r.GetDefaultName(), r.GetValue(after));
                        }
                    }
                }
            }
        }

        return new Dictionary<string, IDictionary<string, object>>
        {
            { "Before", bef },
            { "After", aft },
        };
    }
}

public static class DataLoggerExtension
{
    //public static void AddDataLogger(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
    //{
    //    services.AddDbContext<LogDbContext>(optionsAction);
    //    services.AddScoped<IDataLogger, DataLogger>();
    //}

    public static string GetDefaultName(this PropertyInfo property)
    {
        var attr = property.GetCustomAttribute<DisplayAttribute>();
        return attr == null ? property.Name : attr.Name;
    }

    //public static void UseDataLogger(this WebApplication app)
    //{
    //    var scope = app.Services.CreateScope();
    //    var appDb = scope.ServiceProvider.GetRequiredService<LogDbContext>();
    //    appDb.Database.Migrate();

    //    UseErrorLogger(app);
    //}

    //private static void UseErrorLogger(WebApplication app)
    //{
        
    //}
}

public class DataLogDto
{
    public string DocumentType { get; set; }
    public string ReferenceId { get; set; }
    public string EntityId { get; set; }
    public DataChangeAction Action { get; set; }
    public IDataLog Before { get; set; }
    public IDataLog After { get; set; }
}