using System.Security.Cryptography;
using System.Text;
using EmailSaas.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EmailSaas.Infrastructure.Services;

public class AesEncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public AesEncryptionService(IConfiguration configuration)
    {
        var key = configuration["EncryptionSettings:Key"]
            ?? throw new InvalidOperationException("EncryptionSettings:Key is missing.");
        var iv = configuration["EncryptionSettings:IV"]
            ?? throw new InvalidOperationException("EncryptionSettings:IV is missing.");

        // Key must be 32 bytes for AES-256
        _key = Encoding.UTF8.GetBytes(key.PadRight(32)[..32]);
        // IV must be 16 bytes for AES
        _iv = Encoding.UTF8.GetBytes(iv.PadRight(16)[..16]);
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return plainText;

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var encryptedBytes = encryptor.TransformFinalBlock(
            plainBytes, 0, plainBytes.Length);

        return Convert.ToBase64String(encryptedBytes);
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) return cipherText;

        try
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var cipherBytes = Convert.FromBase64String(cipherText);
            var decryptedBytes = decryptor.TransformFinalBlock(
                cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch
        {
            // If decryption fails return as is
            // handles plain text values already in DB
            return cipherText;
        }
    }
}