using System.Net;
using System.Numerics;
using netIPAM.Enums;

namespace netIPAM.Services;

/// <summary>
/// Conversion entre les formats d'adresses utilisés par phpIPAM.
/// phpIPAM stocke :
///  - IPv4 : entier 32 bits non signé sous forme de chaîne décimale ("168427779")
///  - IPv6 : entier 128 bits sous forme de chaîne décimale (BigInteger)
/// Cette classe est l'équivalent .NET des fonctions PHP `Transform_to_decimal` /
/// `Transform_to_dotted` de phpIPAM (fichier `functions/classes/class.Subnets.php`).
/// </summary>
public static class IpConverter
{
    /// <summary>Convertit "192.168.1.1" ou "2001:db8::1" en chaîne décimale.</summary>
    public static string ToDecimal(string presentation)
    {
        if (string.IsNullOrWhiteSpace(presentation))
            throw new ArgumentException("IP address is empty", nameof(presentation));

        if (!System.Net.IPAddress.TryParse(presentation, out var addr))
            throw new FormatException($"Invalid IP address: {presentation}");

        byte[] bytes = addr.GetAddressBytes();
        // BigInteger ctor expects little-endian, IP bytes are big-endian → reverse and force unsigned
        byte[] be = new byte[bytes.Length + 1];
        for (int i = 0; i < bytes.Length; i++) be[bytes.Length - 1 - i] = bytes[i];
        // last byte is 0 → forces non-negative
        return new BigInteger(be).ToString();
    }

    /// <summary>Convertit la chaîne décimale phpIPAM en notation pointée.</summary>
    public static string ToPresentation(string decimalValue, IpVersion version)
    {
        if (!BigInteger.TryParse(decimalValue, out var big))
            throw new FormatException($"Invalid decimal IP: {decimalValue}");

        int byteLength = version == IpVersion.V4 ? 4 : 16;
        byte[] bytes = big.ToByteArray();
        // Strip the optional sign byte and pad to required length, then reverse to big-endian
        if (bytes.Length > byteLength && bytes[^1] == 0)
            bytes = bytes[..^1];

        byte[] be = new byte[byteLength];
        for (int i = 0; i < bytes.Length && i < byteLength; i++)
            be[byteLength - 1 - i] = bytes[i];

        return new System.Net.IPAddress(be).ToString();
    }

    /// <summary>Devine la version IP en fonction de la magnitude de l'entier.</summary>
    public static IpVersion GuessVersion(string decimalValue)
    {
        if (!BigInteger.TryParse(decimalValue, out var big)) return IpVersion.V4;
        // 2^32 - 1 = 4294967295 — anything larger is IPv6
        return big > new BigInteger(uint.MaxValue) ? IpVersion.V6 : IpVersion.V4;
    }
}
