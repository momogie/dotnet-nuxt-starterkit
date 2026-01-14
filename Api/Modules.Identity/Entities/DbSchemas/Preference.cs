namespace Modules.Identity.Entities.DbSchemas;

public class Preference
{
    [Key]
    [MaxLength(100)]
    public string UserId { get; set; }

    [MaxLength(100)]
    public string Key { get; set; }

    public string Value { get; set; }
}
