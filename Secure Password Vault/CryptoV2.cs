using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security;
using Konscious.Security.Cryptography;
using System.Text;

namespace Secure_Password_Vault;

public static class CryptoV2
{
    public static class Memory
    {
        [DllImport("MemoryManagement.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SecureMemoryClear(IntPtr ptr, IntPtr size);
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

    public static class Conversions
    {
        public static IntPtr CreatePinnedCharArray(string text)
        {
            // Convert the string to a char array
            char[] charArray = text.ToCharArray();

            // Allocate memory and pin the char array
            IntPtr pinnedArray = Marshal.AllocCoTaskMem(charArray.Length * sizeof(char));
            GCHandle handle = GCHandle.Alloc(charArray, GCHandleType.Pinned);

            // Copy the contents of the char array to the pinned memory
            Marshal.Copy(charArray, 0, pinnedArray, charArray.Length);

            // Release the handle to avoid potential memory leaks
            handle.Free();

            return pinnedArray;
        }

        public static IntPtr CreatePinnedCharArray(char[] array)
        {
            // Allocate memory and pin the char array
            IntPtr pinnedArray = Marshal.AllocCoTaskMem(array.Length * sizeof(char));
            GCHandle handle = GCHandle.Alloc(array, GCHandleType.Pinned);

            // Copy the contents of the char array to the pinned memory
            Marshal.Copy(array, 0, pinnedArray, array.Length);

            // Release the handle to avoid potential memory leaks
            handle.Free();

            return pinnedArray;
        }

        public static char[] ConvertIntPtrToCharArray(IntPtr pinnedArray, int length)
        {
            // Create a managed char array
            char[] managedCharArray = new char[length];

            // Copy the contents of the pinned memory to the managed char array
            Marshal.Copy(pinnedArray, managedCharArray, 0, length);

            return managedCharArray;
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

            CryptoUtils.ClearBytes(passwordBytes);
            return result;
        }
    }

    public static class CryptoUtils
    {
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
                    Memory.SecureMemoryClear(handle.AddrOfPinnedObject(), (IntPtr)array.Length * sizeof(byte));
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
                    Memory.SecureMemoryClear(handle.AddrOfPinnedObject(), (IntPtr)array.Length * sizeof(char));
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
                    Memory.SecureMemoryClear(handle.AddrOfPinnedObject(), (IntPtr)value.Length * 2);
                }
                finally
                {
                    // Release the pinned string
                    handle.Free();
                }
            }
        }
    }
}

