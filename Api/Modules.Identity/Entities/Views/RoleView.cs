namespace Modules.Identity.Entities.Views;

[SqlView]
public class RoleView : IDataTable
{
    public string Id { get; set; }
    [Filterable]
    [DataColumn(Name = "Name")]
    public string Name { get; set; }
    public string NormalizedName { get; set; }
    public string ConcurrencyStamp { get; set; }

    public List<RoleClaimView> Claims { get; set; }
}

public class RoleClaimView
{
    public string Type { get; set; } 
    public string Value { get; set; }
}