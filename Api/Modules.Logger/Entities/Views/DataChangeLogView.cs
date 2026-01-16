using Shared;
using System.ComponentModel.DataAnnotations;

namespace Modules.Logger.Entities.Views;

[SqlView]
public class DataChangeLogView : IDataTable
{
    public long Id { get; set; }

    [Filterable]
    [DataColumn(Name = "Date")]
    public DateTime Date { get; set; }

    public string UserId { get; set; }

    [Filterable]
    [DataColumn(Name = "UserName")]
    public string UserName { get; set; }

    [Filterable]
    [DataColumn(Name = "Name")]
    public string Name { get; set; }

    public string UserAgent { get; set; }

    [Filterable]
    [DataColumn(Name = "Remote Addr")]
    public string RemoteAddr { get; set; }

    public string Method { get; set; }

    public string RequestPath { get; set; }

    [Filterable]
    [DataColumn(Name = "Action")]
    /// <summary>
    /// CREATE, UPDATE, DELETE
    /// </summary>
    public string Action { get; set; }

    [Filterable]
    [DataColumn(Name = "Document")]
    public string DocumentType { get; set; }

    public string EntityId { get; set; }

    [Filterable]
    [DataColumn(Name = "Reference")]
    public string ReferenceId { get; set; }

    public string Data { get; set; }

    public long ElapsedMilliseconds { get; set; }
}
