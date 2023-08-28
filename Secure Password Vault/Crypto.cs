using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace Secure_Password_Vault
{
    public static class Crypto
    {
        public static readonly int ByteSize = 24; // 48 bit hex string
        public static readonly int KeySize = 16; // 32 bit hex string
        private const int Iterations = 2;
        private const double MemorySize = 1024 * 1024 * 1; // 10GiB
        public const int SaltSize = 384 / 8; // 64 Bit
        public static readonly int IVBit = 128;
        public static byte[] Salt { get; set; } = Array.Empty<byte>();
        private static byte[] IV { get; set; } = Array.Empty<byte>();
        public static string Hash { get; set; } = string.Empty;
        public static string? checkSum { get; set; } = string.Empty;
        public static async Task<string?> HashAsync(string password, byte[] salt)
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt, // 64 bit
                DegreeOfParallelism = Environment.ProcessorCount, //Maximum cores
                Iterations = Iterations, // 50 Iterations
                MemorySize = (int)MemorySize //explicitly cast MemorySize from double to int                                 
            };
            try
            {
                string result = string.Empty;
                result = Convert.ToHexString(await argon2.GetBytesAsync(ByteSize).ConfigureAwait(false));
                return result;
            }
            catch (CryptographicException ex)
            {
                MessageBox.Show(ex.Message);
                ErrorLogging.ErrorLog(ex);
                return null;
            }
        }
        public static async Task<string?> HashPassword(string password, byte[] salt)
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt, // 64 bit
                DegreeOfParallelism = Environment.ProcessorCount, //Maximum cores
                Iterations = Iterations, // 50 Iterations
                MemorySize = (int)MemorySize //explicitly cast MemorySize from double to int                                 
            };
            try
            {
                string result = string.Empty;
                result = Convert.ToHexString(await argon2.GetBytesAsync(KeySize).ConfigureAwait(false));
                return result;
            }
            catch (CryptographicException ex)
            {
                MessageBox.Show(ex.Message);
                ErrorLogging.ErrorLog(ex);
                return null;
            }
        }
        public static async Task<string?> DeriveAsync(string password, byte[] salt)
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = Environment.ProcessorCount,
                Iterations = Iterations,
                MemorySize = (int)MemorySize
            };
            try
            {
                string result = string.Empty;
              
                result = Convert.ToHexString(await argon2.GetBytesAsync(KeySize).ConfigureAwait(false));
                return result;
            }
            catch (CryptographicException ex)
            {
                MessageBox.Show(ex.Message);
                ErrorLogging.ErrorLog(ex);
                return null;
            }
        }
        public static async Task<bool?> ComparePassword(string hash)
        {
            return await Task.FromResult(Hash != null && CryptographicOperations.FixedTimeEquals(Convert.FromHexString(Hash), Convert.FromHexString(hash))).ConfigureAwait(false);
        }
        public static string ComputeChecksum(string input)
        {
            byte[] hashValue = SHA512.HashData(Encoding.UTF8.GetBytes(input));
            string checksum = DataConversionHelpers.ByteArrayToHexString(hashValue) ?? string.Empty;
            return checksum;
        }
        public static async Task<byte[]?> EncryptUserFiles(string userName, string password)
        {
            string userFile = Authentication.GetUserFilePath(userName);
            if (userFile == null)
                return null;

            byte[] saltBuffer = new byte[SaltSize];
            byte[] IVBuffer = new byte[IVBit / 8];

            IV = RndByteSized(IVBit / 8);

            File.WriteAllText(Authentication.GetUserInfoPath(userName), DataConversionHelpers.ByteArrayToBase64String(Salt) + DataConversionHelpers.ByteArrayToBase64String(IV));
            string userInfo = File.ReadAllText(Authentication.GetUserInfoPath(userName));


            byte[]? infoBytes = DataConversionHelpers.Base64StringToByteArray(userInfo);
            if (infoBytes == null)
                return null;
            Buffer.BlockCopy(infoBytes, 0, saltBuffer, 0, saltBuffer.Length);
            Buffer.BlockCopy(infoBytes, saltBuffer.Length, IVBuffer, 0, IVBuffer.Length);
            Salt = saltBuffer;
            IV = IVBuffer;

            string textString = File.ReadAllText(userFile);
            byte[]? textBytes = DataConversionHelpers.StringToByteArray(textString);
            string? derivedKey = await DeriveAsync(password, Salt);
            if (derivedKey == null)
                throw new ArgumentException("Value returned null or empty.", nameof(derivedKey));
            byte[] keyBytes = Encoding.UTF8.GetBytes(derivedKey);
            byte[] encryptedBytes = Encrypt(textBytes, keyBytes);
            if (encryptedBytes == null)
                throw new ArgumentException("Value returned null or empty.", nameof(encryptedBytes));
            return encryptedBytes;
        }
        public static async Task<byte[]?> DecryptUserFiles(string userName, string password)
        {
            string userFile = Authentication.GetUserFilePath(userName);
            if (userFile == null)
                return null;

            byte[] saltBuffer = new byte[SaltSize];
            byte[] IVBuffer = new byte[IVBit / 8];

            string userInfo = File.ReadAllText(Authentication.GetUserInfoPath(userName));

            byte[]? infoBytes = DataConversionHelpers.Base64StringToByteArray(userInfo);
            if (infoBytes == null)
                return null;
            Buffer.BlockCopy(infoBytes, 0, saltBuffer, 0, saltBuffer.Length);
            Buffer.BlockCopy(infoBytes, saltBuffer.Length, IVBuffer, 0, IVBuffer.Length);
            Salt = saltBuffer;
            IV = IVBuffer;

            string textString = File.ReadAllText(userFile);
            byte[]? textBytes = DataConversionHelpers.Base64StringToByteArray(textString);
            string? derivedKey = await DeriveAsync(password, Salt);
            if (derivedKey == null)
                throw new ArgumentException("Value returned null or empty.", nameof(derivedKey));
            byte[] keyBytes = Encoding.UTF8.GetBytes(derivedKey);
            byte[] decryptedBytes = Decrypt(textBytes, keyBytes);
            if (decryptedBytes == null)
                throw new ArgumentException("Value returned null or empty.", nameof(decryptedBytes));
            return decryptedBytes;
        }
        private static readonly RandomNumberGenerator rndNum = RandomNumberGenerator.Create();

        private static int RndInt()
        {
            byte[] buffer = new byte[(sizeof(int))];
            rndNum.GetBytes(buffer);
            int result = BitConverter.ToInt32(buffer, 0);
            return result;
        }
        private static int BoundedInt(int min, int max)
        {
            var value = RndInt();
            int range = max - min;
            int result = min + (Math.Abs(value) % range);

            return result;
        }
        public static byte[] RndByteSized(int size)
        {
            var buffer = new byte[size];
            rndNum.GetBytes(buffer);
            return buffer;
        }
