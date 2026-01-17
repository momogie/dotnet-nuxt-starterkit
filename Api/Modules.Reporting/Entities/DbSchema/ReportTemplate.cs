using System.ComponentModel.DataAnnotations;

namespace Modules.Reporting.Entities.DbSchema;

public class ReportTemplate
{
    [Key]
    public string Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(255)]
    public string Description { get; set; }

    /// <summary>
    /// XLSX | CSV | FAST REPORT
    /// </summary>
    [MaxLength(50)]
    public string Type { get; set; }

    public bool IsActive { get; set; }
}
