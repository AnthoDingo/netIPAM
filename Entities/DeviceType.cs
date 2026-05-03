using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netIPAM.Entities;

/// <summary>Maps to phpIPAM table `deviceTypes`. PK column is `tid`.</summary>
[Table("deviceTypes")]
public class DeviceType
{
    [Column("tid")] public int Tid { get; set; }
    [Column("tname"), MaxLength(128)] public string? Tname { get; set; }
    [Column("tdescription"), MaxLength(128)] public string? Tdescription { get; set; }
}
