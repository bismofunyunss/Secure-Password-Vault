﻿using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Sodium;

namespace Secure_Password_Vault;

public static class Crypto
{
    public static class Memory
    {
        [DllImport("MemoryManager.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void clearMemory(IntPtr ptr, IntPtr size);
    }

    /// <summary>
    ///     Utility class for cryptographic settings and initialization.
    /// </summary>
    public static class CryptoConstants
    {
        /// <summary>
        ///     Number of iterations for key derivation.
        /// </summary>
        public const int Iterations = 1;

        /// <summary>
        ///     Memory size for key derivation in KiB.
        /// </summary>
        public const int MemorySize = 1024 * 1024 * 1;

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
        public const int KeySize = 32;

        /// <summary>
        ///     Size of the initialization vector (IV) in bytes.
        /// </summary>
        public const int Iv = 16;

        /// <summary>
        ///     Size of the ThreeFish initialization vector (IV) and key in bytes.
        /// </summary>
        public const int ThreeFish = 128;

        /// <summary>
        ///     Size of the shuffle key in bytes.
        /// </summary>
        public const int ShuffleKey = 128;

        /// <summary>
        ///     Random number generator for cryptographic operations.
        /// </summary>
        public static readonly RandomNumberGenerator RndNum = RandomNumberGenerator.Create();

        /// <summary>
        ///     Hash value used for various cryptographic purposes.
        /// </summary>
        /// <remarks>
        ///     Initialized as an empty byte array.
        /// </remarks>
        public static byte[] Hash = Array.Empty<byte>();

        /// <summary>
        ///     SecureString password value.
        /// </summary>
        /// <remarks>
        ///     Holds the user's password as a SecureString.
        /// </remarks>
        public static SecureString SecurePassword = new();
    }

    /// <summary>
    ///     Hashes a password inside a char array or derives a key from a password.
    /// </summary>
    /// <param name="passWord">The char array to hash.</param>
    /// <param name="salt">The salt used during the argon2id hashing process.</param>
    /// <param name="outputSize">The output size in bytes.</param>
    /// <returns>Either a derived key or password hash byte array.</returns>
    public static async Task<byte[]> Argon2Id(char[] passWord, byte[] salt, int outputSize)
    {
        // Check for null or empty values
        if (passWord == null || passWord.Length == 0 || salt == null || salt.Length == 0)
            throw new ArgumentException("Value was null or empty.",
                passWord == null ? nameof(passWord) : nameof(salt));

        var passwordBytes = Encoding.UTF8.GetBytes(passWord);

        // Initialize Argon2id
        using var argon2 = new Argon2id(passwordBytes);
        argon2.Salt = salt;
        argon2.DegreeOfParallelism = Environment.ProcessorCount * 2;
        argon2.Iterations = CryptoConstants.Iterations;
        argon2.MemorySize = CryptoConstants.MemorySize;

        // Get the result
        var result = await argon2.GetBytesAsync(outputSize);

        ClearBytes(passwordBytes);
        return result;
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
    public static async Task<bool> ComparePassword(byte[]? hash1, byte[]? hash2)
    {
        // Check if either hash is null or empty
        if (hash1 == null || hash1.Length == 0 || hash2 == null || hash2.Length == 0)
            throw new ArgumentException("Value was empty or null.",
                hash1 == null || hash1.Length == 0 ? nameof(hash1) : nameof(hash2));
        // Use CryptographicOperations.FixedTimeEquals for secure comparison
        var result = await Task.FromResult(CryptographicOperations.FixedTimeEquals(hash1, hash2));
        ClearBytes(hash1, hash2);
        return result;
    }

    /// <summary>
    ///     Generates a random integer using a cryptographically secure random number generator.
    /// </summary>
    /// <returns>A random integer value.</returns>
    /// <remarks>
    ///     This method generates a random integer by obtaining random bytes from a cryptographically secure
    ///     random number generator and converting them to an integer using the BitConverter class.
    /// </remarks>
    private static int RndInt()
    {
        // Create a buffer to store random bytes
        var buffer = new byte[sizeof(int)];

        // Generate random bytes using a cryptographically secure random number generator
        CryptoConstants.RndNum.GetBytes(buffer);

        // Convert the random bytes to an integer
        var result = BitConverter.ToInt32(buffer, 0);

        return result;
    }


    /// <summary>
    ///     Generates a random integer within the specified inclusive range.
    /// </summary>
    /// <param name="min">The minimum value of the range (inclusive).</param>
    /// <param name="max">The maximum value of the range (inclusive).</param>
    /// <returns>A random integer within the specified range.</returns>
    /// <exception cref="ArgumentException">
    ///     Thrown if the specified minimum value is greater than or equal to the maximum
    ///     value.
    /// </exception>
    /// <remarks>
    ///     This method generates a random integer within the specified inclusive range using the RndInt method.
    ///     The generated integer is constrained to the provided range by applying modular arithmetic.
    /// </remarks>
    public static int BoundedInt(int min, int max)
    {
        // Validate input parameters
        if (min >= max)
            throw new ArgumentException("Min must be less than max.");

        // Generate a random integer using the RndInt method
        var value = RndInt();

        // Calculate the range of values within the specified range
        var range = max - min;

        // Constrain the random integer to the specified range using modular arithmetic
        var result = min + Math.Abs(value) % range;

        return result;
    }

    /// <summary>
    ///     Generates a random byte array of the specified size using a cryptographically secure random number generator.
    /// </summary>
    /// <param name="size">The size of the byte array to generate.</param>
    /// <returns>A random byte array of the specified size.</returns>
    /// <remarks>
    ///     This method generates a random byte array by obtaining random bytes from a cryptographically secure
    ///     random number generator. The size of the byte array is determined by the input parameter 'size'.
    /// </remarks>
    public static byte[] RndByteSized(int size)
    {
        // Create a buffer to store random bytes
        var buffer = new byte[size];

        // Generate random bytes using a cryptographically secure random number generator
        CryptoConstants.RndNum.GetBytes(buffer);

        return buffer;
    }

    /// <summary>
    ///     Clears the sensitive information stored in one or more byte arrays securely.
    /// </summary>
    /// <remarks>
    ///     This method uses a pinned array and the SecureMemoryClear function to overwrite the memory
    ///     containing sensitive information, enhancing security by preventing the information from being
    ///     easily accessible in memory.
    /// </remarks>
    /// <param name="arrays">The byte arrays containing sensitive information to be cleared.</param>
    /// <exception cref="ArgumentNullException">Thrown if any of the input arrays is null.</exception>
    public static void ClearBytes(params byte[][] arrays)
    {
        if (arrays == null)
            throw new ArgumentNullException(nameof(arrays), "Input arrays cannot be null.");

        foreach (var array in arrays)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array), "Input array cannot be null.");

            // Pin the array to get a stable memory address
            var handle = GCHandle.Alloc(array, GCHandleType.Pinned);

            try
            {
                // Clear the memory using the SecureMemoryClear function
                Memory.clearMemory(handle.AddrOfPinnedObject(), (IntPtr)array.Length * sizeof(byte));
            }
            finally
            {
                // Release the pinned array
                handle.Free();
            }
        }
    }

