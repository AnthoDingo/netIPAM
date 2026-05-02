namespace PhpIpamNet.Infrastructure.Identity;

/// <summary>
/// Vérification des hashs hérités de phpIPAM : crypt() SHA-512 (format $6$[rounds=N]$salt$hash).
/// .NET ne fournit pas crypt() en natif. Pour activer la compatibilité avec une base
/// phpIPAM existante (import / migration progressive), implémentez une des options :
///
///  1. Référencer un paquet NuGet de type Isopoh.Cryptography.Argon2 ou
///     CryptSharp (qui fournit Crypt.Sha512Crypt) puis appeler Verify ici.
///
///  2. P/Invoke vers libcrypt sous Linux (`crypt_r`).
///
///  3. Au premier login réussi, ré-hasher en BCrypt et persister, ce qui éteint
///     progressivement le besoin du verifier legacy.
///
/// Tant que cette méthode renvoie false, les comptes hérités ne pourront pas se connecter
/// par mot de passe — ce qui est volontairement explicite.
/// </summary>
internal static class LegacyCryptVerifier
{
    public static bool VerifySha512Crypt(string password, string hash)
    {
        // TODO : brancher CryptSharp ou autre lib selon la stratégie de migration choisie.
        // Exemple avec CryptSharp (référence NuGet à ajouter dans Infrastructure.csproj) :
        //   return CryptSharp.Crypter.CheckPassword(password, hash);
        return false;
    }
}
