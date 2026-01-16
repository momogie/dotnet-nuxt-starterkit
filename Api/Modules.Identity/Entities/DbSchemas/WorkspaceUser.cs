namespace Modules.Identity.Entities.DbSchemas;

public class WorkspaceUser
{
    [Key]
    [MaxLength(100)]
    public string Id { get; set; }

    [MaxLength(100)]
    public string WorkspaceId { get; set; }

    [MaxLength(100)]
    public string UserId { get; set; }

    public bool IsActive { get; set; }
}