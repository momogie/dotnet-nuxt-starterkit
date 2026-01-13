using Microsoft.EntityFrameworkCore;
using Shared;

namespace Api;

public class AppDbContext : ModuleDbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {

    }

    public static Dictionary<string, (string schema, Type type)> ViewList = new Dictionary<string, (string schema, Type type)>();
    public static Dictionary<string, (string schema, Type type)> ExportViewList = new Dictionary<string, (string schema, Type type)>();
    public override string Schema => "dbo";
}
