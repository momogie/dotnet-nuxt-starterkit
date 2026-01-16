namespace Modules.Identity.Entities.Views;

[SqlView]
public class WorkspaceView : IDataTable
{
    public string Id { get; set; }
    [Filterable]
    [DataColumn(Name = "Name")]
    public string Name { get; set; }
    public string NormalizedName { get; set; }
    public string ConcurrencyStamp { get; set; }

    public List<RoleClaimView> Claims { get; set; }
}