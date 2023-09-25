using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace Secure_Password_Vault;

public static class Crypto
{
    private const int Iterations = 64;
    private const double MemorySize = 1024d * 1024d * 10d;
    public const int SaltSize = 384 / 8;
    public static readonly int ByteSize = 24;
    public static readonly int KeySize = 16;
    public static readonly int IvBit = 128;


    private static readonly RandomNumberGenerator RndNum = RandomNumberGenerator.Create();
    public static byte[] Salt { get; set; } = { };
    public static byte[] Iv { get; set; } = { };
    public static byte[]? Hash { get; set; } = { };
    // Not implemented
    private static string? CheckSum { get; set; } = string.Empty;

    public static async Task<byte[]?> HashAsync(char[]? passWord, byte[]? salt)
    {
        if (passWord == null || salt == null)
            return null;

        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(passWord));
        argon2.Salt = salt;
        argon2.DegreeOfParallelism = Environment.ProcessorCount * 2;
        argon2.Iterations = Iterations;
        argon2.MemorySize = (int)MemorySize;
        try
        {
            var result = await Task.Run(() => argon2.GetBytesAsync(ByteSize).ConfigureAwait(false));
            return await result;
        }
        catch (CryptographicException ex)
        {
            ErrorLogging.ErrorLog(ex);
            return null;
        }
    }

    public static async Task<char[]?> DeriveAsync(char[]? passWord, byte[]? salt)
    {
        if (passWord == null || salt == null)
            return null;

        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(passWord));
        argon2.Salt = salt;
        argon2.DegreeOfParallelism = Environment.ProcessorCount * 2;
        argon2.Iterations = Iterations;
        argon2.MemorySize = (int)MemorySize;

        try
        {
            // ReSharper disable once AccessToDisposedClosure
            var resultBytes = await Task.Run(() => argon2.GetBytesAsync(KeySize))
                .ConfigureAwait(false);

            var result = Convert.ToBase64String(resultBytes).ToCharArray();
            return result;
        }

        catch (CryptographicException ex)
        {
            MessageBox.Show(ex.Message);
            ErrorLogging.ErrorLog(ex);
            return null;
        }
    }


    public static Task<bool> ComparePassword(byte[]? inputHash)
    {
        return Task.FromResult(inputHash != null && Hash is not null &&
                               CryptographicOperations.FixedTimeEquals(Hash, inputHash));
    }

    // Not implemented
    public static string ComputeChecksum(byte[] input)
    {
        var hashValue = SHA512.HashData(input);
        var checksum = DataConversionHelpers.ByteArrayToHexString(hashValue) ?? string.Empty;
        return checksum;
    }

    public static async Task<byte[]?> EncryptUserFiles(string userName, char[] passWord, string? file)
    {
        if (file == null)
            return null;

        var saltBuffer = new byte[SaltSize];
        var ivBuffer = new byte[IvBit / 8];

        Iv = RndByteSized(IvBit / 8);

        await File.WriteAllTextAsync(Authentication.GetUserInfoPath(userName),
            DataConversionHelpers.ByteArrayToBase64String(Salt) + DataConversionHelpers.ByteArrayToBase64String(Iv));
        var userInfo = await File.ReadAllTextAsync(Authentication.GetUserInfoPath(userName));


        var infoBytes = DataConversionHelpers.Base64StringToByteArray(userInfo);
        if (infoBytes == null)
            return null;
        Buffer.BlockCopy(infoBytes, 0, saltBuffer, 0, saltBuffer.Length);
        Buffer.BlockCopy(infoBytes, saltBuffer.Length, ivBuffer, 0, ivBuffer.Length);
        Salt = saltBuffer;
        Iv = ivBuffer;

        var textString = await File.ReadAllTextAsync(file);
        var textBytes = DataConversionHelpers.StringToByteArray(textString);
        if (passWord == null)
            throw new ArgumentException(@"Value was empty or null.", nameof(passWord));
        var derivedKey = await DeriveAsync(passWord, Salt);
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
        if (derivedKey == null)
            throw new ArgumentException(@"Value returned null or empty.", nameof(derivedKey));
        var keyBytes = Encoding.UTF8.GetBytes(derivedKey);

        var encryptedBytes = Encrypt(textBytes, keyBytes);
        Array.Clear(keyBytes, 0, keyBytes.Length);
        Array.Clear(derivedKey, 0, derivedKey.Length);
        if (encryptedBytes == null)
            throw new ArgumentException(@"Value returned empty or null.", nameof(encryptedBytes));
        return encryptedBytes;
    }

    public static async Task<byte[]?> DecryptUserFiles(string userName, char[] passWord, string? file)
    {
        if (file == null)
            return null;

        var saltBuffer = new byte[SaltSize];
        var ivBuffer = new byte[IvBit / 8];

        var userInfo = await File.ReadAllTextAsync(Authentication.GetUserInfoPath(userName));

        var infoBytes = DataConversionHelpers.Base64StringToByteArray(userInfo);
        if (infoBytes == null)
            return null;
        Buffer.BlockCopy(infoBytes, 0, saltBuffer, 0, saltBuffer.Length);
        Buffer.BlockCopy(infoBytes, saltBuffer.Length, ivBuffer, 0, ivBuffer.Length);
        Salt = saltBuffer;
        Iv = ivBuffer;

        var textString = await File.ReadAllTextAsync(file);
        var textBytes = DataConversionHelpers.Base64StringToByteArray(textString);
        var derivedKey = await DeriveAsync(passWord, Salt);
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
        if (derivedKey == null)
            throw new ArgumentException(@"Value returned null or empty.", nameof(derivedKey));
        var keyBytes = Encoding.UTF8.GetBytes(derivedKey);
        var decryptedBytes = Decrypt(textBytes, keyBytes);
        if (decryptedBytes == null)
            throw new ArgumentException(
                @"Login failed due to decryption error. Please recheck your credentials and try again.",
                nameof(decryptedBytes));
        return decryptedBytes;
    }

    private static int RndInt()
    {
        var buffer = new byte[sizeof(int)];
        RndNum.GetBytes(buffer);
        var result = BitConverter.ToInt32(buffer, 0);
        return result;
    }

