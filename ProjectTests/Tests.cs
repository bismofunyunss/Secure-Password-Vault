using System.Text;

namespace ProjectTests;

[TestClass]
public class Tests
{
    private static readonly string passString = "Password1234567890!!!!";
    private static readonly string plainText = "Here's some text to encrypt.";
    private static readonly byte[] Salt = {
        0x01, 0x23, 0x45, 0x67,
        0x89, 0xAB, 0xCD, 0xEF,
        0xFE, 0xDC, 0xBA, 0x98,
        0x76, 0x54, 0x32, 0x10,
        0x25, 0x36, 0x47, 0x58,
        0x69, 0x7A, 0x8B, 0x9C
    };
    private static readonly byte[] Salt2 = {
        0x6c, 0x2a, 0x8f, 0xd9,
        0x0b, 0xc2, 0x73, 0x4e,
        0xb7, 0x19, 0x5a, 0x36,
        0x82, 0x15, 0xd1, 0x77,
        0x4b, 0x29, 0xa1, 0x8e,
        0x59, 0x6c, 0x35, 0xf4
    };
    private static readonly byte[] Salt3 = {
        0x98, 0x4a, 0x2e, 0x7f,
        0x5b, 0x1c, 0x09, 0x3e,
        0x6d, 0x8b, 0xa4, 0x70,
        0x3d, 0xc9, 0x6f, 0xe7,
        0x2a, 0xf6, 0x4c, 0x58,
        0x3a, 0xc1, 0x91, 0x0b
    };
    private static readonly byte[] Salt4 = {
        0x6e, 0x9c, 0x34, 0xe1,
        0x71, 0x5d, 0x48, 0x3b,
        0x26, 0xd7, 0x1a, 0xac,
        0x84, 0x2d, 0xe3, 0x7f,
        0x0b, 0x92, 0x6f, 0x8e,
        0x29, 0xf6, 0x4a, 0xd8
    };
    private static readonly byte[] Nonce = {
        0x12, 0x34, 0x56, 0x78,
        0x9A, 0xBC, 0xDE, 0xF0,
        0x11, 0x22, 0x33, 0x44,
        0x55, 0x66, 0x77, 0x88,
        0x99, 0xAA, 0xBB, 0xCC,
        0xDD, 0xEE, 0xFF, 0x97
    };
    private static readonly byte[] Nonce2 = {
        0x1c, 0x8f, 0x5a, 0x63,
        0x2e, 0xd7, 0x49, 0x0b,
        0xc2, 0x7f, 0x38, 0x96,
        0xe1, 0xa4, 0x72, 0x9d

    };
    private static readonly byte[] Nonce3 = {
        0x1b, 0x7e, 0x3d, 0x98,
        0x6a, 0xf4, 0x2c, 0xd1,
        0x8f, 0x5b, 0xe0, 0x93,
        0x45, 0x72, 0x6d, 0xa1,
        0xc7, 0x18, 0xf6, 0x85,
        0x29, 0x0e, 0xb4, 0x5f
    };
    private static readonly byte[] Nonce4 = {
        0x56, 0x1f, 0x8d, 0xa2,
        0xe9, 0x5c, 0x32, 0x78,
        0x4b, 0x0d, 0xf7, 0x61,
        0x3e, 0x97, 0xca, 0x25,
        0x89, 0x6a, 0xbf, 0x40,
        0x73, 0x14, 0xd6, 0x2f
    };

    private const string ExpectedResult =
        "EjRWeJq83vARIjNEVWZ3iJmqu8zd7v+XHI9aYy7XSQvCfziW4aRynRyPWmMu10kLwn84luGkcp2zYHJGIimTvMhsS1d0NuUNiU98xEnBZvHL3bUERDSlKFwwIxqw71869mnBt/tSO8WuZkSC+hhzLgLud9kSgI+yZ9rGfwLRqONsASGW1ol07VtLdhwo1/8gJRGR1P/hHoEklOWeJdYHtMDUhV/wgp4D";

