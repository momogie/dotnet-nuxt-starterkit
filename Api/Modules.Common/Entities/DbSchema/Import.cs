using System.ComponentModel.DataAnnotations;

namespace Modules.Common.Entities;

public class Import
{
    [Key]
    [MaxLength(100)]
    public string Id { get; set; }
}
