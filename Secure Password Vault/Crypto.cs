using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Sodium;

namespace Secure_Password_Vault;

public static class Crypto
{
    /// <summary>
    ///     Utility class for cryptographic settings and initialization.
    ///     All values represent bytes.
    /// </summary>
    public static class CryptoConstants
    {
        /// <summary>
        ///     Number of iterations for key derivation.
        /// </summary>
        public const int Iterations = 32;

        /// <summary>
        ///     Memory size for key derivation in KiB.
        /// </summary>
        public const int MemorySize = 1024 * 1024 * 5;

        /// <summary>
        ///     Size of the salt used in cryptographic operations.
        /// </summary>
        public const int SaltSize = 128;

        /// <summary>
        ///     Length of the authentication tag used in authenticated encryption.
        /// </summary>
        public const int TagLen = 16;

        /// <summary>
        ///     Length of the HMAC (Hash-based Message Authentication Code).
        /// </summary>
        public const int HmacLength = 64;

        /// <summary>
        ///     Size of the nonce used in ChaCha20-Poly1305 authenticated encryption.
        /// </summary>
        public const int ChaChaNonceSize = 24;

        /// <summary>
        ///     Constant representing the size of the nonce used in GCM encryption.
        /// </summary>
        public const int GcmNonceSize = 12;

        /// <summary>
        ///     Size of the derived key in bytes.
        /// </summary>
        public static readonly int KeySize = 32;

        /// <summary>
        ///     Size of the initialization vector (IV) in bytes.
        /// </summary>
        public static readonly int Iv = 16;

        /// <summary>
        ///     Random number generator for cryptographic operations.
        /// </summary>
        public static RandomNumberGenerator RndNum = RandomNumberGenerator.Create();

        /// <summary>
        ///     Hash value used for various cryptographic purposes.
        /// </summary>
        /// <remarks>
        ///     Initialized as an empty byte array.
        /// </remarks>
        public static byte[]? Hash { get; set; } = Array.Empty<byte>();
    }