    /// <summary>
    ///     Clears the sensitive information stored in one or more char arrays securely.
    /// </summary>
    /// <remarks>
    ///     This method uses a pinned array and the SecureMemoryClear function to overwrite the memory
    ///     containing sensitive information, enhancing security by preventing the information from being
    ///     easily accessible in memory.
    /// </remarks>
    /// <param name="arrays">The char arrays containing sensitive information to be cleared.</param>
    /// <exception cref="ArgumentNullException">Thrown if any of the input arrays is null.</exception>
    public static void ClearChars(params char[][] arrays)
    {
        if (arrays == null)
            throw new ArgumentNullException(nameof(arrays), "Input arrays cannot be null.");

        foreach (var array in arrays)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array), "Input array cannot be null.");

            // Pin the array to get a stable memory address
            var handle = GCHandle.Alloc(array, GCHandleType.Pinned);

            try
            {
                // Clear the memory using the SecureMemoryClear function
                Memory.clearMemory(handle.AddrOfPinnedObject(), (IntPtr)array.Length * sizeof(char));
            }
            finally
            {
                // Release the pinned array
                handle.Free();
            }
        }
    }

    /// <summary>
    ///     Clears the sensitive information stored in one or more strings securely.
    /// </summary>
    /// <remarks>
    ///     This method uses a pinned string and the SecureMemoryClear function to overwrite the memory
    ///     containing sensitive information, enhancing security by preventing the information from being
    ///     easily accessible in memory.
    /// </remarks>
    /// <param name="str">The strings containing sensitive information to be cleared.</param>
    /// <exception cref="ArgumentNullException">Thrown if any of the input strings is null.</exception>
    public static void ClearStr(params string[] str)
    {
        if (str == null)
            throw new ArgumentNullException(nameof(str), "Input strings cannot be null.");

        foreach (var value in str)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Input string cannot be null.");

            // Pin the string to get a stable memory address
            var handle = GCHandle.Alloc(value, GCHandleType.Pinned);

            try
            {
                // Clear the memory using the SecureMemoryClear function
                Memory.clearMemory(handle.AddrOfPinnedObject(), (IntPtr)value.Length * 2);
            }
            finally
            {
                // Release the pinned string
                handle.Free();
            }
        }
    }

    /// <summary>
    ///     Converts a character array to a SecureString.
    /// </summary>
    /// <remarks>
    ///     This method creates a SecureString and appends each character from the provided
    ///     character array to enhance the security of sensitive information. The resulting
    ///     SecureString is read-only to prevent further modifications.
    /// </remarks>
    /// <param name="charArray">The character array to be converted to a SecureString.</param>
    /// <returns>A SecureString containing the characters from the input array.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the input character array is null or empty.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if the SecureString is disposed due to an exception.</exception>
    public static SecureString ConvertCharArrayToSecureString(char[] charArray)
    {
        if (charArray == null || charArray.Length == 0)
            throw new ArgumentNullException(nameof(charArray), "Input character array cannot be null or empty.");

        var secureString = new SecureString();

        try
        {
            foreach (var c in charArray)
                secureString.AppendChar(c);

            // Make sure the SecureString is read-only to enhance security.
            secureString.MakeReadOnly();
        }
        catch
        {
            // Dispose if there is an exception to avoid memory leaks.
            secureString.Dispose();
            throw;
        }

        return secureString;
    }

    /// <summary>
    ///     Converts an unmanaged string represented by a pointer to a SecureString.
    /// </summary>
    /// <remarks>
    ///     This method reads characters from the unmanaged string pointed to by the provided pointer
    ///     until a null character is encountered. The characters are appended to a SecureString to
    ///     enhance the security of sensitive information. The resulting SecureString is read-only to
    ///     prevent further modifications.
    /// </remarks>
    /// <param name="unmanagedString">Pointer to an unmanaged string to be converted to a SecureString.</param>
    /// <returns>A SecureString containing the characters from the unmanaged string.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the input unmanaged string pointer is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if the SecureString is disposed due to an exception.</exception>
    public static SecureString ConvertUnmanagedStringToSecureString(IntPtr unmanagedString)
    {
        if (unmanagedString == IntPtr.Zero)
            throw new ArgumentNullException(nameof(unmanagedString), "Pointer to unmanaged string is null.");

        var secureString = new SecureString();

        try
        {
            // Loop through the unmanaged string until a null character is encountered
            var offset = 0;
            while (true)
            {
                var c = (char)Marshal.ReadInt16(unmanagedString, offset);
                if (c == '\0')
                    break;

                secureString.AppendChar(c);
                offset += 2; // Move to the next character (2 bytes for Unicode)
            }

            // Make the SecureString read-only for enhanced security
            secureString.MakeReadOnly();
        }
        finally
        {
            // Zero out the unmanaged string in memory
            Marshal.ZeroFreeBSTR(unmanagedString);
        }

        return secureString;
    }

    /// <summary>
    ///     Converts an unmanaged string represented by a pointer to a char array.
    /// </summary>
    /// <remarks>
    ///     This method calculates the length of the unmanaged string and converts it to a char array.
    ///     The resulting char array contains the characters from the unmanaged string, and the unmanaged
    ///     string is zeroed out in memory for enhanced security.
    /// </remarks>
    /// <param name="unmanagedString">Pointer to an unmanaged string to be converted to a char array.</param>
    /// <returns>A char array containing the characters from the unmanaged string.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the input unmanaged string pointer is null.</exception>
    public static char[] ConvertUnmanagedStringToCharArray(IntPtr unmanagedString)
    {
        if (unmanagedString == IntPtr.Zero)
            throw new ArgumentNullException(nameof(unmanagedString), "Pointer to unmanaged string is null.");

        try
        {
            // Calculate the length of the unmanaged string
            var length = 0;
            while (Marshal.ReadByte(unmanagedString, length * 2) != 0 ||
                   Marshal.ReadByte(unmanagedString, length * 2 + 1) != 0) length++;

            // Convert the unmanaged string to a char array
            var charArray = new char[length];
            Marshal.Copy(unmanagedString, charArray, 0, length);

            return charArray;
        }
        finally
        {
            // Zero out the unmanaged string in memory
            Marshal.ZeroFreeBSTR(unmanagedString);
        }
    }

    /// <summary>
    ///     Converts a <see cref="SecureString" /> to a character array.
    /// </summary>
    /// <param name="secureString">The SecureString to convert.</param>
    /// <returns>A character array containing the characters from the SecureString.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="secureString" /> is null.</exception>
    public static char[] ConvertSecureStringToCharArray(SecureString secureString)
    {
        // Check if secureString is null.
        if (secureString.Length == 0)
            throw new ArgumentException("SecureString was empty.", nameof(secureString));

        var charArray = new char[secureString.Length];
        var unmanagedString = IntPtr.Zero;

        try
        {
            // Convert SecureString to an unmanaged Unicode string.
            unmanagedString = Marshal.SecureStringToBSTR(secureString);

            // Copy characters from the unmanaged string to the charArray.
            for (var i = 0; i < secureString.Length; i++)
                charArray[i] = (char)Marshal.ReadInt16(unmanagedString, i * 2);
        }
        finally
        {
            // Zero-free the allocated unmanaged memory.
            if (unmanagedString != IntPtr.Zero) Marshal.ZeroFreeBSTR(unmanagedString);
        }

        return charArray;
    }

    /// <summary>
    ///     Compresses a byte array using the GZip compression algorithm.
    /// </summary>
    /// <remarks>
    ///     This method takes a byte array as input, compresses it using the GZip compression algorithm,
    ///     and returns the compressed byte array. The compression level used is <see cref="CompressionLevel.SmallestSize" />.
    /// </remarks>
    /// <param name="inputText">The byte array representing the uncompressed text to be compressed.</param>
    /// <returns>A compressed byte array using the GZip compression algorithm.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the input byte array is null.</exception>
    public static async Task<byte[]> CompressText(byte[] inputText)
    {
        if (inputText == null)
            throw new ArgumentNullException(nameof(inputText), "Input byte array cannot be null.");

        using var outputStream = new MemoryStream();

        // Use 'using' statement with 'await' for asynchronous writing
        await using (var zipStream = new GZipStream(outputStream, CompressionLevel.SmallestSize))
        {
            await zipStream.WriteAsync(inputText);
        }

        return outputStream.ToArray();
    }

    /// <summary>
    ///     Decompresses a byte array that was compressed using the GZip compression algorithm.
    /// </summary>
    /// <remarks>
    ///     This method takes a compressed byte array as input, decompresses it using the GZip compression algorithm,
    ///     and returns the decompressed byte array.
    /// </remarks>
    /// <param name="inputText">The byte array representing the compressed text to be decompressed.</param>
    /// <returns>A decompressed byte array using the GZip compression algorithm.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the input byte array is null.</exception>
    public static async Task<byte[]> DecompressText(byte[] inputText)
    {
        if (inputText == null)
            throw new ArgumentNullException(nameof(inputText), "Input byte array cannot be null.");

        using var inputStream = new MemoryStream(inputText);
        await using var zipStream = new GZipStream(inputStream, CompressionMode.Decompress);
        using var outputStream = new MemoryStream();

        // Use 'await' for asynchronous copying
        await zipStream.CopyToAsync(outputStream);

        return outputStream.ToArray();
    }

    /// <summary>
    ///     Initializes multiple byte arrays from a source byte array for cryptographic operations.
    /// </summary>
    /// <remarks>
    ///     This method takes a source byte array and extracts key components for encryption and HMAC operations.
    ///     It initializes multiple byte arrays for different cryptographic purposes and returns them as a tuple.
    /// </remarks>
    /// <param name="src">The source byte array used for initializing cryptographic buffers.</param>
    /// <returns>
    ///     A tuple containing byte arrays for encryption keys (key, key2, key3, key4, key5)
    ///     and HMAC keys (hMacKey, hMackey2, hMacKey3).
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if the source byte array is null.</exception>
    public static (byte[] key, byte[] key2, byte[] key3, byte[] key4, byte[] key5, byte[] hMacKey, byte[] hMackey2, byte
        [] hMacKey3)
        InitBuffers(byte[] src)
    {
        if (src == null)
            throw new ArgumentNullException(nameof(src), "Source byte array cannot be null.");

        // Initialize byte arrays for encryption and HMAC keys
        var key = new byte[CryptoConstants.KeySize];
        var key2 = new byte[CryptoConstants.ThreeFish];
        var key3 = new byte[CryptoConstants.KeySize];
        var key4 = new byte[CryptoConstants.KeySize];
        var key5 = new byte[CryptoConstants.ShuffleKey];
        var hMacKey = new byte[CryptoConstants.HmacLength];
        var hMackey2 = new byte[CryptoConstants.HmacLength];
        var hMacKey3 = new byte[CryptoConstants.HmacLength];

        // Copy bytes from the source array to initialized key arrays
        CopyBytes(src, key, key2, key3, key4, key5, hMacKey, hMackey2, hMacKey3);

        // Return the initialized key arrays as a tuple
        return (key, key2, key3, key4, key5, hMacKey, hMackey2, hMacKey3);
    }

    /// <summary>
    ///     Copies bytes from a source byte array to multiple destination byte arrays.
    /// </summary>
    /// <remarks>
    ///     This method copies bytes from a source byte array to multiple destination byte arrays.
    ///     It uses Buffer.BlockCopy for efficient copying and advances the offset for each destination array.
    /// </remarks>
    /// <param name="src">The source byte array from which bytes are copied.</param>
    /// <param name="dest">The destination byte arrays where bytes are copied to.</param>
    /// <exception cref="ArgumentNullException">Thrown if the source byte array or any destination byte array is null.</exception>
    /// <exception cref="ArgumentException">
    ///     Thrown if the total length of destination arrays exceeds the length of the source
    ///     array.
    /// </exception>
    public static void CopyBytes(byte[] src, params byte[][] dest)
    {
        if (src == null)
            throw new ArgumentNullException(nameof(src), "Source byte array cannot be null.");

        foreach (var dst in dest)
        {
            if (dst == null)
                throw new ArgumentNullException(nameof(dest), "Destination byte array cannot be null.");

            if (src.Length < dst.Length)
                throw new ArgumentException(
                    "Length of the destination array cannot exceed the length of the source array.");

            Buffer.BlockCopy(src, 0, dst, 0, dst.Length);
        }
    }

    /// <summary>
    ///     Encrypts the contents of a file using Argon2 key derivation and XChaCha20-Poly1305 encryption.
    /// </summary>
    /// <param name="userName">The username associated with the user's salt for key derivation.</param>
    /// <param name="passWord">The user's password used for key derivation.</param>
    /// <param name="file">The path to the file whose content will be encrypted.</param>
    /// <returns>
    ///     A Task that completes with the encrypted content of the specified file.
    ///     If any error occurs during the process, returns an empty byte array.
    /// </returns>
    /// <remarks>
    ///     This method performs the following steps:
    ///     1. Validates input parameters to ensure they are not null or empty.
    ///     2. Retrieves the user-specific salt for key derivation.
    ///     3. Derives an encryption key from the user's password and the obtained salt using Argon2id.
    ///     4. Extracts key components for encryption, including two keys and an HMAC key.
    ///     5. Reads and encodes the content of the specified file.
    ///     6. Encrypts the file content using XChaCha20-Poly1305 encryption.
    ///     7. Clears sensitive information, such as the user's password, from memory.
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
        var bytes = await Argon2Id(passWord, salt, 544);
        if (bytes == Array.Empty<byte>())
            throw new Exception("Value was empty.");

        // Read content of the specified file
        var fileStr = await File.ReadAllTextAsync(file);
        var fileBytes = DataConversionHelpers.StringToByteArray(fileStr);

        // Check if file content or salt is empty
        if (fileBytes == null || fileBytes.Length == 0 || salt == null || salt.Length == 0)
            throw new ArgumentException("Value was empty.",
                fileBytes == null || fileBytes.Length == 0 ? nameof(fileBytes) : nameof(salt));

        var (key, key2, key3, key4, key5, hMacKey, hMacKey2, hMacKey3) = InitBuffers(bytes);

        var compressedText = await CompressText(fileBytes);

        // Encrypt the file content
        var encryptedFile =
            await EncryptAsyncV3(compressedText, key, key2, key3, key4, key5, hMacKey, hMacKey2, hMacKey3);

        // Clear sensitive information
        ClearBytes(key, key2, key3, key4, key5, hMacKey, hMacKey2, hMacKey3, bytes);
       // ClearChars(passWord);
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
        var bytes = await Argon2Id(passWord, salt, 544);

        var (key, key2, key3, key4, key5, hMacKey, hMacKey2, hMacKey3) = InitBuffers(bytes);

        // Read and decode the content of the encrypted file
        var fileStr = await File.ReadAllTextAsync(file);
        var fileBytes = DataConversionHelpers.Base64StringToByteArray(fileStr);

        // Check if file content or salt is empty
        if (fileBytes == Array.Empty<byte>() || salt == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.",
                fileBytes == Array.Empty<byte>() ? nameof(fileBytes) : nameof(salt));

        // Decrypt the file content
        var decryptedFile = await DecryptAsyncV3(fileBytes, key, key2, key3, key4, key5, hMacKey, hMacKey2, hMacKey3);

        var decompressedText = await DecompressText(decryptedFile);

        // Clear sensitive information
        ClearBytes(key, key2, key3, key4, key5, hMacKey, hMacKey2, hMacKey3, bytes);
       // ClearChars(passWord);
        return decompressedText;
    }

    /// <summary>
    ///     Generates an array of random indices for shuffling based on a given size and key.
    /// </summary>
    /// <param name="size">The size of the array for which shuffle exchanges are generated.</param>
    /// <param name="key">The key used for generating random indices.</param>
    /// <returns>An array of random indices for shuffling.</returns>
    /// <remarks>
    ///     The method uses a random number generator with the specified key to generate
    ///     unique indices for shuffling a byte array of the given size.
    /// </remarks>
    public static int[] GetShuffleExchanges(int size, byte[] key)
    {
        // Create an array to store shuffle exchanges
        var exchanges = new int[size - 1];

        // Initialize a random number generator with the specified key
        var rand = new Random(BitConverter.ToInt32(key));

        // Generate random indices for shuffling
        for (var i = size - 1; i > 0; i--)
        {
            var n = rand.Next(i + 1);
            exchanges[size - 1 - i] = n;
        }

        // Return the array of shuffle exchanges
        return exchanges;
    }

    /// <summary>
    ///     Shuffles a byte array based on a given key using a custom exchange algorithm.
    /// </summary>
    /// <param name="input">The byte array to be shuffled.</param>
    /// <param name="key">The key used for shuffling.</param>
    /// <returns>The shuffled byte array.</returns>
    /// <remarks>
    ///     The shuffling is performed using a custom exchange algorithm based on the specified key.
    /// </remarks>
    public static byte[] Shuffle(byte[] input, byte[] key)
    {
        // Get the size of the input array
        var size = input.Length;

        // Get the shuffle exchanges based on the size and key
        var exchanges = GetShuffleExchanges(size, key);

        // Perform shuffling using the custom exchange algorithm
        for (var i = size - 1; i > 0; i--)
        {
            var n = exchanges[size - 1 - i];
            (input[i], input[n]) = (input[n], input[i]);
        }

        // Return the shuffled byte array
        return input;
    }

    /// <summary>
    ///     De-shuffles a byte array based on a given key using a custom exchange algorithm.
    /// </summary>
    /// <param name="input">The byte array to be de-shuffled.</param>
    /// <param name="key">The key used for de-shuffling.</param>
    /// <returns>The de-shuffled byte array.</returns>
    /// <remarks>
    ///     The de-shuffling is performed using a custom exchange algorithm based on the specified key.
    /// </remarks>
    public static byte[] DeShuffle(byte[] input, byte[] key)
    {
        // Get the size of the input array
        var size = input.Length;

        // Get the shuffle exchanges based on the size and key
        var exchanges = GetShuffleExchanges(size, key);

        // Perform de-shuffling using the custom exchange algorithm
        for (var i = 1; i < size; i++)
        {
            var n = exchanges[size - i - 1];
            (input[i], input[n]) = (input[n], input[i]);
        }

        // Return the de-shuffled byte array
        return input;
    }

    /// <summary>
    ///     Computes the Hash-based Message Authentication Code (HMAC) using the SHA3-512 algorithm.
    /// </summary>
    /// <param name="input">The byte array to be authenticated.</param>
    /// <param name="key">The key used for HMAC computation.</param>
    /// <returns>The HMAC-SHA3-512 authentication code as a byte array.</returns>
    public static byte[] Hmacsha3(byte[] input, byte[] key)
    {
        // Create SHA3-512 digest
        IDigest digest = new Sha3Digest(512);

        // Create HMAC object with the chosen digest
        var hMac = new HMac(digest);
        hMac.Init(new KeyParameter(key));

        // Update HMAC with the input bytes
        hMac.BlockUpdate(input, 0, input.Length);

        // Calculate the final HMAC value and store it in the result array
        var result = new byte[hMac.GetMacSize()];
        hMac.DoFinal(result, 0);

        // Return the computed HMAC-SHA3-512 authentication code
        return result;
    }

    public static byte[] Sha3Hash(byte[] input)
    {
        // Create SHA3-512 digest
        IDigest digest = new Sha3Digest(512);

        digest.BlockUpdate(input, 0, input.Length);
        var result = new byte[digest.GetDigestSize()];
        digest.DoFinal(result, 0);

        // Return the computed HMAC-SHA3-512 authentication code
        return result;
    }

