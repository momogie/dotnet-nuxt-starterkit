using Shared;

namespace Modules.Common.Entities.Views;

[SqlView]
public class ImportView : IDataTable
{
    public string Id { get; set; }

    [Filterable]
    [DataColumn(Name = "Date")]
    public long CreatedAt { get; set; }
    [Filterable]
    [DataColumn(Name = "File Name")]
    public string FileName { get; set; }
    [Filterable]
    [DataColumn(Name = "Description")]
    public string Description { get; set; }
    [Filterable]
    [DataColumn(Name = "Rows")]
    public int Rows { get; set; }
    [Filterable]
    [DataColumn(Name = "Valid Rows")]
    public int ValidRows { get; set; }
    [Filterable]
    [DataColumn(Name = "Invalid Rows")]
    public int InvalidRows { get; set; }
    [Filterable]
    [DataColumn(Name = "Queue")]
    public int Queue { get; set; }
    [Filterable]
    [DataColumn(Name = "Processed")]
    public int Processed { get; set; }
    [Filterable]
    [DataColumn(Name = "Skipped")]
    public int Skipped { get; set; }
    [Filterable]
    [DataColumn(Name = "Parallel")]
    public bool IsParallelEnabled { get; set; }
    [Filterable]
    [DataColumn(Name = "Status")]
    public bool Status { get; set; }
    public string UserId { get; set; }

}
