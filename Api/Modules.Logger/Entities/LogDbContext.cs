using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modules.Logger.Entities.DbSchema;
using Shared;
using System.Data;

namespace Modules.Logger.Entities;

public class LogDbContext(DbContextOptions<LogDbContext> options) : ModuleDbContext(options)
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

public class LogDbContextFactory : IDesignTimeDbContextFactory<LogDbContext>
{
    public LogDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LogDbContext>();
        optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Logging.Migrations;User Id=sa;Password=pass@word1;Integrated Security=true;TrustServerCertificate=True");

        return new LogDbContext(optionsBuilder.Options);
    }
}