using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netIPAM.Models;

[Table("nameservers")]
public class Nameserver
{
    [Column("id")] public int Id { get; set; }
    [Column("name"), Required, MaxLength(255)] public string Name { get; set; } = string.Empty;
    [Column("namesrv1"), MaxLength(255)] public string? Namesrv1 { get; set; }
    [Column("description", TypeName = "text")] public string? Description { get; set; }
    [Column("permissions"), MaxLength(128)] public string? Permissions { get; set; }
    [Column("editDate")] public DateTime? EditDate { get; set; }
}
