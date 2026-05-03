using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netIPAM.Entities;

[Table("locations")]
public class Location
{
    [Column("id")] public int Id { get; set; }
    [Column("name"), Required, MaxLength(128)] public string Name { get; set; } = string.Empty;
    [Column("description"), Column(TypeName = "text")] public string? Description { get; set; }
    [Column("address"), MaxLength(128)] public string? Address { get; set; }
    [Column("lat"), MaxLength(31)] public string? Lat { get; set; }
    [Column("long"), MaxLength(31)] public string? Long { get; set; }
}