    /// <summary>
    /// Hashes a password inside of a char array or derives a key from a password.
    /// </summary>
    /// <param name="passWord">The char array to hash.</param>
    /// <param name="salt">The salt used during the argon2id hashing process.</param>
    /// <param name="outputSize">The output size in bytes.</param>
    /// <returns>Either a derived key or password hash byte array.</returns>
    public static async Task<byte[]> Argon2Id(char[] passWord, byte[] salt, int outputSize)
    {
        try
        {
            // Check for null or empty values
            if (passWord == null || passWord.Length == 0 || salt == null || salt.Length == 0)
                throw new ArgumentException("Value was null or empty.",
                    passWord == null ? nameof(passWord) : nameof(salt));

            // Initialize Argon2id
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(passWord));
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = Environment.ProcessorCount * 2;
            argon2.Iterations = CryptoConstants.Iterations;
            argon2.MemorySize = CryptoConstants.MemorySize;

            // Get the result
            var result = await argon2.GetBytesAsync(outputSize);
            return result;
        }
        catch (Exception ex)
        {
            // Log any other unexpected exceptions
            ErrorLogging.ErrorLog(ex);
            return Array.Empty<byte>();
        }
    }

    /// <summary>
    ///     Compares two password hashes in a secure manner using fixed-time comparison.
    /// </summary>
    /// <param name="hash1">The first password hash.</param>
    /// <param name="hash2">The second password hash.</param>
    /// <returns>
    ///     A Task that completes with 'true' if the hashes are equal, and 'false' otherwise.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     Thrown if either hash is null or empty.
    /// </exception>
    /// <remarks>
    ///     This method uses fixed-time comparison to mitigate certain types of timing attacks.
    /// </remarks>
    public static Task<bool> ComparePassword(byte[]? hash1, byte[]? hash2)
    {
        try
        {
            // Check if either hash is null or empty
            if (hash1 == null || hash1.Length == 0 || hash2 == null || hash2.Length == 0)
                throw new ArgumentException("Value was empty or null.",
                    hash1 == null || hash1.Length == 0 ? nameof(hash1) : nameof(hash2));

            // Use CryptographicOperations.FixedTimeEquals for secure comparison
            return Task.FromResult(CryptographicOperations.FixedTimeEquals(hash1, hash2));
        }
        catch (Exception ex)
        {
            // Log and handle unexpected exceptions
            ErrorLogging.ErrorLog(ex);
            throw;
        }
    }

    private static int RndInt()
    {
        var buffer = new byte[sizeof(int)];
        RandomNumberGenerator.Fill(buffer);
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
        CryptoConstants.RndNum.GetBytes(buffer);
        return buffer;
    }

    /// <summary>
    ///     Clears sensitive data from memory.
    /// </summary>
    /// <param name="data">The data to be cleared.</param>
    private static void ClearSensitiveData(params byte[][] data)
    {
        foreach (var array in data) Array.Clear(array, 0, array.Length);
    }

    /// <summary>
    ///     Encrypts the contents of a file using Argon2 key derivation and xChaCha20-Poly1305 + AES-CBC-Blake2b encryption.
    /// </summary>
    /// <param name="userName">The username associated with the user's salt for key derivation.</param>
    /// <param name="passWord">The user's password used for key derivation.</param>
    /// <param name="file">The path to the file to be encrypted.</param>
    /// <returns>
    ///     A Task that completes with the encrypted content of the specified file.
    ///     If any error occurs during the process, returns an empty byte array.
    /// </returns>
    /// <remarks>
    ///     This method performs the following steps:
    ///     1. Retrieves the user-specific salt using the provided username.
    ///     2. Derives an encryption key from the user's password and the obtained salt using Argon2id.
    ///     3. Extracts key components for encryption, including two keys and an HMAC key.
    ///     4. Reads the content of the specified file.
    ///     5. Encrypts the file content using xChaCha20-Poly1305 + AES-CBC-Blake2b encryption.
    ///     6. Clears sensitive information, such as the user's password, from memory.
    /// </remarks>
    public static async Task<byte[]> EncryptFile(string userName, char[] passWord, string file)
    {
            // Check if any required input is null or empty
            if (string.IsNullOrEmpty(userName) || passWord == null || passWord.Length == 0 || string.IsNullOrEmpty(file))
                throw new ArgumentException("Value was empty.",
                    string.IsNullOrEmpty(userName) ? nameof(userName) :
                    passWord == null || passWord.Length == 0 ? nameof(passWord) : nameof(file));

            // Retrieve user-specific salt
            var salt = await Authentication.GetUserSaltAsync(userName);

            // Derive encryption key from password and salt
            var bytes = await Argon2Id(passWord, salt, 128);
            if (bytes == Array.Empty<byte>())
                throw new Exception("Value was empty.");

            // Extract key components for encryption
            var key = new byte[CryptoConstants.KeySize];
            var key2 = new byte[CryptoConstants.KeySize];
            var hMacKey = new byte[CryptoConstants.HmacLength];

            Buffer.BlockCopy(bytes, 0, key, 0, key.Length);
            Buffer.BlockCopy(bytes, key.Length, key2, 0, key2.Length);
            Buffer.BlockCopy(bytes, key.Length + key2.Length, hMacKey, 0, hMacKey.Length);

            // Read content of the specified file
            var fileStr = await File.ReadAllTextAsync(file);
            var fileBytes = DataConversionHelpers.StringToByteArray(fileStr);

            // Check if file content or salt is empty
            if (fileBytes == null || fileBytes.Length == 0 || salt == null || salt.Length == 0)
                throw new ArgumentException("Value was empty.",
                    fileBytes == null || fileBytes.Length == 0 ? nameof(fileBytes) : nameof(salt));

            // Encrypt the file content
            var encryptedFile = await EncryptAsyncV3(fileBytes, key, key2, hMacKey);

            // Clear sensitive information
            ClearSensitiveData(key, key2, hMacKey, Encoding.UTF8.GetBytes(passWord));

            return encryptedFile;
        }

    /// <summary>
    ///     Decrypts the contents of an encrypted file using Argon2 key derivation and ChaCha20-Poly1305 decryption.
    /// </summary>
    /// <param name="userName">The username associated with the user's salt for key derivation.</param>
    /// <param name="passWord">The user's password used for key derivation.</param>
    /// <param name="file">The path to the encrypted file to be decrypted.</param>
    /// <returns>
    ///     A Task that completes with the decrypted content of the specified encrypted file.
    ///     If any error occurs during the process, returns an empty byte array.
    /// </returns>
    /// <remarks>
    ///     This method performs the following steps:
    ///     1. Validates input parameters to ensure they are not null or empty.
    ///     2. Retrieves the user-specific salts for key derivation.
    ///     3. Derives an encryption key from the user's password and the obtained salt using Argon2id.
    ///     4. Extracts key components for decryption, including two keys and an HMAC key.
    ///     5. Reads and decodes the content of the encrypted file.
    ///     6. Decrypts the file content using ChaCha20-Poly1305 decryption.
    ///     7. Clears sensitive information, such as the user's password, from memory.
    /// </remarks>
    public static async Task<byte[]> DecryptFile(string userName, char[] passWord, string file)
    {
        // Validate input parameters
        if (string.IsNullOrEmpty(userName) || passWord == null || passWord.Length == 0 ||
            string.IsNullOrEmpty(file))
            throw new ArgumentException("Value was empty.",
                string.IsNullOrEmpty(userName) ? nameof(userName) :
                passWord == null || passWord.Length == 0 ? nameof(passWord) : nameof(file));

        // Retrieve user-specific salts for key derivation
        var salt = await Authentication.GetUserSaltAsync(userName);

        // Derive decryption key from password and salts using Argon2id
        var bytes = await Argon2Id(passWord, salt, 128);

        // Extract key components for decryption
        var key = new byte[CryptoConstants.KeySize];
        var key2 = new byte[CryptoConstants.KeySize];
        var hMacKey = new byte[CryptoConstants.HmacLength];

        Buffer.BlockCopy(bytes, 0, key, 0, key.Length);
        Buffer.BlockCopy(bytes, key.Length, key2, 0, key2.Length);
        Buffer.BlockCopy(bytes, key.Length + key2.Length, hMacKey, 0, hMacKey.Length);

        // Read and decode the content of the encrypted file
        var fileStr = await File.ReadAllTextAsync(file);
        var fileBytes = DataConversionHelpers.Base64StringToByteArray(fileStr);

        // Check if file content or salt is empty
        if (fileBytes == Array.Empty<byte>() || salt == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.",
                fileBytes == Array.Empty<byte>() ? nameof(fileBytes) : nameof(salt));

        // Decrypt the file content
        var decryptedFile = await DecryptAsyncV3(fileBytes, key, key2, hMacKey);

        // Clear sensitive information
        Array.Clear(passWord, 0, passWord.Length);

        return decryptedFile;
    }

