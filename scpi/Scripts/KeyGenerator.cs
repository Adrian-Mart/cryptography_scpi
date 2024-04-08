using System.Security.Cryptography;

namespace scpi;

public class KeyGenerator
{
    public static byte[] GenerateSymmetricKey(string password, out byte[] salt)
    {
        salt = new byte[9];
        RandomNumberGenerator.Fill(salt);

        using (var deriveBytes = new Rfc2898DeriveBytes(
            password,
            salt,
            10000,
            HashAlgorithmName.SHA3_256))
        {
            return deriveBytes.GetBytes(32);
        }
    }

    public static byte[] GenerateSymmetricKey(string password, byte[] salt)
    {
        using (var deriveBytes = new Rfc2898DeriveBytes(
            password,
            salt,
            10000,
            HashAlgorithmName.SHA3_256))
        {
            return deriveBytes.GetBytes(32);
        }
    }

    /// <summary>
    /// Generates a symmetric key from a password.
    /// </summary>
    /// <param name="password">The password to generate the key
    /// from.</param>
    /// <returns>The generated 256-bit key.</returns>
    public static byte[] GenerateSymmetricKey(string password)
    {
        // Salt for the key derivation function
        byte[] salt = new byte[9]; // You should use a unique salt for each password
        RandomNumberGenerator.Fill(salt);

        // Derive a 256-bit key (AES-256) from the password using PBKDF2
        using (var deriveBytes = new Rfc2898DeriveBytes(
            password,
            // Use the salt as additional entropy
            salt,
            // Use 10000 iterations for the key derivation function
            10000,
            // Use SHA-3-256 as the pseudo-random function
            HashAlgorithmName.SHA3_256))
        {
            return deriveBytes.GetBytes(32); // 32 bytes = 256 bits
        }
    }

    /// <summary>
    /// Generates a pair of asymmetric keys.
    /// </summary>
    /// <returns>The generated keys.</returns>
    public static (byte[], byte[]) GenerateAsymmetricKeys()
    {
        using (var rsa = new RSACryptoServiceProvider())
        {
            // Export the public key
            var publicKey = rsa.ExportSubjectPublicKeyInfo();

            // Export the private key
            var privateKey = rsa.ExportRSAPrivateKey();

            // Return the keys
            return (publicKey, privateKey);
        }
    }
}