using Microsoft.AspNetCore.Identity;

namespace Modules.Identity.Entities.DbSchemas;

public class User : IdentityUser<string>
{
    [MaxLength(100)]
    public string Name { get; set; }
    //[MaxLength(40)]
    //public string RoleId { get; set; }
    [MaxLength(500)]
    public string ImageUrl { get; set; }
    public bool IsActive { get; set; }
}
