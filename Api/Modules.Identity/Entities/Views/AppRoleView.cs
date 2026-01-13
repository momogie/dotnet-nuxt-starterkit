namespace Modules.Identity.Entities.Views;

[SqlView]
public class AppRoleView : IDataTable
{
    public string Code { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsAdministrator { get; set; }

    public string[] Privileges { get; set; }
}
