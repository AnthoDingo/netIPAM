using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netIPAM.Entities;

[Table("usersAuthMethod")]
public class UserAuthMethod
{
    [Column("id")] public int Id { get; set; }
    [Column("type"), MaxLength(16)] public string Type { get; set; } = "local";
    [Column("params"), Column(TypeName = "text")] public string? Params { get; set; }
    [Column("protected"), MaxLength(3)] public string Protected { get; set; } = "Yes";
    [Column("description"), Column(TypeName = "text")] public string? Description { get; set; }
}
