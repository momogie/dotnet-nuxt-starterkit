using System.ComponentModel.DataAnnotations;

namespace Modules.Reporting.Api.Commands;

public class ReportTemplateCommand
{
    [Required]
    [Display(Name = "Data Source")]
    public string DataSource { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string Type { get; set; }
    [Required]
    [Display(Name = "Template File")]
    public string AttachmentId { get; set; }
}
