using Shared;
using System.ComponentModel.DataAnnotations;

namespace Modules.Logger.Entities.Views;

[SqlView]
public class ErrorLogView : IDataTable
{
    public long Id { get; set; }

    [Filterable]
    [DataColumn(Name = "Date")]
    public long Date { get; set; }

    [Filterable]
    [DataColumn(Name = "UserName")]
    public string UserName { get; set; }

    [Filterable]
    [DataColumn(Name = "Name")]
    public string Name { get; set; }

    [Filterable]
    [DataColumn(Name = "Message")]
    public string Message { get; set; }

    public string Level { get; set; }

    public string StackTrace { get; set; }

    public string InnerStackTrace { get; set; }

    public string UserId { get; set; }

    public string UserAgent { get; set; }

    [Filterable]
    [DataColumn(Name = "Remote Addr")]
    public string RemoteAddr { get; set; }

    public int StatusCode { get; set; }

    [Filterable]
    [DataColumn(Name = "Method")]
    public string Method { get; set; }

    [Filterable]
    [DataColumn(Name = "Request Path")]
    public string RequestPath { get; set; }

    public long ElapsedMilliseconds { get; set; }
}
