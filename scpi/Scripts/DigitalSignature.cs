using System.Security.Cryptography;

namespace scpi;

public static class DigitalSignature
{
    public static string Sign(string message, byte[] privateKey, byte[] publicKey)
    {
        // use OpenSSL to sign the message
        using (var rsa = new RSAOpenSsl())
        {
            rsa.ImportRSAPrivateKey(privateKey, out _);

            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            byte[] signatureBytes = rsa.SignData(messageBytes, HashAlgorithmName.SHA3_256, RSASignaturePadding.Pss);
            return Convert.ToBase64String(signatureBytes);
        }
    }

    public static bool Verify(string message, string signature, byte[] publicKey)
    {
        // use OpenSSL to verify the signature
        using (var rsa = new RSAOpenSsl())
        {
            rsa.ImportSubjectPublicKeyInfo(publicKey, out _);

            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            byte[] signatureBytes = Convert.FromBase64String(signature);
            return rsa.VerifyData(messageBytes, signatureBytes, HashAlgorithmName.SHA3_256, RSASignaturePadding.Pss);
        }
    }
}