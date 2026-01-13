namespace Modules.Identity.Api.Role.Command;

public class UserRolePrivilegeCommand : ICommand
{
    [Required]
    [MaxLength(100)]
    public string Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(200)]
    public string Description { get; set; }

    [MaxLength(100)]
    public string Group { get; set; }
}
