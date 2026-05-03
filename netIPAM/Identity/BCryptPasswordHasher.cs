namespace netIPAM.Identity;

public interface IPasswordHasher
{
    string Hash(string password);

    /// <summary>
    /// Vérifie un mot de passe.
    /// Si le hash est au format BCrypt, vérification directe.
    /// Si le hash est au format crypt() SHA-512 ($6$rounds=...$...) hérité de phpIPAM,
    /// on délègue à <see cref="LegacyCryptVerifier"/> (à compléter selon vos besoins).
    /// </summary>
    bool Verify(string password, string hash);
}

public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);

    public bool Verify(string password, string hash)
    {
        if (string.IsNullOrEmpty(hash)) return false;

        // BCrypt : $2a$, $2b$, $2y$
        if (hash.StartsWith("$2"))
        {
            try { return BCrypt.Net.BCrypt.Verify(password, hash); }
            catch { return false; }
        }

        // SHA-512 crypt (legacy phpIPAM)
        if (hash.StartsWith("$6$"))
            return LegacyCryptVerifier.VerifySha512Crypt(password, hash);

        return false;
    }
}
