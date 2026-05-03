using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netIPAM.Entities;

/// <summary>
/// Permission d'un sujet (utilisateur ou groupe) sur une ressource réseau.
///
/// SubjectType : "user" | "group"
/// EntityType  : "vlan" | "l2domain" | "vrf" | "device"
/// Level       : 0=aucun, 1=lecture, 2=écriture, 3=admin
///
/// Une ligne absente équivaut à level=0 (aucun accès).
/// Les permissions de groupe se cumulent avec les permissions individuelles
/// (la valeur la plus haute l'emporte — résolue par PermissionService).
/// </summary>
[Table("entity_permissions")]
public class EntityPermission
{
    [Column("id")]
    public int Id { get; set; }

    /// <summary>"user" | "group"</summary>
    [Column("subject_type"), MaxLength(8), Required]
    public string SubjectType { get; set; } = string.Empty;

    [Column("subject_id")]
    public int SubjectId { get; set; }

    /// <summary>"vlan" | "l2domain" | "vrf" | "device"</summary>
    [Column("entity_type"), MaxLength(16), Required]
    public string EntityType { get; set; } = string.Empty;

    [Column("entity_id")]
    public int EntityId { get; set; }

    /// <summary>0=aucun, 1=lecture, 2=écriture, 3=admin</summary>
    [Column("level")]
    public int Level { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

/// <summary>Constantes partagées entre service et UI.</summary>
public static class SubjectTypes
{
    public const string User  = "user";
    public const string Group = "group";
}

public static class EntityTypes
{
    public const string Vlan     = "vlan";
    public const string L2Domain = "l2domain";
    public const string Vrf      = "vrf";
    public const string Device   = "device";
}

public static class PermissionLevels
{
    public const int None  = 0;
    public const int Read  = 1;
    public const int Write = 2;
    public const int Admin = 3;

    public static string Label(int level) => level switch
    {
        1 => "Lecture",
        2 => "Écriture",
        3 => "Admin",
        _ => "Aucun"
    };
}
