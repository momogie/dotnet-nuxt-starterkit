using Api;
using Api.Entities;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var conn = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContextPool<AppDbContext>(p => p.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddModules(builder.Configuration);
builder.Services.AddApiDocumentation();
builder.Services.AddRazorPages();

// Configure JSON options to use PascalCase
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
});

#if DEBUG
builder.Services.AddCors(confg =>
                confg.AddPolicy("AllowAll",
                    p => p.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:3002")
                        .AllowCredentials()
                        .AllowAnyMethod()
                        .AllowAnyHeader()));

/* YARP */
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

#endif

var app = builder.Build();

#if DEBUG
app.UseCors("AllowAll");
app.MapReverseProxy();
app.UseMiddleware<UploadRateLimitMiddleware>(); // 100 KB/s
#endif
app.UseModules();
app.UseApiDocumentation();
app.MapRazorPages();
app.UseStaticContent();

app.Run();

public class UploadRateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly int _maxBytesPerSecond;

    public UploadRateLimitMiddleware(RequestDelegate next)
    {
        _next = next;
        _maxBytesPerSecond = 2 * 1024;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.ContentLength > 0 && context.Request.Body.CanRead)
        {
            var buffer = new byte[8192]; // Buffer 8KB
            var bodyStream = context.Request.Body;
            var tempStream = new MemoryStream();
            int bytesRead;
            var stopwatch = new Stopwatch();

            while ((bytesRead = await bodyStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                stopwatch.Restart();
                await tempStream.WriteAsync(buffer, 0, bytesRead);

                // Hitung waktu yang diperlukan agar tetap dalam batas kecepatan
                var elapsed = stopwatch.ElapsedMilliseconds;
                var expectedTime = (bytesRead * 1000) / _maxBytesPerSecond;

                if (elapsed < expectedTime)
                {
                    await Task.Delay(expectedTime - (int)elapsed);
                }
            }

            tempStream.Seek(0, SeekOrigin.Begin);
            context.Request.Body = tempStream;
        }

        await _next(context);
    }
}
