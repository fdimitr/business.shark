using System;
using System.Security.Cryptography;
using System.Text;

public static class PasswordHelper
{
    // Parameters — can be adjusted
    private const int SaltSize = 16;          // bytes
    private const int HashSize = 32;          // bytes (256 bits)
    private const int Iterations = 150_000;   // reasonable value "not overkill"

    /// <summary>
    /// Hashes the password and returns a string for storage: "iterations.saltBase64.hashBase64"
    /// </summary>
    public static string HashPassword(string password)
    {
        if (password is null) throw new ArgumentNullException(nameof(password));

        // Generate salt
        byte[] salt = new byte[SaltSize];
        RandomNumberGenerator.Fill(salt);

        // Calculate PBKDF2-HMACSHA256
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(HashSize);

        // Serialization: iterations.salt.hash (Base64)
        string result = $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        return result;
    }

    /// <summary>
    /// Checks if the entered password matches the stored hash.
    /// Expected format for storedHash: "iterations.saltBase64.hashBase64"
    /// </summary>
    public static bool VerifyPassword(string password, string storedHash)
    {
        if (password is null) throw new ArgumentNullException(nameof(password));
        if (string.IsNullOrWhiteSpace(storedHash)) return false;

        var parts = storedHash.Split('.');
        if (parts.Length != 3) return false;

        if (!int.TryParse(parts[0], out int iterations)) return false;
        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] expectedHash = Convert.FromBase64String(parts[2]);

        // Calculate hash with the same parameters
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        byte[] actualHash = pbkdf2.GetBytes(expectedHash.Length);

        // Secure comparison (constant time)
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
