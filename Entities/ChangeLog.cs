using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netIPAM.Entities;

/// <summary>Maps to phpIPAM table `changelog`. PK column is `cid`.</summary>
[Table("changelog")]
public class ChangeLog
{
    [Column("cid")] public int Cid { get; set; }
    [Column("ctype"), MaxLength(16)] public string Ctype { get; set; } = string.Empty;
    [Column("coid")] public int Coid { get; set; }
    [Column("cuser")] public int Cuser { get; set; }
    [Column("caction"), MaxLength(16)] public string Caction { get; set; } = "edit";
    [Column("cresult"), MaxLength(8)] public string Cresult { get; set; } = "success";
    [Column("cdate")] public DateTime Cdate { get; set; }
    [Column("cdiff", TypeName = "text")] public string? Cdiff { get; set; }
}