#pragma warning disable

    private const int BlockBitSize = 128;
    private const int KeyBitSize = 256;

    /// <summary>
    ///     Encrypts the provided plaintext using AES encryption with CBC mode and PKCS7 padding.
    ///     The ciphertext is then authenticated using HMAC-Blake2B.
    /// </summary>
    /// <param name="plainText">The plaintext to be encrypted.</param>
    /// <param name="key">The encryption key.</param>
    /// <param name="iv">The initialization vector.</param>
    /// <param name="hMacKey">The HMAC key for authentication.</param>
    /// <returns>
    ///     A Task that completes with the authenticated ciphertext.
    ///     If any error occurs during the process, returns an empty byte array.
    /// </returns>
    /// <remarks>
    ///     This method performs the following steps:
    ///     1. Validates input parameters to ensure they are not null or empty.
    ///     2. Creates an AES object with specified block size, key size, mode, and padding.
    ///     3. Encrypts the plaintext using AES encryption with CBC mode and PKCS7 padding.
    ///     4. Computes HMAC-Blake2B over the IV and ciphertext for authentication.
    ///     5. Clears sensitive information, such as encryption key and HMAC key, from memory.
    /// </remarks>
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
            using var hmac = new HMACBlake2B(hMacKey, CryptoConstants.HmacLength * 8);
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
        catch (Exception ex)
        {
            // Clear sensitive information
            ClearSensitiveData(key, hMacKey);
            throw;
        }
    }

    /// <summary>
    ///     Decrypts the provided ciphertext using AES decryption with CBC mode and PKCS7 padding.
    ///     The authenticity of the ciphertext is verified using HMAC-Blake2B.
    /// </summary>
    /// <param name="cipherText">The ciphertext to be decrypted.</param>
    /// <param name="key">The decryption key.</param>
    /// <param name="hMacKey">The HMAC key for authentication.</param>
    /// <returns>
    ///     A Task that completes with the decrypted plaintext.
    ///     If any error occurs during the process, returns an empty byte array.
    /// </returns>
    /// <remarks>
    ///     This method performs the following steps:
    ///     1. Validates input parameters to ensure they are not null or empty.
    ///     2. Creates an AES object with specified block size, key size, mode, and padding.
    ///     3. Verifies the authenticity of the ciphertext using HMAC-Blake2B.
    ///     4. Decrypts the ciphertext using AES decryption with CBC mode and PKCS7 padding.
    ///     5. Clears sensitive information, such as the decryption key and HMAC key, from memory.
    /// </remarks>
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
            using var hmac = new HMACBlake2B(hMacKey, CryptoConstants.HmacLength * 8);
            var receivedHash = new byte[CryptoConstants.HmacLength];

            // Place the received hash into receivedHash byte array
            Buffer.BlockCopy(cipherText, cipherText.Length - CryptoConstants.HmacLength, receivedHash, 0,
                CryptoConstants.HmacLength);

            // Create byte array for IV and cipherText
            var cipherWithIv = new byte[cipherText.Length - CryptoConstants.HmacLength];

            // Place cipherText and IV into cipherWithIv byte array
            Buffer.BlockCopy(cipherText, 0, cipherWithIv, 0, cipherText.Length - CryptoConstants.HmacLength);

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
            var iv = new byte[CryptoConstants.Iv];
            var cipherResult = new byte[cipherText.Length - iv.Length - CryptoConstants.HmacLength];

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
        catch (Exception ex)
        {
            // Clear sensitive information
            ClearSensitiveData(key, hMacKey);
            throw;
        }
