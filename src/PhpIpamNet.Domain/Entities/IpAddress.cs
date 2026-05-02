using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhpIpamNet.Domain.Entities;

/// <summary>
/// Maps to phpIPAM table `ipaddresses`.
/// 'ip_addr' is the host address packed as a decimal integer string (32-bit for IPv4, 128-bit for IPv6).
/// </summary>
[Table("ipaddresses")]
public class IpAddress
{
    [Column("id")]
    public int Id { get; set; }

    [Column("subnetId")]
    public int? SubnetId { get; set; }

    [Column("ip_addr"), Required, MaxLength(100)]
    public string IpAddr { get; set; } = string.Empty;

    [Column("is_gateway")]
    public bool IsGateway { get; set; }

    [Column("description"), MaxLength(64)]
    public string? Description { get; set; }

    [Column("hostname"), MaxLength(255)]
    public string? Hostname { get; set; }

    [Column("mac"), MaxLength(20)]
    public string? Mac { get; set; }

    [Column("owner"), MaxLength(128)]
    public string? Owner { get; set; }

    [Column("state")]
    public int? State { get; set; } = 2;

    [Column("switch")]
    public int? Switch { get; set; }

    [Column("location")]
    public int? Location { get; set; }

    [Column("port"), MaxLength(32)]
    public string? Port { get; set; }

    [Column("note"), Column(TypeName = "text")]
    public string? Note { get; set; }

    [Column("lastSeen")]
    public DateTime? LastSeen { get; set; }

    [Column("excludePing")]
    public bool ExcludePing { get; set; }

    [Column("PTRignore")]
    public bool PtrIgnore { get; set; }

    [Column("PTR")]
    public int? Ptr { get; set; } = 0;

    [Column("firewallAddressObject"), MaxLength(100)]
    public string? FirewallAddressObject { get; set; }

    [Column("editDate")]
    public DateTime? EditDate { get; set; }

    [Column("customer_id")]
    public int? CustomerId { get; set; }

    public Subnet? Subnet { get; set; }
    public Customer? Customer { get; set; }
}
