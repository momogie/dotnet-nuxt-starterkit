using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modules.Reporting.Entities.DbSchema;
using Shared;

namespace Modules.Reporting.Entities;

public class AppDbContext(DbContextOptions<AppDbContext> options) : ModuleDbContext(options)
{
    public DbSet<ReportTemplate> ReportTemplates { get; set; }

    public override string Schema => "Rpt";

    protected override void OnModelCreating(ModelBuilder builder)
    {
        if (!string.IsNullOrWhiteSpace(Schema))
            builder.HasDefaultSchema(Schema);

        base.OnModelCreating(builder);
    }
}

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=App;User Id=sa;Password=pass@word1;Integrated Security=true;TrustServerCertificate=True");

        return new AppDbContext(optionsBuilder.Options);
    }
}