    [TestMethod]
    [Description("Generates a random byte array of a specified length.")]
    public void GenerateRndBytes()
    {
        byte[] testBytes = Crypto.RndByteSized(24);
        Assert.IsNotNull(testBytes);
    }
    
    [TestMethod]
    public async Task Decryption()
    {
        var bytes = await Crypto.Argon2Id(passString.ToCharArray(), Salt, 128);

        var key = new byte[32];
        var key2 = new byte[32];
        var hMacKey = new byte[64];

        Buffer.BlockCopy(bytes, 0, key, 0, key.Length);
        Buffer.BlockCopy(bytes, key.Length, key2, 0, key2.Length);
        Buffer.BlockCopy(bytes, key.Length + key2.Length, hMacKey, 0, hMacKey.Length);

        byte[] encryptedTest = await Crypto.EncryptAsyncV3Debug(DataConversionHelpers.StringToByteArray(plainText),
            Nonce, Nonce2, key, key2, hMacKey);

        string encryptResult = DataConversionHelpers.ByteArrayToBase64String(encryptedTest);
        Assert.IsNotNull(encryptedTest);
        Assert.AreEqual(ExpectedResult, encryptResult);
        
        bytes = await Crypto.Argon2Id(passString.ToCharArray(), Salt, 128);
        
        key = new byte[32];
        key2 = new byte[32];
        hMacKey = new byte[64];

        Buffer.BlockCopy(bytes, 0, key, 0, key.Length);
        Buffer.BlockCopy(bytes, key.Length, key2, 0, key2.Length);
        Buffer.BlockCopy(bytes, key.Length + key2.Length, hMacKey, 0, hMacKey.Length);

        byte[] decryptedTest = await Crypto.DecryptAsyncV3(DataConversionHelpers.Base64StringToByteArray(encryptResult), key, key2, hMacKey);
        string decryptResult = DataConversionHelpers.ByteArrayToString(decryptedTest);
        Assert.IsNotNull(decryptResult);
        Assert.AreEqual(plainText, decryptResult);
    }

    [TestMethod]
    public async Task Encryption()
    {
        var bytes = await Crypto.Argon2Id(passString.ToCharArray(), Salt, 128);

        var key = new byte[32];
        var key2 = new byte[32];
        var hMacKey = new byte[64];

        Buffer.BlockCopy(bytes, 0, key, 0, key.Length);
        Buffer.BlockCopy(bytes, key.Length, key2, 0, key2.Length);
        Buffer.BlockCopy(bytes, key.Length + key2.Length, hMacKey, 0, hMacKey.Length);

        byte[] encryptedTest = await Crypto.EncryptAsyncV3Debug(DataConversionHelpers.StringToByteArray(plainText), 
            Nonce, Nonce2, key, key2, hMacKey);
        string s = DataConversionHelpers.ByteArrayToBase64String(encryptedTest);
        Assert.IsNotNull(encryptedTest);
        Assert.AreEqual(ExpectedResult, s);
    }

    [TestMethod]
    public async Task Hash()
    {
        char[] passArray = passString.ToCharArray();
        byte[] result = await Crypto.Argon2Id(passArray, Salt, 32) ?? Array.Empty<byte>();

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task CompareHash()
    {
        string hash1 = "5e463665b62f4ec740145bd1a5062602bdec0c462063ff299303cc8d6f413193";
        string hash2 = "a60d2335fc1fbb39b933a2c5acf9f4f9c5ff1b292cf30d82e246107566efef12";
        byte[] hashBytes1 = DataConversionHelpers.StringToByteArray(hash1);
        byte[] hashBytes2 = DataConversionHelpers.StringToByteArray(hash2);

        Assert.IsFalse(await Crypto.ComparePassword(hashBytes1, hashBytes2));
        Assert.IsTrue(await Crypto.ComparePassword(hashBytes1, hashBytes1));
    }
}