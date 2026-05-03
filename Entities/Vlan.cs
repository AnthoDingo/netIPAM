using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netIPAM.Entities;

/// <summary>Maps to phpIPAM table `vlans`. PK column is `vlanId`.</summary>
[Table("vlans")]
public class Vlan
{
    [Column("vlanId")]
    public int VlanId { get; set; }

    [Column("domainId")]
    public int DomainId { get; set; } = 1;

    [Column("name"), Required, MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Column("number")]
    public int? Number { get; set; }

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [Column("editDate")]
    public DateTime? EditDate { get; set; }

    [Column("customer_id")]
    public int? CustomerId { get; set; }

    public VlanDomain? Domain { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<Subnet> Subnets { get; set; } = new List<Subnet>();
}
