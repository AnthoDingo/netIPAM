using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhpIpamNet.Domain.Entities;

/// <summary>Maps to phpIPAM table `vrf`. PK column is `vrfId`.</summary>
[Table("vrf")]
public class Vrf
{
    [Column("vrfId")]
    public int VrfId { get; set; }

    [Column("name"), Required, MaxLength(32)]
    public string Name { get; set; } = string.Empty;

    [Column("rd"), MaxLength(32)]
    public string? Rd { get; set; }

    [Column("description"), MaxLength(256)]
    public string? Description { get; set; }

    [Column("sections"), MaxLength(128)]
    public string? Sections { get; set; }

    [Column("editDate")]
    public DateTime? EditDate { get; set; }

    [Column("customer_id")]
    public int? CustomerId { get; set; }

    public Customer? Customer { get; set; }
    public ICollection<Subnet> Subnets { get; set; } = new List<Subnet>();
}
