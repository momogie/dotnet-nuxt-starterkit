using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PluralizeService.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;

namespace Shared;

public abstract class ModuleDbContext : DbContext
{
    public static Dictionary<string, (string schema, Type type)> ViewList = new Dictionary<string, (string schema, Type type)>();
    public static Dictionary<string, (string schema, Type type)> ExportViewList = new Dictionary<string, (string schema, Type type)>();

    public abstract string Schema { get; }

    private IDbConnection Connection => Database.GetDbConnection();

    protected ModuleDbContext(DbContextOptions options) : base(options)
    {
    }

    public static void RegisterView(string schema, Type type, Type[] excludedTypes)
    {
        if (excludedTypes.Contains(type))
            return;

        if (ViewList.ContainsKey(type.Name))
            throw new Exception($"View type with name {type.Name} is already registered");

        ViewList.Add(type.Name, (schema, type));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (!string.IsNullOrWhiteSpace(Schema) && Schema != "dbo")
        {
            modelBuilder.HasDefaultSchema(Schema);
            modelBuilder.Ignore<__ReparationHistory>();
            modelBuilder.Ignore<Attachment>();
        }

        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
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


            Views.Execute($"CREATE OR ALTER VIEW [{Schema}].{PluralizationProvider.Pluralize(r.Name)} AS {sql}");
        }
    }

    public IEnumerable<T> GetList<T>(string sql, object param = null)
    {
        return Connection.Query<T>(sql, param, Database.CurrentTransaction?.GetDbTransaction());
    }

    public DbView Views => new(this, Schema);

#pragma warning disable IDE1006 // Naming Styles
    public DbSet<__ReparationHistory> __ReparationHistories { get; set; }
#pragma warning restore IDE1006 // Naming Styles

    public DbSet<Attachment> Attachments { get; set; }

}

#pragma warning disable IDE1006 // Naming Styles
public class __ReparationHistory
#pragma warning restore IDE1006 // Naming Styles
{
    [Key]
    [MaxLength(1000)]
    public string ReparationId { get; set; }

    public DateTimeOffset Timestamps { get; set; }
}

public class Attachment
{
    [Key]
    public string Id { get; set; }

    [MaxLength(200)]
    public string FileName { get; set; }

    [MaxLength(200)]
    public string FileType { get; set; }

    public double FileSize { get; set; }

    [MaxLength(50)]
    public string AttachmentId { get; set; }

    public string DocumentId { get; set; }

    [MaxLength(50)]
    public string DocumentType { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    [MaxLength(100)]
    public string Key { get; set; }

    [MaxLength(100)]
    public string ThumbUrl { get; set; }

    [MaxLength(20)]
    public string Provider { get; set; }

    public bool IsPublic { get; set; }

    public string PublicUrl { get; set; }
}