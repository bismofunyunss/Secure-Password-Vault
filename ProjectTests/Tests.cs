using System.Text;
using System.Windows.Forms;

namespace ProjectTests;

[TestClass]
public class Tests
{
    /// <summary>
    /// This code snippet demonstrates the use of static readonly fields for password, plaintext, salt, and nonces,
    /// along with an expected result after encryption.
    /// </summary>
    public class EncryptionExample
    {
        public static readonly string passString = "Password1234567890!!!!";
        public static readonly string plainText = "Here's some text to encrypt.";

        // Salt for key derivation
        public static readonly byte[] Salt =
        [
            0x01, 0x23, 0x45, 0x67,
            0x89, 0xAB, 0xCD, 0xEF,
            0xFE, 0xDC, 0xBA, 0x98,
            0x76, 0x54, 0x32, 0x10,
            0x25, 0x36, 0x47, 0x58,
            0x69, 0x7A, 0x8B, 0x9C
        ];

        // Nonce for xChacha
        public static readonly byte[] Nonce =
        [
            0x12, 0x34, 0x56, 0x78,
            0x9A, 0xBC, 0xDE, 0xF0,
            0x11, 0x22, 0x33, 0x44,
            0x55, 0x66, 0x77, 0x88,
            0x99, 0xAA, 0xBB, 0xCC,
            0xDD, 0xEE, 0xFF, 0x97
        ];

        // Another nonce for AES IV
        public static readonly byte[] Nonce2 =
        [
            0x1C, 0x8F, 0x5A, 0x63,
            0x2E, 0xD7, 0x49, 0x0B,
            0xC2, 0x7F, 0x38, 0x96,
            0xE1, 0xA4, 0x72, 0x9D
        ];

        // The expected result after encryption
        public const string ExpectedResult =
            "EjRWeJq83vARIjNEVWZ3iJmqu8zd7v+XHI9aYy7XSQvCfziW4aRynRyPWmMu10kLwn84luGkcp2zYHJGIimTvMhsS1d0NuUNiU98xEnBZvHL3bUERDSlKFwwIxqw71869mnBt/tSO8WuZkSC+hhzLgLud9kSgI+yZ9rGfwLRqONsASGW1ol07VtLdhwo1/8gJRGR1P/hHoEklOWeJdYHtMDUhV/wgp4D";
    }


    [TestMethod]
    [Description("Generates a random byte array of a specified length.")]
    public void GenerateRndBytes()
    {
        byte[] testBytes = Crypto.RndByteSized(24);
        Assert.IsNotNull(testBytes);
    }

    /// <summary>
    /// This method demonstrates the decryption process using the Crypto class, including key derivation,
    /// encryption, and decryption of test data. It uses Argon2Id for key derivation, and two keys along with an HMAC key for encryption.
    /// </summary>

    [TestMethod]
    public async Task Decryption()
    {
        try
        {
            // Derive keys using Argon2Id
            var bytes = await Crypto.Argon2Id(EncryptionExample.passString.ToCharArray(), EncryptionExample.Salt, 320);

            // Initialize key arrays
            var key = new byte[32];
            var key2 = new byte[32];
            var key3 = new byte[128];
            var hMacKey = new byte[64];
            var hMacKey2 = new byte[64];

            // Copy bytes to key arrays
            Buffer.BlockCopy(bytes, 0, key, 0, key.Length);
            Buffer.BlockCopy(bytes, key.Length, key2, 0, key2.Length);
            Buffer.BlockCopy(bytes, key.Length + key2.Length, hMacKey, 0, hMacKey.Length);
            Buffer.BlockCopy(bytes, key.Length + key2.Length + hMacKey.Length, key3, 0, key3.Length);
            Buffer.BlockCopy(bytes, key.Length + key2.Length + key3.Length + hMacKey.Length, hMacKey2, 0, hMacKey2.Length);

            // Encrypt test data using EncryptAsyncV3Debug
            byte[] encryptedTest = await Crypto.EncryptAsyncV3Debug(
                DataConversionHelpers.StringToByteArray(EncryptionExample.plainText),
                EncryptionExample.Nonce, EncryptionExample.Nonce2, key, key2, hMacKey);

            // Convert encrypted result to Base64 string
            string encryptResult = DataConversionHelpers.ByteArrayToBase64String(encryptedTest);

            // Assert the encryption results
            Assert.IsNotNull(encryptedTest);
            Assert.AreEqual(EncryptionExample.ExpectedResult, encryptResult);

            // Derive keys again for decryption
            bytes = await Crypto.Argon2Id(EncryptionExample.passString.ToCharArray(), EncryptionExample.Salt, 320);

            // Reset key arrays
            key = new byte[32];
            key2 = new byte[32];
            key3 = new byte[128];
            hMacKey = new byte[64];
            hMacKey2 = new byte[64];
            
            // Copy bytes to key arrays
            Buffer.BlockCopy(bytes, 0, key, 0, key.Length);
            Buffer.BlockCopy(bytes, key.Length, key2, 0, key2.Length);
            Buffer.BlockCopy(bytes, key.Length + key2.Length, hMacKey, 0, hMacKey.Length);

            // Decrypt the test data
            byte[] decryptedTest = await Crypto.DecryptAsyncV3(
                DataConversionHelpers.Base64StringToByteArray(encryptResult), key, key2, key3, hMacKey, hMacKey2);

            // Convert decrypted result to string
            string decryptResult = DataConversionHelpers.ByteArrayToString(decryptedTest);

            // Assert the decryption results
            Assert.IsNotNull(decryptResult);
            Assert.AreEqual(EncryptionExample.plainText, decryptResult);
        }
        catch (Exception ex)
        {
            // Handle any exceptions during decryption and fail the test
            Assert.Fail($"Decryption test failed: {ex.Message}");
        }
    }


