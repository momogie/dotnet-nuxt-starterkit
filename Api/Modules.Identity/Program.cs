////using Cherry.Entities.Entities;
//using Microsoft.EntityFrameworkCore;
//using Modules.Identity;
//using Modules.Identity.Entities;

//var builder = WebApplication.CreateBuilder(args);
////builder.Services.AddDbContext<CherryDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Cherry")));
//builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Cherry")));
//builder.Services.AddModuleIdentity(builder.Configuration);
//builder.Services.AddRazorPages();

//var app = builder.Build();
//app.UseModuleIdentity();
//app.Run();
