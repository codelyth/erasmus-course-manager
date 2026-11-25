using System.Security.Cryptography;

namespace ErasmusCourseManager.WinForms.Services;

public class PasswordHasher
{
    private const int Iterations = 600_000;
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int MinimumPasswordLength = 8;
    private const string FormatVersion = "PBKDF2-SHA256";

    public string Hash(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        if (string.IsNullOrWhiteSpace(password)
            || password.Length < MinimumPasswordLength)
        {
            throw new ArgumentException(
                $"Password must contain at least " +
                $"{MinimumPasswordLength} characters.",
                nameof(password));
        }

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return string.Join(
            "$",
            FormatVersion,
            Iterations.ToString(
                System.Globalization.CultureInfo.InvariantCulture),
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash));
    }

    public bool Verify(string password, string storedHash)
    {
        if (password is null || string.IsNullOrWhiteSpace(storedHash))
            return false;

        string[] parts = storedHash.Split('$');

        if (parts.Length != 4
            || parts[0] != FormatVersion
            || !int.TryParse(parts[1], out int iterations)
            || iterations != Iterations)
        {
            return false;
        }

        try
        {
            byte[] salt = Convert.FromBase64String(parts[2]);
            byte[] expectedHash = Convert.FromBase64String(parts[3]);

            if (salt.Length != SaltSize || expectedHash.Length != HashSize)
                return false;

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(
                actualHash, expectedHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}