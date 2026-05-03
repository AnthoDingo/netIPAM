using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netIPAM.Models;

/// <summary>Maps to phpIPAM table `ipTags`. State labels (Offline/Used/Reserved/DHCP).</summary>
[Table("ipTags")]
public class IpTag
{
    [Column("id")] public int Id { get; set; }
    [Column("type"), MaxLength(32)] public string? Type { get; set; }
    [Column("showtag")] public byte ShowTag { get; set; } = 1;
    [Column("bgcolor"), MaxLength(7)] public string? BgColor { get; set; } = "#000";
    [Column("fgcolor"), MaxLength(7)] public string? FgColor { get; set; } = "#fff";
    [Column("compress"), MaxLength(3)] public string Compress { get; set; } = "No";
    [Column("locked"), MaxLength(3)] public string Locked { get; set; } = "No";
    [Column("updateTag")] public bool UpdateTag { get; set; }
}
