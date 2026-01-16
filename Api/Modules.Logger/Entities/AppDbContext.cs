using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modules.Logger.Entities.DbSchema;
using Shared;
using System.Data;

namespace Modules.Logger.Entities;

public class AppDbContext(DbContextOptions<AppDbContext> options) : ModuleDbContext(options)
{
    public DbSet<DataChangeLog> DataChangeLogs { get; set; }
    public DbSet<ErrorLog> ErrorLogs { get; set; }
    public override string Schema => "Log";

    protected override void OnModelCreating(ModelBuilder builder)
    {
        if (!string.IsNullOrWhiteSpace(Schema))
            builder.HasDefaultSchema(Schema);

        base.OnModelCreating(builder);
    }
}

public class LogDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Logging.Migrations;User Id=sa;Password=pass@word1;Integrated Security=true;TrustServerCertificate=True");

        return new AppDbContext(optionsBuilder.Options);
    }
}