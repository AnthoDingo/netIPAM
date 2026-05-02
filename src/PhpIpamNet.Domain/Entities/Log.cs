using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhpIpamNet.Domain.Entities;

[Table("logs")]
public class Log
{
    [Column("id")] public int Id { get; set; }
    [Column("severity")] public int? Severity { get; set; }
    [Column("date"), MaxLength(32)] public string? Date { get; set; }
    [Column("username"), MaxLength(255)] public string? Username { get; set; }
    [Column("ipaddr"), MaxLength(64)] public string? IpAddr { get; set; }
    [Column("command"), Column(TypeName = "text")] public string? Command { get; set; }
    [Column("details"), Column(TypeName = "text")] public string? Details { get; set; }
}
