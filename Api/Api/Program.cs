using Api;
using Microsoft.EntityFrameworkCore;
using Shared;

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
#endif
app.UseModules();
app.UseApiDocumentation();
app.MapRazorPages();
app.UseStaticContent();

app.Run();
