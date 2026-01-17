using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Shared;

namespace Api.Entities;

public class AppDbContext : ModuleDbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {

    }

    public override string Schema => "dbo";
}
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Logging.Migrations;User Id=sa;Password=pass@word1;Integrated Security=true;TrustServerCertificate=True");

        return new AppDbContext(optionsBuilder.Options);
    }
}
