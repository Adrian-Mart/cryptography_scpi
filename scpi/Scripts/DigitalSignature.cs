using System.Security.Cryptography;

namespace scpi;

/// <summary>
/// This class is used to sign and verify digital signatures
/// </summary>
public static class DigitalSignature
{
    /// <summary>
    /// Signs a message using the private key
    /// </summary>
    /// <param name="message">The message to sign</param>
    /// <param name="privateKey">The private key to use for signing</param>
    /// <returns>The signature</returns>
    public static string Sign(string message, byte[] privateKey)
    {
        // use RSA.Create() to create an instance of the RSA algorithm
        using (var rsa = RSA.Create())
        {
            // import the private key into the RSA object
            rsa.ImportRSAPrivateKey(privateKey, out _);

            // convert the message to bytes
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);

            // sign the message using SHA3-256 and PSS padding and convert the signature to a base64 string
            byte[] signatureBytes = rsa.SignData(messageBytes, HashAlgorithmName.SHA3_256, RSASignaturePadding.Pss);

            // return the signature
            return Convert.ToBase64String(signatureBytes);
        }
    }

    /// <summary>
    /// Verifies a signature using the public key
    /// </summary>
    /// <param name="message">The message to verify</param>
    /// <param name="signature">The signature to verify</param>
    /// <param name="publicKey">The public key to use for verification</param>
    /// <returns>True if the signature is valid, false otherwise</returns>
    public static bool Verify(string message, string signature, byte[] publicKey)
    {
        // use OpenSSL to verify the signature
        using (var rsa = RSA.Create())
        {
            // import the public key into the RSA object
            rsa.ImportSubjectPublicKeyInfo(publicKey, out _);

            // convert the message and signature to bytes
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            byte[] signatureBytes = Convert.FromBase64String(signature);

            // verify the signature using SHA3-256 and PSS padding and return the result
            return rsa.VerifyData(messageBytes, signatureBytes, HashAlgorithmName.SHA3_256, RSASignaturePadding.Pss);
        }
    }
}