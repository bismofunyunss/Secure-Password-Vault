﻿using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace Secure_Password_Vault;

public static class Crypto
{
    private const int Iterations = 64;
    private const double MemorySize = 1024d * 1024d * 10d;
    public const int SaltSize = 512 / 8;
    public static readonly int ByteSize = 24;
    public static readonly int KeySize = 32;
    public static readonly int IvBit = 128;
    private const int TagLen = 16;
    private const int HmacTagLength = 64;
    private const int NonceSize = 12;

    private static readonly RandomNumberGenerator RndNum = RandomNumberGenerator.Create();


    public static byte[]? Hash { get; set; } = Array.Empty<byte>();

    // Not implemented
    private static string? CheckSum { get; set; } = string.Empty;

    /// <summary>
    ///     Hashes a password inside of a char array.
    /// </summary>
    /// <param name="passWord">The char array to hash.</param>
    /// <param name="salt">The salt used during the argon2id hashing process.</param>
    /// <returns>A password hash byte array.</returns>
    public static async Task<byte[]?> HashAsync(char[]? passWord, byte[]? salt)
    {
        if (passWord == null || salt == null)
            return null;

        try
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(passWord));
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = Environment.ProcessorCount * 2;
            argon2.Iterations = Iterations;
            argon2.MemorySize = (int)MemorySize;

            var result = await argon2.GetBytesAsync(ByteSize);
            return result;
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

