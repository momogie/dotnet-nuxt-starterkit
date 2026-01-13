using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger.Entities.DbSchema;

public class ErrorLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public DateTime Date { get; set; }

    public string Message { get; set; }

    [MaxLength(255)]
    public string Level { get; set; }

    public string StackTrace { get; set; }

    public string InnerStackTrace { get; set; }

    public long? UserId { get; set; }

    [MaxLength(255)]
    public string UserName { get; set; }

    [MaxLength(255)]
    public string Name { get; set; }

    public string UserAgent { get; set; }

    [MaxLength(255)]
    public string RemoteAddr { get; set; }

    public int StatusCode { get; set; }

    [MaxLength(10)]
    public string Method { get; set; }

    [MaxLength(2000)]
    public string RequestPath { get; set; }

    public long ElapsedMilliseconds { get; set; }
}
