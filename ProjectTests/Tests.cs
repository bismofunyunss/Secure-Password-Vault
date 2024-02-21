using System.Text;
using Secure_Password_Vault;

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
        public static readonly string PassString = "Password1234567890!!!!!!!!!!!!";
        public static readonly string PlainText = "Here's some text to encrypt.";

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

        // Threefish nonce
        public static readonly byte[] Nonce3 = 
        [
            0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,
            0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10,
            0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88,
            0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF, 0x00,
            0x10, 0x32, 0x54, 0x76, 0x98, 0xBA, 0xDC, 0xFE,
            0xED, 0xCB, 0xA9, 0x87, 0x65, 0x43, 0x21, 0x00,
            0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10,
            0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88,
            0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF, 0x00,
            0x10, 0x32, 0x54, 0x76, 0x98, 0xBA, 0xDC, 0xFE,
            0xED, 0xCB, 0xA9, 0x87, 0x65, 0x43, 0x21, 0x00,
            0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10,
            0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88,
            0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF, 0x00,
            0x10, 0x32, 0x54, 0x76, 0x98, 0xBA, 0xDC, 0xFE,
            0xED, 0xCB, 0xA9, 0x87, 0x65, 0x43, 0x21, 0x00,
        ];

        // The expected result after encryption
        public const string ExpectedResult =
            "RgB2s/AiaJnM7uK15TMyHDOH8ERyes0cmZI/ROF2mlRU5lwvSRBmlkNlm8tZz8I1iQPM1cYYdoexu/9jj2NlxP5pd2IAM8uYWZiuYGQQZkrt1DApVfwhvyMc/yIGObRc6bqSOQ5BRdLbSTR0rRcACdoQ8PRjECflBP+dEcWGDsu8t/ekZHuXUUWkZ1pF95iAh9+iP5p8WO4SCJtvQV7T3Hc7BO/cEWSd19i3VUw0EBBDQx4m8Kd4tPL/mLbeAN1lNmr+qpndVphl3WyoSWaEumYtqbttdJQkuvRMdHI8NLqK81rY5/FNLzgjlPOZK1Z/IsQTcMSIv58zGIf/SI+kdnXvyozcFqHwmPzN09CWgSHeliTKh6pUSYjzQzIgVYG6HjiY0gBARbjMnqW843WL3P6AeeDMN3aq7mUFXyNc40MBiOEzjTD+hzyAa1hOw2NRlwg3EI93HBFcm1R3z8FDjZoyTwBgdjIsvUYqQFTCcomBj7o9VYjkqpDcuqpmehntwYCZdVB9LHfdfWMhcMSc4/45qU/hO47H//5yI049OGj+eDgE/jNFuyjLUiX2gO52Cy4LgokhKTHc2F0fUZKtELlJqywbGkR0x7hTu/uAgaZx1mafji5m1+59BFUvXREAp8v2OCnwZ3aWZnlIAMWqbKkAazv9e2WkZHDidpcpqFPnlM0VLnfkNOpiIMqPLQBuRNLWn/LMdz1URDKr/DuqbLWjwruv7boH0ZiZSrjdIFV/EKaH7TK6Tt7wK8NnuoJxUSLW4WoyqgU=";
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
            // Derive encryption key from password and salt

            var bytes = await Crypto.Argon2Id(EncryptionExample.PassString.ToCharArray(), EncryptionExample.Salt, 448);
            if (bytes == Array.Empty<byte>())
                throw new Exception("Value was empty.");

            // Extract key components for encryption
            var key = new byte[Crypto.CryptoConstants.KeySize];
            var key2 = new byte[Crypto.CryptoConstants.ThreeFish];
            var key3 = new byte[Crypto.CryptoConstants.KeySize];
            var key4 = new byte[Crypto.CryptoConstants.ShuffleKey];
            var hMacKey = new byte[Crypto.CryptoConstants.HmacLength];
            var hMackey2 = new byte[Crypto.CryptoConstants.HmacLength];

            Buffer.BlockCopy(bytes, 0, key, 0, key.Length);
            Buffer.BlockCopy(bytes, key.Length, key2, 0, key2.Length);
            Buffer.BlockCopy(bytes, key.Length + key2.Length, key3, 0, key3.Length);
            Buffer.BlockCopy(bytes, key.Length + key2.Length + key3.Length, key4, 0, key4.Length);
            Buffer.BlockCopy(bytes, key.Length + key2.Length + key3.Length + key4.Length, hMacKey, 0, hMacKey.Length);
            Buffer.BlockCopy(bytes, key.Length + key2.Length + key3.Length + key4.Length + hMacKey.Length, hMackey2, 0,
                hMackey2.Length);

            // Encrypt test data using EncryptAsyncV3Debug
            byte[] encryptedTest = await Crypto.EncryptAsyncV3Debug(
                DataConversionHelpers.StringToByteArray(EncryptionExample.PlainText),
                EncryptionExample.Nonce, EncryptionExample.Nonce3, EncryptionExample.Nonce2, key, key2, key3, key4, hMacKey, hMackey2);

            // Convert encrypted result to Base64 string
            string encryptResult = DataConversionHelpers.ByteArrayToBase64String(encryptedTest);

            // Assert the encryption results
            Assert.IsNotNull(encryptedTest);
            Assert.AreEqual(EncryptionExample.ExpectedResult, encryptResult);

            // Derive keys again for decryption
            // Derive encryption key from password and salt
            var passWordBytes = EncryptionExample.PassString.ToCharArray();
            bytes = await Crypto.Argon2Id(passWordBytes, EncryptionExample.Salt, 448);
            if (bytes == Array.Empty<byte>())
                throw new Exception("Value was empty.");

            // Extract key components for encryption
            key = new byte[Crypto.CryptoConstants.KeySize]; 
            key2 = new byte[Crypto.CryptoConstants.ThreeFish]; 
            key3 = new byte[Crypto.CryptoConstants.KeySize]; 
            key4 = new byte[Crypto.CryptoConstants.ShuffleKey]; 
            hMacKey = new byte[Crypto.CryptoConstants.HmacLength]; 
            hMackey2 = new byte[Crypto.CryptoConstants.HmacLength];

            Buffer.BlockCopy(bytes, 0, key, 0, key.Length);
            Buffer.BlockCopy(bytes, key.Length, key2, 0, key2.Length);
            Buffer.BlockCopy(bytes, key.Length + key2.Length, key3, 0, key3.Length);
            Buffer.BlockCopy(bytes, key.Length + key2.Length + key3.Length, key4, 0, key4.Length);
            Buffer.BlockCopy(bytes, key.Length + key2.Length + key3.Length + key4.Length, hMacKey, 0, hMacKey.Length);
            Buffer.BlockCopy(bytes, key.Length + key2.Length + key3.Length + key4.Length + hMacKey.Length, hMackey2, 0,
                hMackey2.Length);

            // Decrypt the test data
            byte[] decryptedTest = await Crypto.DecryptAsyncV3Debug(
                DataConversionHelpers.Base64StringToByteArray(encryptResult), key, key2, key3, key4, hMacKey, hMackey2);

            // Convert decrypted result to string
            string decryptResult = DataConversionHelpers.ByteArrayToString(decryptedTest);

            // Assert the decryption results
            Assert.IsNotNull(decryptResult);
            Assert.AreEqual(EncryptionExample.PlainText, decryptResult);
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
                EncryptionExample.PassString.ToCharArray(),
                EncryptionExample.Salt, 448);

            // Extract key components for encryption
            var key = new byte[Crypto.CryptoConstants.KeySize];
            var key2 = new byte[Crypto.CryptoConstants.ThreeFish];
            var key3 = new byte[Crypto.CryptoConstants.KeySize];
            var key4 = new byte[Crypto.CryptoConstants.ShuffleKey];
            var hMacKey = new byte[Crypto.CryptoConstants.HmacLength];
            var hMackey2 = new byte[Crypto.CryptoConstants.HmacLength];

            Buffer.BlockCopy(bytes, 0, key, 0, key.Length);
            Buffer.BlockCopy(bytes, key.Length, key2, 0, key2.Length);
            Buffer.BlockCopy(bytes, key.Length + key2.Length, key3, 0, key3.Length);
            Buffer.BlockCopy(bytes, key.Length + key2.Length + key3.Length, key4, 0, key4.Length);
            Buffer.BlockCopy(bytes, key.Length + key2.Length + key3.Length + key4.Length, hMacKey, 0, hMacKey.Length);
            Buffer.BlockCopy(bytes, key.Length + key2.Length + key3.Length + key4.Length + hMacKey.Length, hMackey2, 0,
                hMackey2.Length);

            // Encrypt test data using EncryptAsyncV3Debug
            byte[] encryptedTest = await Crypto.EncryptAsyncV3Debug(DataConversionHelpers.StringToByteArray(EncryptionExample.PlainText),
            EncryptionExample.Nonce, EncryptionExample.Nonce3, EncryptionExample.Nonce2, key, key2, key3, key4, hMacKey, hMackey2);

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
            var passArray = Encoding.UTF8.GetBytes(EncryptionExample.PassString);
            var passChars = Encoding.UTF8.GetChars(passArray);
            // Hash the password using Argon2Id with a specified salt and a target hash length of 32 bytes
            byte[] result = await Crypto.Argon2Id(passChars, EncryptionExample.Salt, 32) ?? Array.Empty<byte>();

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