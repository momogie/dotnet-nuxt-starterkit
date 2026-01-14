using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modules.Identity.Entities.DbSchemas;
using PluralizeService.Core;
using System.Text;

namespace Modules.Identity.Entities;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User, Role, string,
        UserClaim, UserRole, UserLogin,
        RoleClaim, UserToken>(options)
{
    public DbSet<Preference> Preferences { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public override DbSet<Role> Roles { get; set; }
    public override DbSet<RoleClaim> RoleClaims { get; set; }
    public override DbSet<User> Users { get; set; }
    public override DbSet<UserClaim> UserClaims { get; set; }
    public override DbSet<UserLogin> UserLogins { get; set; }
    public override DbSet<UserRole> UserRoles { get; set; }
    public override DbSet<UserToken> UserTokens { get; set; }

    public DbView Views => new (this, "");

    protected override void OnModelCreating(ModelBuilder builder)
    {
        //modelBuilder.Entity<Preference>(entity =>
        //{
        //    entity.HasNoKey();
        //});

        //modelBuilder.Ignore<User>();
        base.OnModelCreating(builder);

        // Users
        builder.Entity<User>().ToTable("Users");
        builder.Entity<Role>().ToTable("Roles");
        builder.Entity<UserRole>().ToTable("UserRoles");
        builder.Entity<UserClaim>().ToTable("UserClaims");
        builder.Entity<RoleClaim>().ToTable("RoleClaims");
        builder.Entity<UserLogin>().ToTable("UserLogins");
        builder.Entity<UserToken>().ToTable("UserTokens");

        // Refresh token
        builder.Entity<RefreshToken>().ToTable("RefreshTokens");
    }
    private static string FormatViewName(Type type)
    {
        var c = type.Namespace.Replace("Module.", "").Replace(".Entities.Views", "").Replace(".", "_");
        return $"{c}_{PluralizationProvider.Pluralize(type.Name)}";
    }

    public void InitializeViews()
    {
        var name = GetType().Namespace;
        foreach (Type r in GetType().Assembly.GetExportedTypes().Where(p => p.GetCustomAttributes(true).Any(c => c.GetType() == typeof(SqlView))))
        {
            string dir = System.IO.Path.GetDirectoryName(AppContext.BaseDirectory);
            string sqlPath = r.Namespace.Replace(".Entities.Views", "/Entities/Views");
            string fileNames = string.Concat(dir, "/", sqlPath, "/", r.Name, ".sql");
            if (!System.IO.File.Exists(fileNames))
                throw new Exception($"The sql file ${fileNames} does not exist.");

            var sqlLines = System.IO.File.ReadAllLines(fileNames, Encoding.UTF8);
            var sql = string.Join(Environment.NewLine, sqlLines);

            Database.ExecuteSqlRaw($"CREATE OR ALTER VIEW {FormatViewName(r)} AS {sql}");
        }
    }
}

public class DbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("*");

        return new AppDbContext(optionsBuilder.Options);
    }
}