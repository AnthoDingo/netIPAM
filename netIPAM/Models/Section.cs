using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netIPAM.Models;

/// <summary>
/// Maps to phpIPAM table `sections`.
/// Top-level grouping used to organize subnets (e.g. "Customers", "IPv6").
/// </summary>
[Table("sections")]
public class Section
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name"), Required, MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [Column("masterSection")]
    public int? MasterSection { get; set; } = 0;

    /// <summary>JSON-encoded {groupId:permissionLevel}, e.g. {"3":"1","2":"2"}</summary>
    [Column("permissions"), MaxLength(1024)]
    public string? Permissions { get; set; }

    [Column("strictMode")]
    public bool StrictMode { get; set; } = true;

    [Column("subnetOrdering"), MaxLength(16)]
    public string? SubnetOrdering { get; set; }

    [Column("order")]
    public int? Order { get; set; }

    [Column("editDate")]
    public DateTime? EditDate { get; set; }

    [Column("showSubnet")]
    public bool ShowSubnet { get; set; } = true;

    [Column("showVLAN")]
    public bool ShowVlan { get; set; }

    [Column("showVRF")]
    public bool ShowVrf { get; set; }

    [Column("showSupernetOnly")]
    public bool ShowSupernetOnly { get; set; }

    [Column("DNS"), MaxLength(128)]
    public string? Dns { get; set; }

    public ICollection<Subnet> Subnets { get; set; } = new List<Subnet>();
}
