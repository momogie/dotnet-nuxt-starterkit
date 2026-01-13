using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.AspNetCore.Identity;

using Modules.Identity.Entities.DbSchemas;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Modules.Identity.Entities;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User, Role, string,
        UserClaim, UserRole, UserLogin,
        RoleClaim, UserToken>(options)
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public override DbSet<Role> Roles { get; set; }
    public override DbSet<RoleClaim> RoleClaims { get; set; }
    public override DbSet<User> Users { get; set; }
    public override DbSet<UserClaim> UserClaims { get; set; }
    public override DbSet<UserLogin> UserLogins { get; set; }
    public override DbSet<UserRole> UserRoles { get; set; }
    public override DbSet<UserToken> UserTokens { get; set; }

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