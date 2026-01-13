using Logger.Entities.DbSchema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger.Entities;

public class LogDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<DataChangeLog> DataChangeLogs { get; set; }
    public DbSet<ErrorLog> ErrorLogs { get; set; }

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