#pragma warning disable
        private const int BlockBitSize = 128;
        private const int KeyBitSize = 256;
   
        public static byte[]? Encrypt(byte[]? PlainText, byte[]? Key)
        {
            try
            {
                byte[] cipherText;
                using (Aes aes = Aes.Create())
                {
                    aes.BlockSize = BlockBitSize;
                    aes.KeySize = KeyBitSize;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var encryptor = aes.CreateEncryptor(Key, IV))
                    using (var memStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (var cipherStream = new MemoryStream(PlainText))
                            {
                                cipherStream.CopyTo(cryptoStream, (int)cipherStream.Length);
                                cipherStream.Flush();
                                cryptoStream.FlushFinalBlock();
                            }
                        }
                        cipherText = memStream.ToArray();
                    }
                }
                Key = null;
                byte[] prependItems = new byte[cipherText.Length + IV.Length];
                Buffer.BlockCopy(IV, 0, prependItems, 0, IV.Length);
                Buffer.BlockCopy(cipherText, 0, prependItems, IV.Length, cipherText.Length);
                return prependItems;
            }
            catch (Exception ex)
            {
                ErrorLogging.ErrorLog(ex);
                string error = ex.Message + " " + ex.InnerException;
                MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        public static byte[] Decrypt(byte[]? CipherText, byte[]? Key)
        {
            try
            {
                if (CipherText == null)
                {
                    throw new ArgumentException("Value was empty or null.", nameof(CipherText));
                }

                using (var aes = Aes.Create())
                {
                    aes.BlockSize = BlockBitSize;
                    aes.KeySize = KeyBitSize;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    
                    Buffer.BlockCopy(CipherText, 0, IV, 0, IV.Length);
                    var cipherResult = new byte[CipherText.Length - IV.Length];
                    Buffer.BlockCopy(CipherText, IV.Length, cipherResult, 0, cipherResult.Length);
                    
                    using (var decryptor = aes.CreateDecryptor(Key, IV))
                    using (var memStrm = new MemoryStream())
                    {
                        using (var decryptStream = new CryptoStream(memStrm, decryptor, CryptoStreamMode.Write))
                        {
                            using (var plainStream = new MemoryStream(cipherResult))
                            {
                                plainStream.CopyTo(decryptStream, (int)plainStream.Length);
                                plainStream.Flush();
                                decryptStream.FlushFinalBlock();
                            }
                        }
                        Key = null;
                        return memStrm.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Key = null;
                ErrorLogging.ErrorLog(ex);
                string error = ex.Message + " " + ex.InnerException;
                return null;
            }
#pragma warning restore
        }
    }
}
