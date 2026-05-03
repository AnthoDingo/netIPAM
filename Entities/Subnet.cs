using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netIPAM.Entities;

/// <summary>
/// Maps to phpIPAM table `subnets`.
/// IPv4: 'subnet' is the network address packed as a 32-bit integer (decimal string).
/// IPv6: 'subnet' is the 128-bit integer as a decimal string (e.g. "33639554990...").
/// 'mask' is the prefix length (CIDR), 0..128.
/// </summary>
[Table("subnets")]
public class Subnet
{
    [Column("id")]
    public int Id { get; set; }

    [Column("subnet"), MaxLength(255)]
    public string? SubnetAddress { get; set; }

    [Column("mask"), MaxLength(3)]
    public string? Mask { get; set; }

    [Column("sectionId")]
    public int? SectionId { get; set; }

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [Column("linked_subnet")]
    public int? LinkedSubnet { get; set; }

    [Column("firewallAddressObject"), MaxLength(100)]
    public string? FirewallAddressObject { get; set; }

    [Column("vrfId")]
    public int? VrfId { get; set; }

    [Column("masterSubnetId")]
    public int MasterSubnetId { get; set; } = 0;

    [Column("allowRequests")]
    public bool AllowRequests { get; set; }

    [Column("vlanId")]
    public int? VlanId { get; set; }

    [Column("showName")]
    public bool ShowName { get; set; }

    [Column("device")]
    public int? Device { get; set; } = 0;

    [Column("permissions"), MaxLength(1024)]
    public string? Permissions { get; set; }

    [Column("pingSubnet")]
    public bool PingSubnet { get; set; }

    [Column("discoverSubnet")]
    public bool DiscoverSubnet { get; set; }

    [Column("resolveDNS")]
    public bool ResolveDns { get; set; }

    [Column("DNSrecursive")]
    public bool DnsRecursive { get; set; }

    [Column("DNSrecords")]
    public bool DnsRecords { get; set; }

    [Column("nameserverId")]
    public int? NameserverId { get; set; } = 0;

    [Column("scanAgent")]
    public int? ScanAgent { get; set; }

    [Column("customer_id")]
    public int? CustomerId { get; set; }

    [Column("isFolder")]
    public bool IsFolder { get; set; }

    [Column("isFull")]
    public bool IsFull { get; set; }

    [Column("isPool")]
    public bool IsPool { get; set; }

    [Column("state")]
    public int? State { get; set; } = 2;

    [Column("threshold")]
    public int? Threshold { get; set; } = 0;

    [Column("location")]
    public int? Location { get; set; }

    [Column("editDate")]
    public DateTime? EditDate { get; set; }

    [Column("lastScan")]
    public DateTime? LastScan { get; set; }

    [Column("lastDiscovery")]
    public DateTime? LastDiscovery { get; set; }

    public Section? Section { get; set; }
    public Vlan? Vlan { get; set; }
    public Vrf? Vrf { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<IpAddress> IpAddresses { get; set; } = new List<IpAddress>();
    public ICollection<Subnet> Children { get; set; } = new List<Subnet>();
}
