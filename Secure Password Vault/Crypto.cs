using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Sodium;

namespace Secure_Password_Vault;

public static class Crypto
{
    private const int Iterations = 32;
    private const double MemorySize = 1024d * 1024d * 5d;
    public const int SaltSize = 512 / 8;
    public static readonly int ByteSize = 24;
    public static readonly int KeySize = 32;
    public static readonly int IvBit = 128;
    private const int TagLen = 16;
    private const int HmacLength = 64;
    private const int GcmNonceSize = 12;
    private const int ChaChaNonceSize = 24;

    private static readonly RandomNumberGenerator RndNum = RandomNumberGenerator.Create();

    public static byte[]? Hash { get; set; } = Array.Empty<byte>();

    /// <summary>
    ///     Hashes a password inside of a char array.
    /// </summary>
    /// <param name="passWord">The char array to hash.</param>
    /// <param name="salt">The salt used during the argon2id hashing process.</param>
    /// <returns>A password hash byte array.</returns>
    public static async Task<byte[]?> HashAsync(char[]? passWord, byte[]? salt)
    {
        try
        {
            if (passWord == Array.Empty<char>() || salt == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty.",
                    passWord == Array.Empty<char>() ? nameof(passWord) : nameof(salt));

            using var argon2 =
                new Argon2id(Encoding.UTF8.GetBytes(passWord ?? throw new ArgumentNullException(nameof(passWord))));
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

            var fileStr = await File.ReadAllTextAsync(file);
            var fileBytes = DataConversionHelpers.StringToByteArray(fileStr);

            var passwordBytes = Encoding.UTF8.GetBytes(passWord);

            if (fileBytes == Array.Empty<byte>() || salt == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty.",
                    fileBytes == Array.Empty<byte>() ? nameof(fileBytes) : nameof(salt));

            var encryptedFile = await EncryptAsyncV3(fileBytes, salt, salt2, salt3, passwordBytes);

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

            var fileStr = await File.ReadAllTextAsync(file);
            var fileBytes = DataConversionHelpers.Base64StringToByteArray(fileStr);

            var passwordBytes = Encoding.UTF8.GetBytes(passWord);

            if (fileBytes == Array.Empty<byte>() || salt == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty.",
                    fileBytes == Array.Empty<byte>() ? nameof(fileBytes) : nameof(salt));

            var decryptedFile = await DecryptAsyncV3(fileBytes, salt, salt2, salt3, passwordBytes);

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

    public static async Task<byte[]> EncryptAsync(byte[] plainText, byte[] key, byte[] iv, byte[] salt)
    {
        try
        {
            if (plainText == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty or null.", nameof(plainText));
            if (key == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty or null.", nameof(key));
            if (salt == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty or null.", nameof(salt));
            if (iv == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty or null.", nameof(iv));

            using var aes = Aes.Create();
            aes.BlockSize = BlockBitSize;
            aes.KeySize = KeyBitSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var argon2 = new Argon2id(key);
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = Environment.ProcessorCount * 2;
            argon2.Iterations = Iterations;
            argon2.MemorySize = (int)MemorySize;

            var hmacKey = await argon2.GetBytesAsync(HmacLength);

            byte[] cipherText;

            using (var encryptor = aes.CreateEncryptor(key, iv))
            using (var memStream = new MemoryStream())
            {
                await using (var cryptoStream =
                             new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                {
                    using (var cipherStream = new MemoryStream(plainText))
                    {
                        cipherStream.FlushAsync();
                        cipherStream.CopyToAsync(cryptoStream, (int)cipherStream.Length);
                    }

                    cryptoStream.FlushFinalBlockAsync();
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
            Array.Clear(key, 0, key.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (ArgumentNullException ex)
        {
            Array.Clear(key, 0, key.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (ObjectDisposedException ex)
        {
            Array.Clear(key, 0, key.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            Array.Clear(key, 0, key.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
    }

    public static async Task<byte[]> DecryptAsync(byte[] cipherText, byte[] key, byte[] salt)
    {
        try
        {
            if (cipherText == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty or null.", nameof(cipherText));
            if (key == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty or null.", nameof(key));
            if (salt == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty or null.", nameof(salt));

            using var aes = Aes.Create();
            aes.BlockSize = BlockBitSize;
            aes.KeySize = KeyBitSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var argon2 = new Argon2id(key);
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = Environment.ProcessorCount * 2;
            argon2.Iterations = Iterations;
            argon2.MemorySize = (int)MemorySize;

            var hmacKey = await argon2.GetBytesAsync(HmacLength);

            using var hmac = new HMACSHA512(hmacKey);
            var receivedHash = new byte[HmacLength];

            Buffer.BlockCopy(cipherText, cipherText.Length - HmacLength, receivedHash, 0, HmacLength);

            var cipherWithIv = new byte[cipherText.Length - HmacLength];

            Buffer.BlockCopy(cipherText, 0, cipherWithIv, 0, cipherText.Length - HmacLength);

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
                    plainStream.FlushAsync();
                }

                await decryptStream.FlushFinalBlockAsync();
            }

            Array.Clear(key, 0, key.Length);

            return memStream.ToArray();
        }
        catch (CryptographicException ex)
        {
            Array.Clear(key, 0, key.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (ArgumentNullException ex)
        {
            Array.Clear(key, 0, key.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (ObjectDisposedException ex)
        {
            Array.Clear(key, 0, key.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            Array.Clear(key, 0, key.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
#pragma warning restore
    }

    public static byte[] EncryptXSalsaPoly1305(byte[] input, byte[] key, byte[] nonce)
    {
        var result = StreamEncryption.Encrypt(input, nonce, key);
        var authentication = OneTimeAuth.Sign(result, key);
        return result.Concat(authentication).ToArray();
    }

    public static byte[] DecryptXSalsaPoly1305(byte[] input, byte[] key, byte[] nonce)
    {
        try
        {
            var receivedHash = new byte[16];
            Buffer.BlockCopy(input, input.Length - receivedHash.Length, receivedHash, 0, receivedHash.Length);
            var inputText = new byte[input.Length - receivedHash.Length];
            Buffer.BlockCopy(input, 0, inputText, 0, inputText.Length);

            var originalHash = OneTimeAuth.Sign(inputText, key);
            var isMatch = CryptographicOperations.FixedTimeEquals(receivedHash, originalHash);

            if (!isMatch)
                throw new CryptographicException("Invalid tag.");

            var result = StreamEncryption.Decrypt(inputText, nonce, key);

            return result;
        }
        catch (CryptographicException ex)
        {
            Array.Clear(key, 0, key.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            Array.Clear(key, 0, key.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
    }

    public static async Task<byte[]> EncryptAsyncV3(byte[] plaintext, byte[] salt, byte[] salt2, byte[] salt3,
        byte[] password)
    {
        using var argon2 = new Argon2id(password);
        argon2.Salt = salt;
        argon2.DegreeOfParallelism = Environment.ProcessorCount * 2;
        argon2.Iterations = Iterations;
        argon2.MemorySize = (int)MemorySize;

        var key = await argon2.GetBytesAsync(KeySize);

        using var argon2L2 = new Argon2id(key);
        argon2L2.Salt = salt2;
        argon2L2.DegreeOfParallelism = Environment.ProcessorCount * 2;
        argon2L2.Iterations = Iterations;
        argon2L2.MemorySize = (int)MemorySize;

        var key2 = await argon2L2.GetBytesAsync(KeySize);

        using var argon2L3 = new Argon2id(key2);
        argon2L3.Salt = salt3;
        argon2L3.DegreeOfParallelism = Environment.ProcessorCount * 2;
        argon2L3.Iterations = Iterations;
        argon2L3.MemorySize = (int)MemorySize;

        var key3 = await argon2L3.GetBytesAsync(KeySize);

        try
        {
            var nonce = RndByteSized(ChaChaNonceSize);
            var nonce2 = RndByteSized(ChaChaNonceSize);
            var nonce3 = RndByteSized(IvBit / 8);

            var cipherText = SecretAeadXChaCha20Poly1305.Encrypt(plaintext, nonce, key);
            var cipherTextL2 = EncryptXSalsaPoly1305(cipherText, key2, nonce2);
            var cipherTextL3 = await EncryptAsync(cipherTextL2, key3, nonce3, salt);

            Array.Clear(key, 0, key.Length);
            Array.Clear(key2, 0, key2.Length);
            Array.Clear(key3, 0, key3.Length);

            return nonce.Concat(nonce2).Concat(nonce3).Concat(cipherTextL3).ToArray();
        }
        catch (CryptographicException ex)
        {
            Array.Clear(key, 0, key.Length);
            Array.Clear(key2, 0, key2.Length);
            Array.Clear(key3, 0, key3.Length);
            Array.Clear(password, 0, password.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            Array.Clear(key, 0, key.Length);
            Array.Clear(key2, 0, key2.Length);
            Array.Clear(key3, 0, key3.Length);
            Array.Clear(password, 0, password.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
    }

    public static async Task<byte[]> DecryptAsyncV3(byte[] cipherText, byte[] salt, byte[] salt2, byte[] salt3,
        byte[] password)
    {
        using var argon2 = new Argon2id(password);
        argon2.Salt = salt;
        argon2.DegreeOfParallelism = Environment.ProcessorCount * 2;
        argon2.Iterations = Iterations;
        argon2.MemorySize = (int)MemorySize;

        var key = await argon2.GetBytesAsync(KeySize);

        using var argon2L2 = new Argon2id(key);
        argon2L2.Salt = salt2;
        argon2L2.DegreeOfParallelism = Environment.ProcessorCount * 2;
        argon2L2.Iterations = Iterations;
        argon2L2.MemorySize = (int)MemorySize;

        var key2 = await argon2L2.GetBytesAsync(KeySize);

        using var argon2L3 = new Argon2id(key2);
        argon2L3.Salt = salt3;
        argon2L3.DegreeOfParallelism = Environment.ProcessorCount * 2;
        argon2L3.Iterations = Iterations;
        argon2L3.MemorySize = (int)MemorySize;

        var key3 = await argon2L3.GetBytesAsync(KeySize);

        try
        {
            if (cipherText == Array.Empty<byte>() || salt == Array.Empty<byte>() || salt2 == Array.Empty<byte>()
                || salt3 == Array.Empty<byte>() || password == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty.",
                    cipherText == Array.Empty<byte>() ? nameof(cipherText) :
                    salt == Array.Empty<byte>() ? nameof(salt) :
                    salt2 == Array.Empty<byte>() ? nameof(salt2) :
                    salt3 == Array.Empty<byte>() ? nameof(salt3) :
                    nameof(password));

            var nonce = new byte[ChaChaNonceSize];
            Buffer.BlockCopy(cipherText, 0, nonce, 0, nonce.Length);

            var nonce2 = new byte[ChaChaNonceSize];
            Buffer.BlockCopy(cipherText, nonce.Length, nonce2, 0, nonce2.Length);

            var nonce3 = new byte[IvBit / 8];
            Buffer.BlockCopy(cipherText, nonce.Length + nonce2.Length, nonce3, 0, nonce3.Length);

            var cipherResult =
                new byte[cipherText.Length - nonce3.Length - nonce2.Length - nonce.Length];

            Buffer.BlockCopy(cipherText, nonce.Length + nonce2.Length + nonce3.Length, cipherResult, 0,
                cipherResult.Length);

            var resultL3 = await DecryptAsync(cipherResult, key3, salt);
            var resultL2 = DecryptXSalsaPoly1305(resultL3, key2, nonce2);
            var resultL0 = SecretAeadXChaCha20Poly1305.Decrypt(resultL2, nonce, key);


            Array.Clear(key, 0, key.Length);
            Array.Clear(key2, 0, key2.Length);
            Array.Clear(key3, 0, key3.Length);

            return resultL0;
        }
        catch (CryptographicException ex)
        {
            Array.Clear(key, 0, key.Length);
            Array.Clear(key2, 0, key2.Length);
            Array.Clear(key3, 0, key3.Length);
            Array.Clear(password, 0, password.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            Array.Clear(key, 0, key.Length);
            Array.Clear(key2, 0, key2.Length);
            Array.Clear(key3, 0, key3.Length);
            Array.Clear(password, 0, password.Length);
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
    }

    public static async Task<byte[]> EncryptAsyncV3Debug(byte[] plaintext, byte[] salt, byte[] salt2, byte[] salt3,
        byte[] salt4, byte[] password, byte[] nonce, byte[] nonce2, byte[] nonce3, byte[] nonce4)
    {
        // Debug method allows us to set the nonce manually.
        using var argon2 = new Argon2id(password);
        argon2.Salt = salt;
        argon2.DegreeOfParallelism = Environment.ProcessorCount * 2;
        argon2.Iterations = Iterations;
        argon2.MemorySize = (int)MemorySize;

        var key = await argon2.GetBytesAsync(KeySize);

        using var argon2L2 = new Argon2id(key);
        argon2L2.Salt = salt2;
        argon2L2.DegreeOfParallelism = Environment.ProcessorCount * 2;
        argon2L2.Iterations = Iterations;
        argon2L2.MemorySize = (int)MemorySize;

        var key2 = await argon2L2.GetBytesAsync(KeySize);

        using var argon2L3 = new Argon2id(key2);
        argon2L3.Salt = salt3;
        argon2L3.DegreeOfParallelism = Environment.ProcessorCount * 2;
        argon2L3.Iterations = Iterations;
        argon2L3.MemorySize = (int)MemorySize;

        var key3 = await argon2L3.GetBytesAsync(KeySize);

        using var argon2L4 = new Argon2id(key3);
        argon2L4.Salt = salt4;
        argon2L4.DegreeOfParallelism = Environment.ProcessorCount * 2;
        argon2L4.Iterations = Iterations;
        argon2L4.MemorySize = (int)MemorySize;

        var key4 = await argon2L4.GetBytesAsync(KeySize);

        try
        {
            var cipherText = SecretAeadXChaCha20Poly1305.Encrypt(plaintext, nonce, key);
            var cipherTextL2 = SecretAeadXChaCha20Poly1305.Encrypt(cipherText, nonce2, key2);
            var cipherTextL3 = SecretAeadXChaCha20Poly1305.Encrypt(cipherTextL2, nonce3, key3);
            var cipherTextL4 = SecretAeadXChaCha20Poly1305.Encrypt(cipherTextL3, nonce4, key4);

            Array.Clear(key, 0, key.Length);
            Array.Clear(key2, 0, key2.Length);
            Array.Clear(key3, 0, key3.Length);
            Array.Clear(key4, 0, key4.Length);

            var cipherResult = nonce.Concat(nonce2).Concat(nonce3).Concat(nonce4).Concat(cipherTextL4).ToArray();

            return cipherResult;
        }

        catch (Exception ex)
        {
            Array.Clear(key, 0, key.Length);
            Array.Clear(key2, 0, key2.Length);
            Array.Clear(key3, 0, key3.Length);
            Array.Clear(key4, 0, key4.Length);
            MessageBox.Show(@"There was an error during encryption.", @"Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            Array.Clear(password, 0, password.Length);
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