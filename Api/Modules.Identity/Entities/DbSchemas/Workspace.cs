namespace Modules.Identity.Entities.DbSchemas;

public class Workspace
{
    [Key]
    [MaxLength(100)]
    public string Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(255)]
    public string Description { get; set; }
}