/*
    private static int BoundedInt(int min, int max)
    {
        if (min >= max)
            throw new ArgumentException("Min must be less than max.");
        var value = RndInt();
        var range = max - min;
        var result = min + Math.Abs(value) % range;

        return result;
    }
*/

    public static byte[] RndByteSized(int size)
    {
        var buffer = new byte[size];
        RndNum.GetBytes(buffer);
        return buffer;
    }
#pragma warning disable
    private const int BlockBitSize = 128;
    private const int KeyBitSize = 256;

    public static byte[]? Encrypt(byte[]? plainText, byte[]? key)
    {
        try
        {
            if (plainText == null)
                throw new ArgumentException(@"Value was empty or null.", nameof(plainText));
            byte[] cipherText;
            using (var aes = Aes.Create())
            {
                aes.BlockSize = BlockBitSize;
                aes.KeySize = KeyBitSize;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor(key, Iv))
                using (var memStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (var cipherStream = new MemoryStream(plainText))
                        {
                            cipherStream.CopyTo(cryptoStream, (int)cipherStream.Length);
                            cipherStream.Flush();
                            cryptoStream.FlushFinalBlock();
                        }
                    }

                    cipherText = memStream.ToArray();
                }
            }

            Array.Clear(key, 0, key.Length);
            var prependItems = new byte[cipherText.Length + Iv.Length];
            Buffer.BlockCopy(Iv, 0, prependItems, 0, Iv.Length);
            Buffer.BlockCopy(cipherText, 0, prependItems, Iv.Length, cipherText.Length);
            return prependItems;
        }
        catch (CryptographicException ex)
        {
            Array.Clear(key, 0, key.Length);
            ErrorLogging.ErrorLog(ex);
            return null;
        }

        catch (Exception ex)
        {
            Array.Clear(key, 0, key.Length);
            ErrorLogging.ErrorLog(ex);
            return null;
        }
    }

    public static byte[] Decrypt(byte[]? cipherText, byte[]? key)
    {
        try
        {
            if (cipherText == null) throw new ArgumentException(@"Value was empty or null.", nameof(cipherText));

            using var aes = Aes.Create();
            aes.BlockSize = BlockBitSize;
            aes.KeySize = KeyBitSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;


            Buffer.BlockCopy(cipherText, 0, Iv, 0, Iv.Length);
            var cipherResult = new byte[cipherText.Length - Iv.Length];
            Buffer.BlockCopy(cipherText, Iv.Length, cipherResult, 0, cipherResult.Length);

            using var decryptor = aes.CreateDecryptor(key, Iv);
            using var memStrm = new MemoryStream();
            using (var decryptStream = new CryptoStream(memStrm, decryptor, CryptoStreamMode.Write))
            {
                using (var plainStream = new MemoryStream(cipherResult))
                {
                    plainStream.CopyTo(decryptStream, (int)plainStream.Length);
                    plainStream.Flush();
                    decryptStream.FlushFinalBlock();
                }
            }

            Array.Clear(key, 0, key.Length);
            return memStrm.ToArray();
        }
        catch (CryptographicException ex)
        {
            Array.Clear(key, 0, key.Length);
            ErrorLogging.ErrorLog(ex);
            return null;
        }
        catch (Exception ex)
        {
            Array.Clear(key, 0, key.Length);
            ErrorLogging.ErrorLog(ex);
            return null;
        }
#pragma warning restore
    }
}