    /// <summary>
    /// This method tests the encryption process using the Crypto class, including key derivation, and encryption of test data.
    /// It uses Argon2Id for key derivation, and two keys along with an HMAC key for encryption.
    /// </summary>
    [TestMethod]
    public async Task Encryption()
    {
        try
        {
            // Derive keys using Argon2Id
            var bytes = await Crypto.Argon2Id(
                EncryptionExample.passString.ToCharArray(),
                EncryptionExample.Salt, 128);

            // Initialize key arrays
            var key = new byte[32];
            var key2 = new byte[32];
            var hMacKey = new byte[64];

            // Copy bytes to key arrays
            Buffer.BlockCopy(bytes, 0, key, 0, key.Length);
            Buffer.BlockCopy(bytes, key.Length, key2, 0, key2.Length);
            Buffer.BlockCopy(bytes, key.Length + key2.Length, hMacKey, 0, hMacKey.Length);

            // Encrypt test data using EncryptAsyncV3Debug
            byte[] encryptedTest = await Crypto.EncryptAsyncV3Debug(
                DataConversionHelpers.StringToByteArray(EncryptionExample.plainText),
                EncryptionExample.Nonce, EncryptionExample.Nonce2, key, key2, hMacKey);

            // Convert encrypted result to Base64 string
            string base64Result = DataConversionHelpers.ByteArrayToBase64String(encryptedTest);

            // Assert the encryption results
            Assert.IsNotNull(encryptedTest);
            Assert.AreEqual(EncryptionExample.ExpectedResult, base64Result);
        }
        catch (Exception ex)
        {
            // Handle any exceptions during encryption and fail the test
            Assert.Fail($"Encryption test failed: {ex.Message}");
        }
    }


    /// <summary>
    /// This method tests the password hashing functionality in the Crypto class using Argon2Id.
    /// It hashes a password represented as a character array with a specified salt and checks for a non-null result.
    /// </summary>
    [TestMethod]
    public async Task Hash()
    {
        try
        {
            // Convert the password string to a character array
            char[] passArray = EncryptionExample.passString.ToCharArray();

            // Hash the password using Argon2Id with a specified salt and a target hash length of 32 bytes
            byte[] result = await Crypto.Argon2Id(passArray, EncryptionExample.Salt, 32) ?? Array.Empty<byte>();

            // Assert that the result is not null
            Assert.IsNotNull(result);
        }
        catch (Exception ex)
        {
            // Handle any exceptions during password hashing and fail the test
            Assert.Fail($"Password hashing test failed: {ex.Message}");
        }
    }

    /// <summary>
    /// This method tests the password hash comparison functionality in the Crypto class.
    /// It compares two password hashes, demonstrating both a mismatched and a matched case.
    /// </summary>
    [TestMethod]
    public async Task CompareHash()
    {
        try
        {
            // Define two password hashes as hexadecimal strings
            string hash1 = "5e463665b62f4ec740145bd1a5062602bdec0c462063ff299303cc8d6f413193";
            string hash2 = "a60d2335fc1fbb39b933a2c5acf9f4f9c5ff1b292cf30d82e246107566efef12";

            // Convert hexadecimal strings to byte arrays
            byte[] hashBytes1 = DataConversionHelpers.StringToByteArray(hash1);
            byte[] hashBytes2 = DataConversionHelpers.StringToByteArray(hash2);

            // Assert that the ComparePassword method correctly identifies mismatched hashes
            Assert.IsFalse(await Crypto.ComparePassword(hashBytes1, hashBytes2));

            // Assert that the ComparePassword method correctly identifies matched hashes
            Assert.IsTrue(await Crypto.ComparePassword(hashBytes1, hashBytes1));
        }
        catch (Exception ex)
        {
            // Handle any exceptions during hash comparison and fail the test
            Assert.Fail($"Hash comparison test failed: {ex.Message}");
        }
    }
}