#pragma warning disable

    private const int BlockBitSize = 128;
    private const int KeyBitSize = 256;

    /// <summary>
    ///     Asynchronously encrypts a byte array using the AES block cipher in Cipher Block Chaining (CBC) mode
    ///     with HMAC-SHA3 authentication.
    /// </summary>
    /// <param name="inputText">The byte array to be encrypted.</param>
    /// <param name="key">The key used for encryption.</param>
    /// <param name="iv">The initialization vector used in CBC mode.</param>
    /// <param name="hMacKey">The key used for HMAC-SHA3 authentication.</param>
    /// <returns>The encrypted and authenticated byte array.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided inputText, key, iv, or hMacKey is empty or null.</exception>
    public static async Task<byte[]> EncryptAsync(byte[] inputText, byte[] key, byte[] iv, byte[] hMacKey)
    {
        // Parameter checks
        if (inputText == Array.Empty<byte>())
            throw new ArgumentException("Value was empty or null.", nameof(inputText));
        if (key == Array.Empty<byte>())
            throw new ArgumentException("Value was empty or null.", nameof(key));
        if (iv == Array.Empty<byte>())
            throw new ArgumentException("Value was empty or null.", nameof(iv));
        if (hMacKey == Array.Empty<byte>())
            throw new ArgumentException("Value was empty or null.", nameof(hMacKey));

        // Create AES object
        using var aes = Aes.Create();
        aes.BlockSize = BlockBitSize;
        aes.KeySize = KeyBitSize;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        // Create byte array to store cipherText
        byte[] cipherText;

        // Create streams and copy cipherStream to memStream
        using (var encryptor = aes.CreateEncryptor(key, iv))
        using (var memStream = new MemoryStream())
        {
            await using (var cryptoStream =
                         new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
            {
                using (var cipherStream = new MemoryStream(inputText))
                {
                    await cipherStream.FlushAsync();
                    await cipherStream.CopyToAsync(cryptoStream, (int)cipherStream.Length);
                }

                await cryptoStream.FlushFinalBlockAsync();
            }

            cipherText = memStream.ToArray();
        }

        // Concatenate IV and cipherText for HMAC-SHA3 authentication
        var prependItems = new byte[cipherText.Length + iv.Length];
        Buffer.BlockCopy(iv, 0, prependItems, 0, iv.Length);
        Buffer.BlockCopy(cipherText, 0, prependItems, iv.Length, cipherText.Length);

        // Calculate HMAC-SHA3 tag and concatenate it with IV and cipherText
        var tag = Hmacsha3(prependItems, hMacKey);
        var authenticatedBuffer = new byte[prependItems.Length + tag.Length];
        Buffer.BlockCopy(prependItems, 0, authenticatedBuffer, 0, prependItems.Length);
        Buffer.BlockCopy(tag, 0, authenticatedBuffer, prependItems.Length, tag.Length);

        // Return the authenticated and encrypted byte array
        return authenticatedBuffer;
    }

    /// <summary>
    ///     Asynchronously decrypts a byte array that has been encrypted using the AES block cipher in Cipher Block Chaining
    ///     (CBC) mode
    ///     with HMAC-SHA3 authentication.
    /// </summary>
    /// <param name="inputText">The byte array to be decrypted.</param>
    /// <param name="key">The key used for decryption.</param>
    /// <param name="hMacKey">The key used for HMAC-SHA3 authentication.</param>
    /// <returns>The decrypted byte array.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided inputText, key, or hMacKey is empty or null.</exception>
    /// <exception cref="CryptographicException">Thrown when the authentication tag does not match.</exception>
    public static async Task<byte[]> DecryptAsync(byte[] inputText, byte[] key, byte[] hMacKey)
    {
        // Parameter checks
        if (inputText == Array.Empty<byte>())
            throw new ArgumentException("Value was empty or null.", nameof(inputText));
        if (key == Array.Empty<byte>())
            throw new ArgumentException("Value was empty or null.", nameof(key));
        if (hMacKey == Array.Empty<byte>())
            throw new ArgumentException("Value was empty or null.", nameof(hMacKey));

        // Create AES object
        using var aes = Aes.Create();
        aes.BlockSize = BlockBitSize;
        aes.KeySize = KeyBitSize;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        // Create byte array for received HMAC tag
        var receivedHash = new byte[CryptoConstants.HmacLength];

        // Extract the received HMAC tag from the inputText
        Buffer.BlockCopy(inputText, inputText.Length - CryptoConstants.HmacLength, receivedHash, 0,
            CryptoConstants.HmacLength);

        // Create byte array for IV and cipherText
        var cipherWithIv = new byte[inputText.Length - CryptoConstants.HmacLength];

        // Extract cipherText and IV from the inputText
        Buffer.BlockCopy(inputText, 0, cipherWithIv, 0, inputText.Length - CryptoConstants.HmacLength);

        // Calculate HMAC-SHA3 on cipherText and IV
        var hashedInput = Hmacsha3(cipherWithIv, hMacKey);

        // Compare received HMAC tag with calculated HMAC to ensure data integrity
        var isMatch = CryptographicOperations.FixedTimeEquals(receivedHash, hashedInput);

        // If the HMAC tags do not match, raise an exception
        if (!isMatch)
            throw new CryptographicException("Authentication tag does not match.");

        // Extract IV and cipherText from the inputText
        var iv = new byte[CryptoConstants.Iv];
        var cipherResult = new byte[inputText.Length - CryptoConstants.Iv - CryptoConstants.HmacLength];

        Buffer.BlockCopy(inputText, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(inputText, iv.Length, cipherResult, 0, cipherResult.Length);

        byte[] decryptedText;

        // Create AES decryptor and decrypt the cipherText
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

        // Return memStream as byte array
        return memStream.ToArray();
    }
#pragma warning restore

    /// <summary>
    ///     Encrypts a byte array using the ThreeFish block cipher in Cipher Block Chaining (CBC) mode with HMAC-SHA3
    ///     authentication.
    /// </summary>
    /// <param name="inputText">The byte array to be encrypted.</param>
    /// <param name="key">The key used for encryption.</param>
    /// <param name="iv">The initialization vector used in CBC mode.</param>
    /// <param name="hMacKey">The key used for HMAC-SHA3 authentication.</param>
    /// <returns>The encrypted and authenticated byte array.</returns>
    private static byte[] EncryptThreeFish(byte[] inputText, byte[] key, byte[] iv, byte[] hMacKey)
    {
        // Initialize ThreeFish block cipher with CBC mode and PKCS7 padding
        IBlockCipher threeFish = new ThreefishEngine(1024);
        IBlockCipherMode cipherMode = new CbcBlockCipher(threeFish);
        IBlockCipherPadding padding = new Pkcs7Padding();

        // Create a buffered block cipher with the chosen mode and padding
        var cbcCipher = new PaddedBufferedBlockCipher(cipherMode, padding);

        // Initialize the cipher with the key and IV
        var keyParam = new KeyParameter(key);
        cbcCipher.Init(true, new ParametersWithIV(keyParam, iv));

        // Process the input text and obtain the final cipher text
        var cipherText = new byte[cbcCipher.GetOutputSize(inputText.Length)];
        var processLength = cbcCipher.ProcessBytes(inputText, 0, inputText.Length, cipherText, 0);
        var finalLength = cbcCipher.DoFinal(cipherText, processLength);
        var finalCipherText = new byte[finalLength + processLength];
        Buffer.BlockCopy(cipherText, 0, finalCipherText, 0, finalCipherText.Length);

        // Concatenate IV and cipherText for HMAC-SHA3 authentication
        var prependItems = new byte[finalCipherText.Length + iv.Length];
        Buffer.BlockCopy(iv, 0, prependItems, 0, iv.Length);
        Buffer.BlockCopy(finalCipherText, 0, prependItems, iv.Length, finalCipherText.Length);

        // Calculate HMAC-SHA3 tag and concatenate it with IV and cipherText
        var tag = Hmacsha3(prependItems, hMacKey);
        var authenticatedBuffer = new byte[prependItems.Length + tag.Length];
        Buffer.BlockCopy(prependItems, 0, authenticatedBuffer, 0, prependItems.Length);
        Buffer.BlockCopy(tag, 0, authenticatedBuffer, prependItems.Length, tag.Length);

        // Return the authenticated and encrypted byte array
        return authenticatedBuffer;
    }

    /// <summary>
    ///     Decrypts a byte array that has been encrypted using the ThreeFish block cipher in Cipher Block Chaining (CBC) mode
    ///     with HMAC-SHA3 authentication.
    /// </summary>
    /// <param name="inputText">The byte array to be decrypted.</param>
    /// <param name="key">The key used for decryption.</param>
    /// <param name="hMacKey">The key used for HMAC-SHA3 authentication.</param>
    /// <returns>The decrypted byte array.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided inputText, key, or hMacKey is empty or null.</exception>
    /// <exception cref="CryptographicException">Thrown when the authentication tag does not match.</exception>
    public static byte[] DecryptThreeFish(byte[] inputText, byte[] key, byte[] hMacKey)
    {
        // Parameter checks
        if (inputText == Array.Empty<byte>())
            throw new ArgumentException("Value was empty or null.", nameof(inputText));
        if (key == Array.Empty<byte>())
            throw new ArgumentException("Value was empty or null.", nameof(key));
        if (hMacKey == Array.Empty<byte>())
            throw new ArgumentException("Value was empty or null.", nameof(hMacKey));

        // Create byte array for received HMAC tag
        var receivedHash = new byte[CryptoConstants.HmacLength];

        // Extract the received HMAC tag from the inputText
        Buffer.BlockCopy(inputText, inputText.Length - CryptoConstants.HmacLength, receivedHash, 0,
            CryptoConstants.HmacLength);

        // Create byte array for IV and cipherText
        var cipherWithIv = new byte[inputText.Length - CryptoConstants.HmacLength];

        // Extract cipherText and IV from the inputText
        Buffer.BlockCopy(inputText, 0, cipherWithIv, 0, inputText.Length - CryptoConstants.HmacLength);

        // Calculate HMAC-SHA3 on cipherText and IV
        var hashedInput = Hmacsha3(cipherWithIv, hMacKey);

        // Compare received HMAC tag with calculated HMAC to ensure data integrity
        var isMatch = CryptographicOperations.FixedTimeEquals(receivedHash, hashedInput);

        // If the HMAC tags do not match, raise an exception
        if (!isMatch)
            throw new CryptographicException("Authentication tag does not match.");

        // Extract IV and cipherText from the inputText
        var iv = new byte[CryptoConstants.ThreeFish];
        var cipherResult = new byte[inputText.Length - CryptoConstants.ThreeFish - CryptoConstants.HmacLength];

        Buffer.BlockCopy(inputText, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(inputText, iv.Length, cipherResult, 0, cipherResult.Length);

        // Initialize ThreeFish block cipher with CBC mode and PKCS7 padding
        IBlockCipher threeFish = new ThreefishEngine(1024);
        IBlockCipherMode cipherMode = new CbcBlockCipher(threeFish);
        IBlockCipherPadding padding = new Pkcs7Padding();

        // Create a buffered block cipher with the chosen mode and padding
        var cbcCipher = new PaddedBufferedBlockCipher(cipherMode, padding);

        // Initialize the cipher with the key and IV
        var keyParam = new KeyParameter(key);
        cbcCipher.Init(false, new ParametersWithIV(keyParam, iv));

        // Decrypt the cipherText to obtain the original plainText
        var plainText = new byte[cbcCipher.GetOutputSize(cipherResult.Length)];
        var processLength = cbcCipher.ProcessBytes(cipherResult, 0, cipherResult.Length, plainText, 0);
        var finalLength = cbcCipher.DoFinal(plainText, processLength);
        var finalPlainText = new byte[finalLength + processLength];
        Buffer.BlockCopy(plainText, 0, finalPlainText, 0, finalPlainText.Length);

        // Return the final decrypted plainText
        return finalPlainText;
    }

    /// <summary>
    ///     Encrypts a byte array using the Serpent block cipher in Cipher Block Chaining (CBC) mode with HMAC-SHA3
    ///     authentication.
    /// </summary>
    /// <param name="inputText">The byte array to be encrypted.</param>
    /// <param name="key">The key used for encryption.</param>
    /// <param name="iv">The initialization vector used in CBC mode.</param>
    /// <param name="hMacKey">The key used for HMAC-SHA3 authentication.</param>
    /// <returns>The encrypted and authenticated byte array.</returns>
    private static byte[] EncryptSerpent(byte[] inputText, byte[] key, byte[] iv, byte[] hMacKey)
    {
        // Initialize Serpent block cipher with CBC mode and PKCS7 padding
        IBlockCipher serpent = new SerpentEngine();
        IBlockCipherMode cipherMode = new CbcBlockCipher(serpent);
        IBlockCipherPadding padding = new Pkcs7Padding();

        // Create a buffered block cipher with the chosen mode and padding
        var cbcCipher = new PaddedBufferedBlockCipher(cipherMode, padding);

        // Initialize the cipher with the key and IV
        var keyParam = new KeyParameter(key);
        cbcCipher.Init(true, new ParametersWithIV(keyParam, iv));

        // Process the input text and obtain the final cipher text
        var cipherText = new byte[cbcCipher.GetOutputSize(inputText.Length)];
        var processLength = cbcCipher.ProcessBytes(inputText, 0, inputText.Length, cipherText, 0);
        var finalLength = cbcCipher.DoFinal(cipherText, processLength);
        var finalCipherText = new byte[finalLength + processLength];
        Buffer.BlockCopy(cipherText, 0, finalCipherText, 0, finalCipherText.Length);

        // Concatenate IV and cipherText for HMAC-SHA3 authentication
        var prependItems = new byte[finalCipherText.Length + iv.Length];
        Buffer.BlockCopy(iv, 0, prependItems, 0, iv.Length);
        Buffer.BlockCopy(finalCipherText, 0, prependItems, iv.Length, finalCipherText.Length);

        // Calculate HMAC-SHA3 tag and concatenate it with IV and cipherText
        var tag = Hmacsha3(prependItems, hMacKey);
        var authenticatedBuffer = new byte[prependItems.Length + tag.Length];
        Buffer.BlockCopy(prependItems, 0, authenticatedBuffer, 0, prependItems.Length);
        Buffer.BlockCopy(tag, 0, authenticatedBuffer, prependItems.Length, tag.Length);

        // Return the authenticated and encrypted byte array
        return authenticatedBuffer;
    }

    /// <summary>
    ///     Decrypts a byte array that has been encrypted using the Serpent block cipher in Cipher Block Chaining (CBC) mode
    ///     with HMAC-SHA3 authentication.
    /// </summary>
    /// <param name="inputText">The byte array to be decrypted.</param>
    /// <param name="key">The key used for decryption.</param>
    /// <param name="hMacKey">The key used for HMAC-SHA3 authentication.</param>
    /// <returns>The decrypted byte array.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided inputText, key, or hMacKey is empty or null.</exception>
    /// <exception cref="CryptographicException">Thrown when the authentication tag does not match.</exception>
    public static byte[] DecryptSerpent(byte[] inputText, byte[] key, byte[] hMacKey)
    {
        // Parameter checks
        if (inputText == Array.Empty<byte>())
            throw new ArgumentException("Value was empty or null.", nameof(inputText));
        if (key == Array.Empty<byte>())
            throw new ArgumentException("Value was empty or null.", nameof(key));
        if (hMacKey == Array.Empty<byte>())
            throw new ArgumentException("Value was empty or null.", nameof(hMacKey));

        // Create byte array for received HMAC tag
        var receivedHash = new byte[CryptoConstants.HmacLength];

        // Extract the received HMAC tag from the inputText
        Buffer.BlockCopy(inputText, inputText.Length - CryptoConstants.HmacLength, receivedHash, 0,
            CryptoConstants.HmacLength);

        // Create byte array for IV and cipherText
        var cipherWithIv = new byte[inputText.Length - CryptoConstants.HmacLength];

        // Extract cipherText and IV from the inputText
        Buffer.BlockCopy(inputText, 0, cipherWithIv, 0, inputText.Length - CryptoConstants.HmacLength);

        // Calculate HMAC-SHA3 on cipherText and IV
        var hashedInput = Hmacsha3(cipherWithIv, hMacKey);

        // Compare received HMAC tag with calculated HMAC to ensure data integrity
        var isMatch = CryptographicOperations.FixedTimeEquals(receivedHash, hashedInput);

        // If the HMAC tags do not match, raise an exception
        if (!isMatch)
            throw new CryptographicException("Authentication tag does not match.");

        // Extract IV and cipherText from the inputText
        var iv = new byte[CryptoConstants.Iv];
        var cipherResult = new byte[inputText.Length - CryptoConstants.Iv - CryptoConstants.HmacLength];

        Buffer.BlockCopy(inputText, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(inputText, iv.Length, cipherResult, 0, cipherResult.Length);

        // Initialize Serpent block cipher with CBC mode and PKCS7 padding
        IBlockCipher serpent = new SerpentEngine();
        IBlockCipherMode cipherMode = new CbcBlockCipher(serpent);
        IBlockCipherPadding padding = new Pkcs7Padding();

        // Create a buffered block cipher with the chosen mode and padding
        var cbcCipher = new PaddedBufferedBlockCipher(cipherMode, padding);

        // Initialize the cipher with the key and IV
        var keyParam = new KeyParameter(key);
        cbcCipher.Init(false, new ParametersWithIV(keyParam, iv));

        // Decrypt the cipherText to obtain the original plainText
        var plainText = new byte[cbcCipher.GetOutputSize(cipherResult.Length)];
        var processLength = cbcCipher.ProcessBytes(cipherResult, 0, cipherResult.Length, plainText, 0);
        var finalLength = cbcCipher.DoFinal(plainText, processLength);
        var finalPlainText = new byte[finalLength + processLength];
        Buffer.BlockCopy(plainText, 0, finalPlainText, 0, finalPlainText.Length);

        // Return the final decrypted plainText
        return finalPlainText;
    }

    /// <summary>
    ///     Asynchronously encrypts a byte array using a multi-layer encryption approach.
    /// </summary>
    /// <param name="plaintext">The byte array to be encrypted.</param>
    /// <param name="key">The key used for the first layer of encryption (XChaCha20-Poly1305).</param>
    /// <param name="key2">The key used for the second layer of encryption (ThreeFish).</param>
    /// <param name="key3">The key used for the third layer of encryption.</param>
    /// <param name="key4">The key used for the fourth layer of encryption.</param>
    /// <param name="key5">The key used for shuffling the final encrypted result.</param>
    /// <param name="hMacKey">The key used for HMAC in the second layer of encryption.</param>
    /// <param name="hMacKey2">The key used for HMAC in the third layer of encryption.</param>
    /// <param name="hMacKey3">The key used for HMAC in the fourth layer of encryption.</param>
    /// <returns>A shuffled byte array containing nonces and the final encrypted result.</returns>
    /// <exception cref="ArgumentException">Thrown when any of the input parameters is an empty array.</exception>
    /// <exception cref="Exception">Thrown when any intermediate or final encrypted value is empty.</exception>
    public static async Task<byte[]> EncryptAsyncV3(byte[] plaintext,
        byte[] key, byte[] key2, byte[] key3, byte[] key4, byte[] key5, byte[] hMacKey, byte[] hMacKey2,
        byte[] hMacKey3)
    {
        // Parameter checks
        if (plaintext == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(plaintext));
        if (key == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(key));
        if (key2 == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(key2));
        if (key3 == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(key3));
        if (key4 == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(key4));
        if (key5 == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(key5));
        if (hMacKey == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(hMacKey));
        if (hMacKey2 == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(hMacKey2));
        if (hMacKey3 == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(hMacKey3));

        // Generate nonces for each encryption layer
        var nonce = RndByteSized(CryptoConstants.ChaChaNonceSize);
        var nonce2 = RndByteSized(CryptoConstants.ThreeFish);
        var nonce3 = RndByteSized(CryptoConstants.Iv);
        var nonce4 = RndByteSized(CryptoConstants.Iv);

        // Encrypt using XChaCha20-Poly1305
        var cipherText = SecretAeadXChaCha20Poly1305.Encrypt(plaintext, nonce, key) ??
                         throw new Exception("Value was empty.");

        // Encrypt using ThreeFish and HMAC
        var cipherTextL2 = EncryptThreeFish(cipherText, key2, nonce2, hMacKey) ??
                           throw new Exception("Value was empty.");

        // Encrypt using Serpent and HMAC
        var cipherTextL3 = EncryptSerpent(cipherTextL2, key3, nonce3, hMacKey2) ??
                           throw new Exception("Value was empty.");

        // Encrypt using AES and HMAC
        var cipherTextL4 = await EncryptAsync(cipherTextL3, key4, nonce4, hMacKey3) ??
                           throw new Exception("Value was empty.");

        // Concatenate nonces and the fourth level ciphertext
        var result = nonce.Concat(nonce2).Concat(nonce3).Concat(nonce4).Concat(cipherTextL4).ToArray();

        // Shuffle the result based on a key
        var shuffledResult = Shuffle(result, key5);

        // Return the shuffled and encrypted result
        return shuffledResult;
    }

    /// <summary>
    ///     Asynchronously decrypts a byte array that has been encrypted using a multi-layer encryption approach.
    /// </summary>
    /// <param name="cipherText">The byte array to be decrypted.</param>
    /// <param name="key">The key used for the first layer of decryption (XChaCha20-Poly1305).</param>
    /// <param name="key2">The key used for the second layer of decryption (ThreeFish).</param>
    /// <param name="key3">The key used for the third layer of decryption.</param>
    /// <param name="key4">The key used for the fourth layer of encryption.</param>
    /// <param name="key5">The key used for unshuffling the ciphertext.</param>
    /// <param name="hMacKey">The key used for HMAC in the second layer of decryption.</param>
    /// <param name="hMacKey2">The key used for HMAC in the third layer of decryption.</param>
    /// <param name="hMacKey3">The key used for HMAC in the fourth layer of encryption.</param>
    /// <returns>The decrypted byte array.</returns>
    /// <exception cref="ArgumentException">Thrown when any of the input parameters is an empty array.</exception>
    /// <exception cref="Exception">Thrown when any intermediate or final decrypted value is empty.</exception>
    public static async Task<byte[]> DecryptAsyncV3(byte[] cipherText,
        byte[] key, byte[] key2, byte[] key3, byte[] key4, byte[] key5, byte[] hMacKey, byte[] hMacKey2,
        byte[] hMacKey3)
    {
        // Parameter checks
        if (cipherText == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(cipherText));
        if (key == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(key));
        if (key2 == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(key2));
        if (key3 == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(key3));
        if (key4 == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(key4));
        if (key5 == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(key5));
        if (hMacKey == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(hMacKey));
        if (hMacKey2 == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(hMacKey2));
        if (hMacKey3 == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(hMacKey3));

        // Unshuffle the input 
        var unshuffledResult = DeShuffle(cipherText, key5);

        // Extract nonces and ciphertext from the unshuffled result
        var nonce = new byte[CryptoConstants.ChaChaNonceSize];
        Buffer.BlockCopy(unshuffledResult, 0, nonce, 0, nonce.Length);

        var nonce2 = new byte[CryptoConstants.ThreeFish];
        Buffer.BlockCopy(unshuffledResult, nonce.Length, nonce2, 0, nonce2.Length);

        var nonce3 = new byte[CryptoConstants.Iv];
        Buffer.BlockCopy(unshuffledResult, nonce.Length + nonce2.Length, nonce3, 0, nonce3.Length);

        var nonce4 = new byte[CryptoConstants.Iv];
        Buffer.BlockCopy(unshuffledResult, nonce.Length + nonce2.Length + nonce3.Length, nonce4, 0, nonce4.Length);

        var cipherResult =
            new byte[unshuffledResult.Length - nonce4.Length - nonce3.Length - nonce2.Length - nonce.Length];

        Buffer.BlockCopy(unshuffledResult, nonce.Length + nonce2.Length + nonce3.Length + nonce4.Length, cipherResult,
            0,
            cipherResult.Length);

        // Decrypt using AES and HMAC
        var resultL4 = await DecryptAsync(cipherResult, key4, hMacKey3) ??
                       throw new Exception("Value was empty.");

        // Decrypt using Serpent and HMAC
        var resultL3 = DecryptSerpent(resultL4, key3, hMacKey2) ??
                       throw new Exception("Value was empty.");

        // Decrypt using ThreeFish and HMAC
        var resultL2 = DecryptThreeFish(resultL3, key2, hMacKey) ??
                       throw new Exception("Value was empty.");

        // Decrypt using XChaCha20-Poly1305
        var result = SecretAeadXChaCha20Poly1305.Decrypt(resultL2, nonce, key) ??
                     throw new Exception("Value was empty.");

        // Return the final decrypted result
        return result;
    }

    public static async Task<byte[]> EncryptAsyncV3Debug(byte[] plaintext,
        byte[] nonce, byte[] nonce2, byte[] nonce3, byte[] key, byte[] key2, byte[] key3, byte[] key4, byte[] hMacKey,
        byte[] hMacKey2)
    {
        // Debug method allows us to set our own nonce
        // Parameter checks
        if (plaintext == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(plaintext));
        if (key == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(key));
        if (key2 == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(key2));
        if (key3 == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(key3));
        if (key4 == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(key4));
        if (hMacKey == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(hMacKey));
        if (hMacKey2 == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(hMacKey2));

        // Encrypt using XChaCha20-Poly1305
        var cipherText = SecretAeadXChaCha20Poly1305.Encrypt(plaintext, nonce, key) ??
                         throw new Exception("Value was empty.");

        // Encrypt using ThreeFish and HMAC
        var cipherTextL2 = EncryptThreeFish(cipherText, key2, nonce2, hMacKey) ??
                           throw new Exception("Value was empty.");

        // Encrypt using provided key and nonce, and await the result
        var cipherTextL3 = await EncryptAsync(cipherTextL2, key3, nonce3, hMacKey2) ??
                           throw new Exception("Value was empty.");

        // Concatenate nonces and the third level ciphertext
        var result = nonce.Concat(nonce2).Concat(nonce3).Concat(cipherTextL3).ToArray();

        // Shuffle the result based on a key
        var shuffleKey = key4;
        var shuffledResult = Shuffle(result, shuffleKey);

        // Return the shuffled and encrypted result
        return shuffledResult;
    }


    /// <summary>
    ///     Asynchronously decrypts a byte array that has been encrypted using a multi-layer encryption approach.
    /// </summary>
    /// <param name="cipherText">The byte array to be decrypted.</param>
    /// <param name="key">The key used for the first layer of decryption (XChaCha20-Poly1305).</param>
    /// <param name="key2">The key used for the second layer of decryption (ThreeFish).</param>
    /// <param name="key3">The key used for the third layer of decryption.</param>
    /// <param name="key4">The key used for unshuffling the input cipherText.</param>
    /// <param name="hMacKey">The key used for HMAC in the second layer of decryption.</param>
    /// <param name="hMacKey2">The key used for HMAC in the third layer of decryption.</param>
    /// <returns>The decrypted byte array.</returns>
    /// <exception cref="ArgumentException">Thrown when any of the input parameters is an empty array.</exception>
    /// <exception cref="Exception">Thrown when any intermediate or final decrypted value is empty.</exception>
    public static async Task<byte[]> DecryptAsyncV3Debug(byte[] cipherText,
        byte[] key, byte[] key2, byte[] key3, byte[] key4, byte[] hMacKey, byte[] hMacKey2)
    {
        // Parameter checks
        if (cipherText == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(cipherText));
        if (key == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(key));
        if (key2 == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(key2));
        if (key3 == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(key3));
        if (key4 == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(key4));
        if (hMacKey == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(hMacKey));
        if (hMacKey2 == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(hMacKey2));

        // Extract the shuffle key from the provided key4
        var shuffleKey = key4;

        // Unshuffle the input cipherText
        var unshuffledResult = DeShuffle(cipherText, shuffleKey);

        // Extract nonces and ciphertext from the unshuffled result
        var nonce = new byte[CryptoConstants.ChaChaNonceSize];
        Buffer.BlockCopy(unshuffledResult, 0, nonce, 0, nonce.Length);

        var nonce2 = new byte[CryptoConstants.ThreeFish];
        Buffer.BlockCopy(unshuffledResult, nonce.Length, nonce2, 0, nonce2.Length);

        var nonce3 = new byte[CryptoConstants.Iv];
        Buffer.BlockCopy(unshuffledResult, nonce.Length + nonce2.Length, nonce3, 0, nonce3.Length);

        var cipherResult =
            new byte[unshuffledResult.Length - nonce3.Length - nonce2.Length - nonce.Length];
        Buffer.BlockCopy(unshuffledResult, nonce.Length + nonce2.Length + nonce3.Length, cipherResult, 0,
            cipherResult.Length);

        // Decrypt at each layer in reverse order
        var resultL3 = await DecryptAsync(cipherResult, key3, hMacKey2) ?? throw new Exception("Value was empty.");
        var resultL2 = DecryptThreeFish(resultL3, key2, hMacKey) ?? throw new Exception("Value was empty.");
        var result = SecretAeadXChaCha20Poly1305.Decrypt(resultL2, nonce, key) ??
                     throw new Exception("Value was empty.");

        // Clear sensitive information to prevent accidental exposure
        ClearBytes(key, key2, key3, key4, hMacKey, hMacKey2);

        return result;
    }

    #region Unused

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