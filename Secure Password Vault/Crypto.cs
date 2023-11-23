using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Sodium;

namespace Secure_Password_Vault;

public static class Crypto
{
    private const int Iterations = 1; // 32 iterations in release build
    private const double MemorySize = 1024d * 1024d * 1d; // 5gib in release build
    public const int SaltSize = 512 / 8;
    public static readonly int KeySize = 32;
    public static readonly int IvBit = 128;
    private const int TagLen = 16;
    private const int HmacLength = 64;
    private const int GcmNonceSize = 12;
    private const int ChaChaNonceSize = 24;

    private static readonly RandomNumberGenerator RndNum = RandomNumberGenerator.Create();

    public static byte[]? Hash { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Hashes a password inside of a char array or derives a key from a password.
    /// </summary>
    /// <param name="passWord">The char array to hash.</param>
    /// <param name="salt">The salt used during the argon2id hashing process.</param>
    /// <param name="outputSize">The output size in bytes.</param>
    /// <returns>A password hash byte array.</returns>
    public static async Task<byte[]> Argon2Id(char[] passWord, byte[] salt, int outputSize)
    {
        try
        {
            if (passWord == Array.Empty<char>() || salt == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty.",
                    passWord == Array.Empty<char>() ? nameof(passWord) : nameof(salt));

            using var argon2 =
                new Argon2id(Encoding.UTF8.GetBytes(passWord ??
                                                    throw new ArgumentException(@"Value was empty.",
                                                        nameof(passWord))));
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = Environment.ProcessorCount * 2;
            argon2.Iterations = Iterations;
            argon2.MemorySize = (int)MemorySize;

            var result = await argon2.GetBytesAsync(outputSize);
            return result;
        }
        catch (CryptographicException ex)
        {
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (ArgumentException ex)
        {
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
    }

    public static Task<bool> ComparePassword(byte[]? hash1, byte[]? hash2)
    {
        if (hash1 == null || hash1 == Array.Empty<byte>() || hash2 == null || hash2 == Array.Empty<byte>())
            throw new ArgumentException(@"Value was empty.", hash1 == null ? nameof(hash1) : nameof(hash2));

        return Task.FromResult(CryptographicOperations.FixedTimeEquals(hash1, hash2));
    }

    private static int RndInt()
    {
        var buffer = new byte[sizeof(int)];
        RndNum.GetBytes(buffer);
        var result = BitConverter.ToInt32(buffer, 0);
        return result;
    }


    public static int BoundedInt(int min, int max)
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
        try
        {
            if (userName == string.Empty || passWord == Array.Empty<char>() || file == string.Empty)
                throw new ArgumentException(@"Value was empty.", userName == string.Empty ? nameof(userName) :
                    passWord == Array.Empty<char>() ? nameof(passWord) : nameof(file));

            var saltResult = await Authentication.GetUserSaltAsync(userName);

            var salt = saltResult.Salt;
            var salt2 = saltResult.Salt2;
            var salt3 = saltResult.Salt3;

            var bytes = await Argon2Id(passWord, salt2, 128);

            var key = new byte[KeyBitSize];
            var key2 = new byte[KeyBitSize];
            var hMacKey = new byte[HmacLength];

            Buffer.BlockCopy(bytes, 0, key, 0, key.Length);
            Buffer.BlockCopy(bytes, key.Length, key2, 0, key2.Length);
            Buffer.BlockCopy(bytes, key.Length + key2.Length, hMacKey, 0, hMacKey.Length);

            var fileStr = await File.ReadAllTextAsync(file);
            var fileBytes = DataConversionHelpers.StringToByteArray(fileStr);

            if (fileBytes == Array.Empty<byte>() || salt == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty.",
                    fileBytes == Array.Empty<byte>() ? nameof(fileBytes) : nameof(salt));

            var encryptedFile = await EncryptAsyncV3(fileBytes, key, key2, hMacKey);

            Array.Clear(passWord, 0, passWord.Length);

            return encryptedFile;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
    }

    public static async Task<byte[]> DecryptFile(string userName, char[] passWord, string file)
    {
        try
        {
            if (userName == string.Empty || passWord == Array.Empty<char>() || file == string.Empty)
                throw new ArgumentException(@"Value was empty.", userName == string.Empty ? nameof(userName) :
                    passWord == Array.Empty<char>() ? nameof(passWord) : nameof(file));

            var saltResult = await Authentication.GetUserSaltAsync(userName);

            var salt = saltResult.Salt;
            var salt2 = saltResult.Salt2;
            var salt3 = saltResult.Salt3;

            var bytes = await Argon2Id(passWord, salt2, 128);

            var key = new byte[KeyBitSize];
            var key2 = new byte[KeyBitSize];
            var hMacKey = new byte[HmacLength];

            Buffer.BlockCopy(bytes, 0, key, 0, key.Length);
            Buffer.BlockCopy(bytes, key.Length, key2, 0, key2.Length);
            Buffer.BlockCopy(bytes, key.Length + key2.Length, hMacKey, 0, hMacKey.Length);

            var fileStr = await File.ReadAllTextAsync(file);
            var fileBytes = DataConversionHelpers.Base64StringToByteArray(fileStr);

            if (fileBytes == Array.Empty<byte>() || salt == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty.",
                    fileBytes == Array.Empty<byte>() ? nameof(fileBytes) : nameof(salt));

            var decryptedFile = await DecryptAsyncV3(fileBytes, key, key2, hMacKey);

            Array.Clear(passWord, 0, passWord.Length);

            return decryptedFile;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
    }

#pragma warning disable

    private const int BlockBitSize = 128;
    private const int KeyBitSize = 256;
    public static async Task<byte[]> EncryptAsync(byte[] plainText, byte[] key, byte[] iv, byte[] hMacKey)
    {
        // Parameter checks
        if (plainText == Array.Empty<byte>())
            throw new ArgumentException(@"Value was empty or null.", nameof(plainText));
        if (key == Array.Empty<byte>())
            throw new ArgumentException(@"Value was empty or null.", nameof(key));

        try
        {
            // Create AES object
            using var aes = Aes.Create();
            aes.BlockSize = BlockBitSize;
            aes.KeySize = KeyBitSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Create byte array to store cipherText into.
            byte[] cipherText;

            // Create streams and copy cipherStream to memStream
            using (var encryptor = aes.CreateEncryptor(key, iv))
            using (var memStream = new MemoryStream())
            {
                await using (var cryptoStream =
                             new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                {
                    using (var cipherStream = new MemoryStream(plainText))
                    {
                       await cipherStream.FlushAsync();
                       await cipherStream.CopyToAsync(cryptoStream, (int)cipherStream.Length);
                    }

                    await cryptoStream.FlushFinalBlockAsync();
                }

                cipherText = memStream.ToArray();
            }

            // Clear key parameter
            Array.Clear(key, 0, key.Length);

            // Create HMAC object
            using var hmac = new HMACBlake2B(hMacKey, HmacLength * 8);
            var prependItems = new byte[cipherText.Length + iv.Length];

            Buffer.BlockCopy(iv, 0, prependItems, 0, iv.Length);
            Buffer.BlockCopy(cipherText, 0, prependItems, iv.Length, cipherText.Length);

            // Hash the IV and cipherText and place them into authenticatedBytes byte array
            var tag = hmac.ComputeHash(prependItems);
            var authenticatedBuffer = prependItems.Length + tag.Length;
            var authenticatedBytes = new byte[authenticatedBuffer];

            Buffer.BlockCopy(prependItems, 0, authenticatedBytes, 0, prependItems.Length);
            Buffer.BlockCopy(tag, 0, authenticatedBytes, prependItems.Length, tag.Length);
            // Clear HMAC key
            Array.Clear(hMacKey, 0, hMacKey.Length);
            // Return authenticatedBytes byte array.
            return authenticatedBytes;
        }
        // Catch blocks
        catch (CryptographicException ex)
        {
            Array.Clear(key, 0, key.Length);
            Array.Clear(hMacKey, 0, hMacKey.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (ArgumentNullException ex)
        {
            Array.Clear(key, 0, key.Length);
            Array.Clear(hMacKey, 0, hMacKey.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (ObjectDisposedException ex)
        {
            Array.Clear(key, 0, key.Length);
            Array.Clear(hMacKey, 0, hMacKey.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            Array.Clear(key, 0, key.Length);
            Array.Clear(hMacKey, 0, hMacKey.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
    }

    public static async Task<byte[]> DecryptAsync(byte[] cipherText, byte[] key, byte[] hMacKey)
    {
        // Parameter checks
        if (cipherText == Array.Empty<byte>())
            throw new ArgumentException(@"Value was empty or null.", nameof(cipherText));
        if (key == Array.Empty<byte>())
            throw new ArgumentException(@"Value was empty or null.", nameof(key));

        try
        {
            // Create AES object
            using var aes = Aes.Create();
            aes.BlockSize = BlockBitSize;
            aes.KeySize = KeyBitSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Create HMAC object
            using var hmac = new HMACBlake2B(hMacKey, HmacLength * 8);
            var receivedHash = new byte[HmacLength];

            // Place the received hash into receivedHash byte array
            Buffer.BlockCopy(cipherText, cipherText.Length - HmacLength, receivedHash, 0, HmacLength);

            // Create byte array for IV and cipherText
            var cipherWithIv = new byte[cipherText.Length - HmacLength];

            // Place cipherText and IV into cipherWithIv byte array
            Buffer.BlockCopy(cipherText, 0, cipherWithIv, 0, cipherText.Length - HmacLength);

            // Get the hash value of cipherText and IV
            var hashedInput = hmac.ComputeHash(cipherWithIv);

            // Compare hash byte arrays using fixed timing to prevent timing attacks
            var isMatch = CryptographicOperations.FixedTimeEquals(receivedHash, hashedInput);

            // If hash values do not match, throw an exception
            if (!isMatch)
                throw new CryptographicException();

            // Clear HMAC key array
            Array.Clear(hMacKey, 0, hMacKey.Length);

            // Get cipherResult and IV values
            var iv = new byte[IvBit / 8];
            var cipherResult = new byte[cipherText.Length - iv.Length - HmacLength];

            Buffer.BlockCopy(cipherText, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(cipherText, iv.Length, cipherResult, 0, cipherResult.Length);

            // Create streams, and copy the contents of plainStream to memStream
            using var decryptor = aes.CreateDecryptor(key, iv);
            using var memStream = new MemoryStream();
            await using (var decryptStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Write))
            {
                using (var plainStream = new MemoryStream(cipherResult))
                {
                    await plainStream.CopyToAsync(decryptStream, (int)plainStream.Length);
                    await plainStream.FlushAsync();
                }

                await decryptStream.FlushFinalBlockAsync();
            }

            // Clear key array
            Array.Clear(key, 0, key.Length);

            // Return memStream as byte array
            return memStream.ToArray();
        }

        // Catch blocks
        catch (CryptographicException ex)
        {
            Array.Clear(key, 0, key.Length);
            Array.Clear(hMacKey, 0, hMacKey.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (ArgumentNullException ex)
        {
            Array.Clear(key, 0, key.Length);
            Array.Clear(hMacKey, 0, hMacKey.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (ObjectDisposedException ex)
        {
            Array.Clear(key, 0, key.Length);
            Array.Clear(hMacKey, 0, hMacKey.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            Array.Clear(key, 0, key.Length);
            Array.Clear(hMacKey, 0, hMacKey.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
#pragma warning restore
    }

    public static async Task<byte[]> EncryptAsyncV3(byte[] plaintext, 
          byte[] key, byte[] key2, byte[] hMacKey)
    {
        try
        {
            if (plaintext == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty.",
                    plaintext == Array.Empty<byte>() ? nameof(plaintext) :
                    key == Array.Empty<byte>() ? nameof(key2) :
                    key2 == Array.Empty<byte>() ? nameof(key2) : 
                    nameof(hMacKey));

            var nonce = RndByteSized(ChaChaNonceSize);
            var nonce2 = RndByteSized(IvBit / 8);

            var cipherText = SecretAeadXChaCha20Poly1305.Encrypt(plaintext, nonce, key);
            var cipherTextL2 = await EncryptAsync(cipherText, key2, nonce2, hMacKey);

            Array.Clear(key, 0, key.Length);
            Array.Clear(key2, 0, key2.Length);
            Array.Clear(hMacKey, 0, hMacKey.Length);

            return nonce.Concat(nonce2).Concat(cipherTextL2).ToArray();
        }
        catch (CryptographicException ex)
        {
            Array.Clear(key, 0, key.Length);
            Array.Clear(key2, 0, key2.Length);
            Array.Clear(hMacKey, 0, hMacKey.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            Array.Clear(key, 0, key.Length);
            Array.Clear(key2, 0, key2.Length);
            Array.Clear(hMacKey, 0, hMacKey.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
    }

    public static async Task<byte[]> DecryptAsyncV3(byte[] cipherText,
        byte[] key, byte[] key2, byte[] hMacKey)
    {
        try
        {
            if (cipherText == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty.",
                    cipherText == Array.Empty<byte>() ? nameof(cipherText) :
                    key == Array.Empty<byte>() ? nameof(key2) :
                    key2 == Array.Empty<byte>() ? nameof(key2) :
                    nameof(hMacKey));

            var nonce = new byte[ChaChaNonceSize];
            Buffer.BlockCopy(cipherText, 0, nonce, 0, nonce.Length);

            var nonce2 = new byte[IvBit / 8];
            Buffer.BlockCopy(cipherText, nonce.Length, nonce2, 0, nonce2.Length);

            var cipherResult =
                new byte[cipherText.Length - nonce2.Length - nonce.Length];

            Buffer.BlockCopy(cipherText, nonce.Length + nonce2.Length, cipherResult, 0,
                cipherResult.Length);

            var resultL2 = await DecryptAsync(cipherResult, key2, hMacKey);
            var resultL0 = SecretAeadXChaCha20Poly1305.Decrypt(resultL2, nonce, key);


            Array.Clear(key, 0, key.Length);
            Array.Clear(key2, 0, key2.Length);
            Array.Clear(hMacKey, 0, hMacKey.Length);

            return resultL0;
        }
        catch (CryptographicException ex)
        {
            Array.Clear(key, 0, key.Length);
            Array.Clear(key2, 0, key2.Length);
            Array.Clear(hMacKey, 0, hMacKey.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            Array.Clear(key, 0, key.Length);
            Array.Clear(key2, 0, key2.Length);
            Array.Clear(hMacKey, 0, hMacKey.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
    }

    public static async Task<byte[]> EncryptAsyncV3Debug(byte[] plaintext,
      byte[] nonce, byte[] nonce2, byte[] key, byte[] key2, byte[] hMacKey)
    {
        // Debug method allows us to set our own nonce
        try
        {
            if (plaintext == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty.",
                    plaintext == Array.Empty<byte>() ? nameof(plaintext) :
                    key == Array.Empty<byte>() ? nameof(key2) :
                    key2 == Array.Empty<byte>() ? nameof(key2) :
                    nameof(hMacKey));

            var cipherText = SecretAeadXChaCha20Poly1305.Encrypt(plaintext, nonce, key);
            var cipherTextL2 = await EncryptAsync(cipherText, key2, nonce2, hMacKey);

            Array.Clear(key, 0, key.Length);
            Array.Clear(key2, 0, key2.Length);
            Array.Clear(hMacKey, 0, hMacKey.Length);

            return nonce.Concat(nonce2).Concat(cipherTextL2).ToArray();
        }
        catch (CryptographicException ex)
        {
            Array.Clear(key, 0, key.Length);
            Array.Clear(key2, 0, key2.Length);
            Array.Clear(hMacKey, 0, hMacKey.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            Array.Clear(key, 0, key.Length);
            Array.Clear(key2, 0, key2.Length);
            Array.Clear(hMacKey, 0, hMacKey.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
    }

    #region UnusedGCM

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
            var nonce = new byte[GcmNonceSize];
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

    #endregion
}