using Microsoft.EntityFrameworkCore;
using Shared;

namespace Api;

public class AppDbContext : ModuleDbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {

    }

    public override string Schema => "dbo";
}
