using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhpIpamNet.Domain.Entities;

[Table("requests")]
public class Request
{
    [Column("id")] public int Id { get; set; }
    [Column("subnetId")] public int? SubnetId { get; set; }
    [Column("ip_addr"), MaxLength(100)] public string? IpAddr { get; set; }
    [Column("description"), MaxLength(64)] public string? Description { get; set; }
    [Column("mac"), MaxLength(20)] public string? Mac { get; set; }
    [Column("hostname"), MaxLength(255)] public string? Hostname { get; set; }
    [Column("state")] public int? State { get; set; } = 2;
    [Column("owner"), MaxLength(128)] public string? Owner { get; set; }
    [Column("requester"), MaxLength(128)] public string? Requester { get; set; }
    [Column("comment"), Column(TypeName = "text")] public string? Comment { get; set; }
    [Column("processed")] public byte? Processed { get; set; }
    [Column("accepted")] public byte? Accepted { get; set; }
    [Column("adminComment"), Column(TypeName = "text")] public string? AdminComment { get; set; }
}
