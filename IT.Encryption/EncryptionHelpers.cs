using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace IT.Encryption;

public class EncryptionHelpers
{
    private const int NonceBytes = 10;
    private const int KeyBytes = 16;
    
    public static byte[] Encrypt(byte[] toEncrypt, byte[] key, byte[] tag, byte[]? associatedData = null)
    {
        var nonce = new byte[NonceBytes];
        var cipherText = new byte[toEncrypt.Length];

        var cipher1 = Aes.Create();

        using var cipher = new AesGcm(key);
        cipher.Encrypt(nonce, toEncrypt, cipherText, tag, associatedData);

        return Concat(tag, Concat(nonce, cipherText));
    }
    
    public static string Encrypt(string plain, byte[] key)
    {
        // Get bytes of plaintext string
        var plainBytes = Encoding.UTF8.GetBytes(plain);
    
        // Get parameter sizes
        var nonceSize = AesGcm.NonceByteSizes.MaxSize;
        var tagSize = AesGcm.TagByteSizes.MaxSize;
        var cipherSize = plainBytes.Length;
    
        // We write everything into one big array for easier encoding
        var encryptedDataLength = 4 + nonceSize + 4 + tagSize + cipherSize;
        var encryptedData = encryptedDataLength < 1024
            ? stackalloc byte[encryptedDataLength]
            : new byte[encryptedDataLength].AsSpan();
    
        // Copy parameters
        BinaryPrimitives.WriteInt32LittleEndian(encryptedData[..4], nonceSize);
        BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4), tagSize);
        var nonce = encryptedData.Slice(4, nonceSize);
        var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
        var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);
    
        // Generate secure nonce
        RandomNumberGenerator.Fill(nonce);
    
        // Encrypt
        using var aes = new AesGcm(key);
        aes.Encrypt(nonce, plainBytes.AsSpan(), cipherBytes, tag);
    
        // Encode for transmission
        return Convert.ToBase64String(encryptedData);
    }
    
    public static string Decrypt(string cipher, byte[] key)
    {
        // Decode
        var encryptedData = Convert.FromBase64String(cipher).AsSpan();
        
        // Extract parameter sizes
        var nonceSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData[..4]);
        var tagSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4));
        var cipherSize = encryptedData.Length - 4 - nonceSize - 4 - tagSize;
    
        // Extract parameters
        var nonce = encryptedData.Slice(4, nonceSize);
        var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
        var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);
    
        // Decrypt
        Span<byte> plainBytes = cipherSize < 1024
            ? stackalloc byte[cipherSize]
            : new byte[cipherSize];
        using var aes = new AesGcm(key);
        aes.Decrypt(nonce, cipherBytes, tag, plainBytes);
    
        // Convert plain bytes back into string
        return Encoding.UTF8.GetString(plainBytes);
    }
    
    public static byte[] Decrypt(byte[] cipherText, byte[] key, byte[]? associatedData = null)
    {
        var tag = SubArray(cipherText, 0, KeyBytes);
        var nonce = SubArray(cipherText, KeyBytes, NonceBytes);

        var toDecrypt = SubArray(cipherText, KeyBytes + NonceBytes, cipherText.Length - tag.Length - nonce.Length);
        var decryptedData = new byte[toDecrypt.Length];

        using var cipher = new AesGcm(key);
        cipher.Decrypt(nonce, toDecrypt, tag, decryptedData, associatedData);

        return decryptedData;
    }

    private static byte[] Concat(byte[] a, byte[] b)
    {
        var output = new byte[a.Length + b.Length];

        for (var i = 0; i < a.Length; i++)
        {
            output[i] = a[i];
        }

        for (var j = 0; j < b.Length; j ++)
        {
            output[a.Length + j] = b[j];
        }

        return output;
    }

    private static byte[] SubArray(byte[] data, int start, int length)
    {
        var result = new byte[length];

        Array.Copy(data, start, result, 0, length);

        return result;
    }

}