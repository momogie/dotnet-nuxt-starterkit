using Shared;

namespace Modules.Reporting.Entities.Views;

[SqlView]
public class ReportTemplateView : IDataTable
{
    public string Id { get; set; }

    [Filterable]
    [DataColumn(Name = "Name")]
    public string Name { get; set; }

    [Filterable]
    [DataColumn(Name = "Description")]
    public string Description { get; set; }

    /// <summary>
    /// XLSX | CSV | FAST REPORT
    /// </summary>
    [Filterable]
    [DataColumn(Name = "Type")]
    public string Type { get; set; }

    [Filterable]
    [DataColumn(Name = "Active")]
    public bool IsActive { get; set; }
}
