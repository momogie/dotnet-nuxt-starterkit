namespace Modules.Identity.Api.Role.Command;

public class UserRoleCommand : ICommand
{
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsStrictDataAccess { get; set; }
    public List<UserRoleClaimCommand> Claims { get; set; }
}

public class UserRoleClaimCommand
{
    public string Type { get; set; }
    public string Value { get; set; }
}