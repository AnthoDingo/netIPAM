using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netIPAM.Entities;

/// <summary>
/// Maps to phpIPAM table `users`.
/// IMPORTANT: original phpIPAM stores `password` as crypt() SHA-512 ($6$rounds=3000$...).
/// In netIPAM new users get a BCrypt hash. To keep compatibility with imported phpIPAM
/// databases, see <see cref="netIPAM.Identity.LegacyCryptVerifier"/>.
/// </summary>
[Table("users")]
public class User
{
    [Column("id")]
    public int Id { get; set; }

    [Column("username"), Required, MaxLength(255)]
    public string Username { get; set; } = string.Empty;

    [Column("authMethod")]
    public int AuthMethod { get; set; } = 1;

    [Column("password"), MaxLength(128)]
    public string? Password { get; set; }

    [Column("groups"), MaxLength(1024)]
    public string? Groups { get; set; }

    [Column("role", TypeName = "text")]
    public string? Role { get; set; }

    [Column("real_name"), MaxLength(128)]
    public string? RealName { get; set; }

    [Column("email"), MaxLength(254)]
    public string? Email { get; set; }

    [Column("domainUser")]
    public bool DomainUser { get; set; }

    [Column("widgets"), MaxLength(1024)]
    public string? Widgets { get; set; } = "statistics;favourite_subnets;changelog;top10_hosts_v4";

    [Column("lang")]
    public int? Lang { get; set; } = 9;

    [Column("favourite_subnets"), MaxLength(1024)]
    public string? FavouriteSubnets { get; set; }

    [Column("disabled"), MaxLength(3)]
    public string Disabled { get; set; } = "No";

    [Column("mailNotify"), MaxLength(3)]
    public string MailNotify { get; set; } = "No";

    [Column("mailChangelog"), MaxLength(3)]
    public string MailChangelog { get; set; } = "No";

    [Column("passChange"), MaxLength(3)]
    public string PassChange { get; set; } = "No";

    [Column("editDate")]
    public DateTime? EditDate { get; set; }

    [Column("lastLogin")]
    public DateTime? LastLogin { get; set; }

    [Column("lastActivity")]
    public DateTime? LastActivity { get; set; }

    [Column("compressOverride"), MaxLength(16)]
    public string CompressOverride { get; set; } = "default";

    [Column("hideFreeRange")]
    public bool HideFreeRange { get; set; }

    [Column("menuType"), MaxLength(8)]
    public string MenuType { get; set; } = "Dynamic";

    [Column("menuCompact")]
    public bool MenuCompact { get; set; } = true;

    [Column("2fa")]
    public bool TwoFactor { get; set; }

    [Column("2fa_secret"), MaxLength(32)]
    public string? TwoFactorSecret { get; set; }

    [Column("theme"), MaxLength(32)]
    public string? Theme { get; set; }

    [Column("token"), MaxLength(24)]
    public string? Token { get; set; }

    [Column("token_valid_until")]
    public DateTime? TokenValidUntil { get; set; }

    [Column("module_permissions"), MaxLength(255)]
    public string ModulePermissions { get; set; } =
        "{\"vlan\":\"1\",\"l2dom\":\"1\",\"vrf\":\"1\",\"pdns\":\"1\",\"circuits\":\"1\",\"racks\":\"1\",\"nat\":\"1\",\"pstn\":\"1\",\"customers\":\"1\",\"locations\":\"1\",\"devices\":\"1\",\"routing\":\"1\",\"vaults\":\"1\"}";

    [Column("compress_actions")]
    public bool CompressActions { get; set; } = true;
}
