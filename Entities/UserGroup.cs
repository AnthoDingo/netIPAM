using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netIPAM.Entities;

/// <summary>Maps to phpIPAM table `userGroups`. PK column is `g_id`.</summary>
[Table("userGroups")]
public class UserGroup
{
    [Column("g_id")]
    public int GId { get; set; }

    [Column("g_name"), MaxLength(32)]
    public string? GName { get; set; }

    [Column("g_desc"), MaxLength(1024)]
    public string? GDesc { get; set; }

    [Column("editDate")]
    public DateTime? EditDate { get; set; }
}
