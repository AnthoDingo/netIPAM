using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netIPAM.Models;

/// <summary>Maps to phpIPAM table `vlanDomains` (L2 domains).</summary>
[Table("vlanDomains")]
public class VlanDomain
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name"), MaxLength(64)]
    public string? Name { get; set; }

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [Column("permissions"), MaxLength(128)]
    public string? Permissions { get; set; }

    public ICollection<Vlan> Vlans { get; set; } = new List<Vlan>();
}