        try
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(passWord));
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = Environment.ProcessorCount * 2;
            argon2.Iterations = Iterations;
            argon2.MemorySize = (int)MemorySize;

            // ReSharper disable once AccessToDisposedClosure
            var resultBytes = await Task.Run(() => argon2.GetBytesAsync(KeySize)).ConfigureAwait(false);

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
        return Task.FromResult(inputHash != null && Hash != null &&
                               CryptographicOperations.FixedTimeEquals(Hash, inputHash));
    }

    // Not implemented
    public static string ComputeChecksum(byte[] input)
    {
        var hashValue = SHA512.HashData(input);
        var checksum = DataConversionHelpers.ByteArrayToHexString(hashValue) ?? string.Empty;
        return checksum;
    }

    private static int RndInt()
    {
        var buffer = new byte[sizeof(int)];
        RndNum.GetBytes(buffer);
        var result = BitConverter.ToInt32(buffer, 0);
        return result;
    }


    private static int BoundedInt(int min, int max)
    {
        if (min >= max)
            throw new ArgumentException("Min must be less than max.");
        var value = RndInt();
        var range = max - min;
        var result = min + Math.Abs(value) % range;

        return result;
    }


    public static byte[] RndByteSized(int size)
    {
        var buffer = new byte[size];
        RndNum.GetBytes(buffer);
        return buffer;
    }

    public static async Task<byte[]> EncryptFile(string userName, char[] passWord, string file)
    {
        if (userName == null || passWord == null || file == null)
            throw new ArgumentNullException();

        var iv = RndByteSized(IvBit / 8);

        var saltString = await File.ReadAllTextAsync(Authentication.GetUserSalt(userName));

        var salt = DataConversionHelpers.Base64StringToByteArray(saltString);

        var fileStr = await File.ReadAllTextAsync(file);
        var fileBytes = DataConversionHelpers.StringToByteArray(fileStr);

        var passwordBytes = Encoding.UTF8.GetBytes(passWord);

        if (fileBytes == null || salt == null)
            return Array.Empty<byte>();

        var encryptedFile = await EncryptAsync(fileBytes, passwordBytes, iv, salt);

        return encryptedFile;
    }

    public static async Task<byte[]> DecryptFile(string userName, char[] passWord, string file)
    {
        if (userName == null || passWord == null || file == null)
            throw new ArgumentNullException();

        var saltString = await File.ReadAllTextAsync(Authentication.GetUserSalt(userName));

        var salt = DataConversionHelpers.Base64StringToByteArray(saltString);

        var fileStr = await File.ReadAllTextAsync(file);
        var fileBytes = DataConversionHelpers.Base64StringToByteArray(fileStr);

        var passwordBytes = Encoding.UTF8.GetBytes(passWord);
        if (fileBytes == null || salt == null)
            return Array.Empty<byte>();

        var encryptedFile = await DecryptAsync(fileBytes, passwordBytes, salt);

        return encryptedFile;
    }

    private static (byte[] cipherResult, byte[] iv) InitBuffer(byte[] cipherText)
    {
        var iv = new byte[IvBit / 8];
        var cipherResult = new byte[cipherText.Length - iv.Length];

        Buffer.BlockCopy(cipherText, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(cipherText, iv.Length, cipherResult, 0, cipherResult.Length);

        return (cipherResult, iv);
    }

#pragma warning disable
    private const int BlockBitSize = 128;
    private const int KeyBitSize = 256;

    public static async Task<byte[]> EncryptAsync(byte[]? plainText, byte[]? password, byte[]? iv, byte[]? salt)
    {
        try
        {
            if (plainText == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty or null.", nameof(plainText));
            if (password == null)
                throw new ArgumentException(@"Value was empty or null.", nameof(password));
            if (salt == null)
                throw new ArgumentException(@"Value was empty or null.", nameof(salt));
            if (iv == null)
                throw new ArgumentException(@"Value was empty or null.", nameof(iv));

            using var aes = Aes.Create();
            aes.BlockSize = BlockBitSize;
            aes.KeySize = KeyBitSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var argon2 = new Argon2id(password);
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = Environment.ProcessorCount * 2;
            argon2.Iterations = Iterations;
            argon2.MemorySize = (int)MemorySize;

            var key = await argon2.GetBytesAsync(KeySize);

            var hmacKey = await argon2.GetBytesAsync(HmacTagLength);

            byte[] cipherText;
            using (var encryptor = aes.CreateEncryptor(key, iv))
            using (var memStream = new MemoryStream())
            {
                await using (var cryptoStream =
                             new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                {
                    using (var cipherStream = new MemoryStream(plainText))
                    {
                        cipherStream.CopyTo(cryptoStream, (int)cipherStream.Length);
                        cipherStream.Flush();
                        cryptoStream.FlushFinalBlockAsync();
                    }
                }

                cipherText = memStream.ToArray();
            }

            Array.Clear(key, 0, key.Length);

            using var hmac = new HMACSHA512(hmacKey);
            var prependItems = new byte[cipherText.Length + iv.Length];

            Buffer.BlockCopy(iv, 0, prependItems, 0, iv.Length);
            Buffer.BlockCopy(cipherText, 0, prependItems, iv.Length, cipherText.Length);

            var tag = hmac.ComputeHash(prependItems);
            var authenticatedBuffer = prependItems.Length + tag.Length;
            var authenticatedBytes = new byte[authenticatedBuffer];

            Buffer.BlockCopy(prependItems, 0, authenticatedBytes, 0, prependItems.Length);
            Buffer.BlockCopy(tag, 0, authenticatedBytes, prependItems.Length, tag.Length);
            Array.Clear(hmacKey, 0, hmacKey.Length);

            return authenticatedBytes;
        }
        catch (CryptographicException ex)
        {
            Array.Clear(password, 0, password.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (ArgumentNullException ex)
        {
            Array.Clear(password, 0, password.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (ObjectDisposedException ex)
        {
            Array.Clear(password, 0, password.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            Array.Clear(password, 0, password.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
    }

    public static async Task<byte[]> DecryptAsync(byte[]? cipherText, byte[]? password, byte[]? salt)
    {
        try
        {
            if (cipherText == null)
                throw new ArgumentException(@"Value was empty or null.", nameof(cipherText));
            if (password == null)
                throw new ArgumentException(@"Value was empty or null.", nameof(password));
            if (salt == null)
                throw new ArgumentException(@"Value was empty or null.", nameof(salt));

            using var aes = Aes.Create();
            aes.BlockSize = BlockBitSize;
            aes.KeySize = KeyBitSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var argon2 = new Argon2id(password);
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = Environment.ProcessorCount * 2;
            argon2.Iterations = Iterations;
            argon2.MemorySize = (int)MemorySize;

            var key = await argon2.GetBytesAsync(KeySize);

            var hmacKey = await argon2.GetBytesAsync(HmacTagLength);

            using var hmac = new HMACSHA512(hmacKey);
            var receivedHash = new byte[HmacTagLength];

            Buffer.BlockCopy(cipherText, cipherText.Length - HmacTagLength, receivedHash, 0, HmacTagLength);

            var cipherWithIv = new byte[cipherText.Length - HmacTagLength];

            Buffer.BlockCopy(cipherText, 0, cipherWithIv, 0, cipherText.Length - HmacTagLength);

            var hashedInput = hmac.ComputeHash(cipherWithIv);

            var isMatch = CryptographicOperations.FixedTimeEquals(receivedHash, hashedInput);

            if (!isMatch)
                throw new CryptographicException("Invalid tag.");

            var (cipherResult, iv) = InitBuffer(cipherWithIv);

            using var decryptor = aes.CreateDecryptor(key, iv);
            using var memStream = new MemoryStream();
            await using (var decryptStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Write))
            {
                using (var plainStream = new MemoryStream(cipherResult))
                {
                    plainStream.CopyTo(decryptStream, (int)plainStream.Length);
                    plainStream.Flush();
                    await decryptStream.FlushFinalBlockAsync();
                }
            }

            Array.Clear(key, 0, key.Length);

            return memStream.ToArray();
        }
        catch (CryptographicException ex)
        {
            Array.Clear(password, 0, password.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (ArgumentNullException ex)
        {
            Array.Clear(password, 0, password.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (ObjectDisposedException ex)
        {
            Array.Clear(password, 0, password.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            Array.Clear(password, 0, password.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
#pragma warning restore
    }

    public static async Task<byte[]> EncryptAsyncV2(byte[] plainText, byte[] password, byte[] nonce, byte[] salt)
    {
        try
        {
            if (plainText == null)
                throw new ArgumentException(@"Value was empty or null.", nameof(plainText));
            if (password == null)
                throw new ArgumentException(@"Value was empty or null.", nameof(password));
            if (salt == null)
                throw new ArgumentException(@"Value was empty or null.", nameof(salt));
            if (nonce == null)
                throw new ArgumentException(@"Value was empty or null.", nameof(nonce));

            var cipherText = new byte[plainText.Length];
            var tag = new byte[TagLen];

            using (var argon2 = new Argon2id(password))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = Environment.ProcessorCount * 2;
                argon2.Iterations = Iterations;
                argon2.MemorySize = (int)MemorySize;

                var key = await argon2.GetBytesAsync(KeySize);

                using var aesGcm = new AesGcm(key, TagLen);
                aesGcm.Encrypt(nonce, plainText, cipherText, tag);
                Array.Clear(key, 0, key.Length);
            }

            cipherText = tag.Concat(nonce.Concat(cipherText)).ToArray();
            return cipherText;
        }
        catch (CryptographicException ex)
        {
            Array.Clear(password, 0, password.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (ArgumentNullException ex)
        {
            Array.Clear(password, 0, password.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            Array.Clear(password!, 0, password!.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
    }

    public static async Task<byte[]> DecryptAsyncV2(byte[] cipherText, byte[] password, byte[] salt)
    {
        try
        {
            if (cipherText == null)
                throw new ArgumentException(@"Value was empty or null.", nameof(cipherText));
            if (password == null)
                throw new ArgumentException(@"Value was empty or null.", nameof(password));
            if (salt == null)
                throw new ArgumentException(@"Value was empty or null.", nameof(salt));

            using var argon2 = new Argon2id(password);
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = Environment.ProcessorCount * 2;
            argon2.Iterations = Iterations;
            argon2.MemorySize = (int)MemorySize;

            var key = await argon2.GetBytesAsync(KeySize);

            using var aesGcm = new AesGcm(key, TagLen);
            var tag = new byte[TagLen];
            var nonce = new byte[NonceSize];
            var cipherResult = new byte[cipherText.Length - nonce.Length - tag.Length];

            Buffer.BlockCopy(cipherText, 0, tag, 0, tag.Length);
            Buffer.BlockCopy(cipherText, tag.Length, nonce, 0, nonce.Length);
            Buffer.BlockCopy(cipherText, tag.Length + nonce.Length, cipherResult, 0, cipherResult.Length);

            var plainText = new byte[cipherResult.Length];

            aesGcm.Decrypt(nonce, cipherResult, tag, plainText);

            Array.Clear(key, 0, key.Length);

            return plainText;
        }
        catch (CryptographicException ex)
        {
            Array.Clear(password, 0, password.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (ArgumentNullException ex)
        {
            Array.Clear(password, 0, password.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            Array.Clear(password!, 0, password!.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
#pragma warning restore
    }
}