#pragma warning restore
    }

    /// <summary>
    ///     Encrypts the provided plaintext using a combination of XChaCha20-Poly1305 and AES encryption.
    /// </summary>
    /// <param name="plaintext">The plaintext to be encrypted.</param>
    /// <param name="key">The first encryption key.</param>
    /// <param name="key2">The second encryption key.</param>
    /// <param name="hMacKey">The HMAC key for authentication.</param>
    /// <returns>
    ///     A Task that completes with the encrypted ciphertext.
    ///     If any error occurs during the process, returns an empty byte array.
    /// </returns>
    /// <remarks>
    ///     This method performs the following steps:
    ///     1. Validates input parameters to ensure they are not null or empty.
    ///     2. Generates nonces for XChaCha20-Poly1305 and AES encryption.
    ///     3. Encrypts the plaintext using XChaCha20-Poly1305 with the first key.
    ///     4. Encrypts the XChaCha20-Poly1305 ciphertext using AES encryption with the second key.
    ///     5. Clears sensitive information, such as encryption keys, from memory.
    /// </remarks>
    public static async Task<byte[]> EncryptAsyncV3(byte[] plaintext,
        byte[] key, byte[] key2, byte[] hMacKey)
    {
        try
        {
            // Parameter checks
            if (plaintext == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty.", nameof(plaintext));

            // Generate nonces
            var nonce = RndByteSized(CryptoConstants.ChaChaNonceSize);
            var nonce2 = RndByteSized(CryptoConstants.Iv);

            // Encrypt using XChaCha20-Poly1305
            var cipherText = SecretAeadXChaCha20Poly1305.Encrypt(plaintext, nonce, key);
            if (cipherText == null)
                throw new Exception("Value was empty.");

            // Encrypt the XChaCha20-Poly1305 ciphertext using AES
            var cipherTextL2 = await EncryptAsync(cipherText, key2, nonce2, hMacKey);
            if (cipherTextL2 == null)
                throw new Exception("Value was empty.");

            // Clear sensitive information
            ClearSensitiveData(key, key2, hMacKey);

            // Concatenate nonces and the second level ciphertext
            return nonce.Concat(nonce2).Concat(cipherTextL2).ToArray();
        }
        catch (Exception)
        {
            // Clear sensitive information
            ClearSensitiveData(key, key2, hMacKey);
            throw;
        }
    }


    /// <summary>
    ///     Decrypts the provided ciphertext using a combination of XChaCha20-Poly1305 and AES decryption.
    /// </summary>
    /// <param name="cipherText">The ciphertext to be decrypted.</param>
    /// <param name="key">The first decryption key.</param>
    /// <param name="key2">The second decryption key.</param>
    /// <param name="hMacKey">The HMAC key for authentication.</param>
    /// <returns>
    ///     A Task that completes with the decrypted plaintext.
    ///     If any error occurs during the process, returns an empty byte array.
    /// </returns>
    /// <remarks>
    ///     This method performs the following steps:
    ///     1. Validates input parameters to ensure they are not null or empty.
    ///     2. Extracts nonces and ciphertext from the provided cipherText.
    ///     3. Decrypts the ciphertext using AES decryption with the second key.
    ///     4. Decrypts the AES ciphertext using XChaCha20-Poly1305 with the first key.
    ///     5. Clears sensitive information, such as decryption keys, from memory.
    /// </remarks>
    public static async Task<byte[]> DecryptAsyncV3(byte[] cipherText,
        byte[] key, byte[] key2, byte[] hMacKey)
    {
        try
        {
            // Parameter checks
            if (cipherText == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty.", nameof(cipherText));

            // Extract nonces and ciphertext
            var nonce = new byte[CryptoConstants.ChaChaNonceSize];
            Buffer.BlockCopy(cipherText, 0, nonce, 0, nonce.Length);

            var nonce2 = new byte[CryptoConstants.Iv];
            Buffer.BlockCopy(cipherText, nonce.Length, nonce2, 0, nonce2.Length);

            var cipherResult =
                new byte[cipherText.Length - nonce2.Length - nonce.Length];

            Buffer.BlockCopy(cipherText, nonce.Length + nonce2.Length, cipherResult, 0,
                cipherResult.Length);

            // Decrypt using AES with the second key
            var resultL2 = await DecryptAsync(cipherResult, key2, hMacKey);

            // Decrypt the AES ciphertext using XChaCha20-Poly1305 with the first key
            var resultL0 = SecretAeadXChaCha20Poly1305.Decrypt(resultL2, nonce, key);

            // Clear sensitive information
            ClearSensitiveData(key, key2, hMacKey);

            return resultL0;
        }
        catch (Exception)
        {
            // Clear sensitive information
            ClearSensitiveData(key, key2, hMacKey);
            throw;
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
            var tag = new byte[CryptoConstants.TagLen];

            using (var argon2 = new Argon2id(password))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = Environment.ProcessorCount * 2;
                argon2.Iterations = CryptoConstants.Iterations;
                argon2.MemorySize = CryptoConstants.MemorySize;

                var key = await argon2.GetBytesAsync(CryptoConstants.KeySize);

                using var aesGcm = new AesGcm(key, CryptoConstants.TagLen);
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
            argon2.Iterations = CryptoConstants.Iterations;
            argon2.MemorySize = CryptoConstants.MemorySize;

            var key = await argon2.GetBytesAsync(CryptoConstants.KeySize);
            using var aesGcm = new AesGcm(key, CryptoConstants.TagLen);
            var tag = new byte[CryptoConstants.TagLen];
            var nonce = new byte[CryptoConstants.GcmNonceSize];
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
