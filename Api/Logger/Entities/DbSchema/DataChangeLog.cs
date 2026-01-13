using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logger.Entities.DbSchema;

public class DataChangeLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public DateTime Date { get; set; }

    [MaxLength(36)]
    public string UserId { get; set; }

    [MaxLength(255)]
    public string UserName { get; set; }

    [MaxLength(255)]
    public string Name { get; set; }

    public string UserAgent { get; set; }

    [MaxLength(255)]
    public string RemoteAddr { get; set; }

    [MaxLength(10)]
    public string Method { get; set; }

    [MaxLength(2000)]
    public string RequestPath { get; set; }

    /// <summary>
    /// CREATE, UPDATE, DELETE
    /// </summary>
    [MaxLength(10)]
    public string Action { get; set; }

    [MaxLength(50)]
    public string DocumentType { get; set; }

    [MaxLength(50)]
    public string EntityId { get; set; }

    [MaxLength(100)]
    public string ReferenceId { get; set; }

    public string Data { get; set; }

    public long ElapsedMilliseconds { get; set; }
}
