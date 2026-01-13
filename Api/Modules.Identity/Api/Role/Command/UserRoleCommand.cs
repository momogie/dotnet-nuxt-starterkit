namespace Modules.Identity.Api.Role.Command;

public class UserRoleCommand : ICommand
{
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }

    public bool IsStrictDataAccess { get; set; }

    public string[] Privileges { get; set; }

    public string[] WorkLocationIds { get; set; }
    public string[] PayrollGroupIds { get; set; }
    public string[] MainOrganizationIds { get; set; }
    public string[] SubOrganizationIds { get; set; }
}
