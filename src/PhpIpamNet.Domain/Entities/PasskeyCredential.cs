using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhpIpamNet.Domain.Entities;

/// <summary>
/// Stocke un credential WebAuthn (passkey) lié à un utilisateur.
/// Un utilisateur peut avoir plusieurs passkeys (téléphone, ordinateur, clé physique).
/// </summary>
[Table("passkey_credentials")]
public class PasskeyCredential
{
    [Column("id")]
    public int Id { get; set; }

    /// <summary>FK vers users.id</summary>
    [Column("user_id")]
    public int UserId { get; set; }

    /// <summary>
    /// ID de credential au format bytes (base64url côté browser).
    /// Envoyé par le browser lors de chaque authentification pour identifier le credential.
    /// </summary>
    [Column("descriptor_id")]
    public byte[] DescriptorId { get; set; } = [];

    /// <summary>Clé publique COSE encodée en CBOR — fournie par le browser à l'enregistrement.</summary>
    [Column("public_key")]
    public byte[] PublicKey { get; set; } = [];

    /// <summary>
    /// User handle opaque (non-PII). Utilisé par l'authenticator pour retrouver le compte.
    /// Ici = 4 bytes du userId big-endian, déterministe.
    /// </summary>
    [Column("user_handle")]
    public byte[] UserHandle { get; set; } = [];

    /// <summary>Compteur de signature — remonte à chaque assertion. Détecte les clones.</summary>
    [Column("sign_count")]
    public uint SignCount { get; set; }

    /// <summary>Format d'attestation ("packed", "none", "fido-u2f"…)</summary>
    [Column("reg_type"), MaxLength(32)]
    public string? RegType { get; set; }

    /// <summary>AAGUID de l'authenticator (identifie le modèle de hardware).</summary>
    [Column("aaguid"), MaxLength(64)]
    public string? AaGuid { get; set; }

    /// <summary>Nom lisible donné par l'utilisateur à cette clé (ex. "iPhone 15").</summary>
    [Column("device_name"), MaxLength(128)]
    public string? DeviceName { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("last_used")]
    public DateTime? LastUsed { get; set; }

    public User? User { get; set; }
}
