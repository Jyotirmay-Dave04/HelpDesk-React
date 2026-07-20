using System;
using System.Security.Cryptography;
using System.Text;

public static class AesGcmHelper
{
    private const int NonceSize = 12;
    private const int TagSize = 16;

    public static string Encrypt(string plaintext, byte[] key)
    {
        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var ciphertext = new byte[plaintextBytes.Length];
        var tag = new byte[TagSize];

        using var aesGcm = new AesGcm(key, TagSize);
        aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);

        var result = new byte[NonceSize + ciphertext.Length + TagSize];
        Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
        Buffer.BlockCopy(ciphertext, 0, result, NonceSize, ciphertext.Length);
        Buffer.BlockCopy(tag, 0, result, NonceSize + ciphertext.Length, TagSize);

        return Convert.ToBase64String(result);
    }

    public static string Decrypt(string encryptedBase64, byte[] key)
    {
        var fullBytes = Convert.FromBase64String(encryptedBase64);

        var nonce = fullBytes[..NonceSize];
        var tag = fullBytes[^TagSize..];
        var ciphertext = fullBytes[NonceSize..^TagSize];

        var plaintextBytes = new byte[ciphertext.Length];
        using var aesGcm = new AesGcm(key, TagSize);
        aesGcm.Decrypt(nonce, ciphertext, tag, plaintextBytes);

        return Encoding.UTF8.GetString(plaintextBytes);
    }
}