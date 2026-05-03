using netIPAM.Enums;
using System.Numerics;

namespace netIPAM.Services;

/// <summary>
/// Port partiel de phpIPAM `class.Subnets.php`.
/// Couvre les fonctions les plus utilisées : adresses réseau / broadcast,
/// nombre d'hôtes, plage utilisable, validation, contenance.
/// </summary>
public static class SubnetCalculator
{
    public record SubnetInfo(
        string Network,
        string Broadcast,
        string FirstUsable,
        string LastUsable,
        BigInteger TotalAddresses,
        BigInteger UsableHosts,
        IpVersion Version,
        int Mask);

    /// <summary>
    /// Calcule les caractéristiques d'un sous-réseau à partir de l'adresse réseau (décimale)
    /// et du masque CIDR.
    /// </summary>
    public static SubnetInfo Describe(string networkDecimal, int mask)
    {
        if (!BigInteger.TryParse(networkDecimal, out var net))
            throw new FormatException("Invalid network decimal");

        IpVersion version = IpConverter.GuessVersion(networkDecimal);
        int totalBits = version == IpVersion.V4 ? 32 : 128;
        if (mask < 0 || mask > totalBits)
            throw new ArgumentOutOfRangeException(nameof(mask));

        int hostBits = totalBits - mask;
        BigInteger size = BigInteger.One << hostBits;          // 2^hostBits
        BigInteger maskBig = ((BigInteger.One << totalBits) - 1) ^ (size - 1);
        BigInteger network = net & maskBig;
        BigInteger broadcast = network + size - 1;

        BigInteger first, last, usable;
        if (version == IpVersion.V4 && mask < 31)
        {
            first = network + 1;
            last = broadcast - 1;
            usable = size - 2;
        }
        else
        {
            // IPv6 ou /31, /32 : pas de réservation broadcast
            first = network;
            last = broadcast;
            usable = size;
        }

        return new SubnetInfo(
            IpConverter.ToPresentation(network.ToString(), version),
            IpConverter.ToPresentation(broadcast.ToString(), version),
            IpConverter.ToPresentation(first.ToString(), version),
            IpConverter.ToPresentation(last.ToString(), version),
            size,
            usable,
            version,
            mask);
    }

    /// <summary>Vérifie si une adresse hôte appartient au sous-réseau donné.</summary>
    public static bool Contains(string networkDecimal, int mask, string hostDecimal)
    {
        if (!BigInteger.TryParse(networkDecimal, out var net)) return false;
        if (!BigInteger.TryParse(hostDecimal, out var host)) return false;

        IpVersion version = IpConverter.GuessVersion(networkDecimal);
        int totalBits = version == IpVersion.V4 ? 32 : 128;
        int hostBits = totalBits - mask;
        BigInteger maskBig = ((BigInteger.One << totalBits) - 1) ^ ((BigInteger.One << hostBits) - 1);
        return (host & maskBig) == (net & maskBig);
    }

    /// <summary>Format CIDR pour affichage.</summary>
    public static string ToCidr(string networkDecimal, int mask)
    {
        IpVersion v = IpConverter.GuessVersion(networkDecimal);
        return $"{IpConverter.ToPresentation(networkDecimal, v)}/{mask}";
    }
}
