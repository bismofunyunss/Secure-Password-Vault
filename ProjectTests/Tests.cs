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
    private static readonly byte[] Nonce = {
        0x12, 0x34, 0x56, 0x78,
        0x9A, 0xBC, 0xDE, 0xF0,
        0x11, 0x22, 0x33, 0x44,
        0x55, 0x66, 0x77, 0x88,
        0x99, 0xAA, 0xBB, 0xCC,
        0xDD, 0xEE, 0xFF, 0x97
    };
    string expectedResult = "EjRWeJq83vARIjNEVWZ3iJmqu8zd7v+XoyF5h7qaUpSf5dhIzLauSEwFuMq49mGE8EKA9zHiOj/GBi4j2UrnGLgeUhA=";

    [TestMethod]
    [Description("Generates a random byte array of a specified length.")]
    public void GenerateRndBytes()
    {
        byte[] testBytes = Crypto.RndByteSized(24);
        Assert.IsNotNull(testBytes);
    }
    
    [TestMethod]
    public async Task xChaCha20Poly1305Decryption()
    {
        byte[] encryptedTest = await Crypto.EncryptAsyncV3Debug(DataConversionHelpers.StringToByteArray(plainText),
            Salt,
            Encoding.UTF8.GetBytes(passString), Nonce);

        string encryptResult = DataConversionHelpers.ByteArrayToBase64String(encryptedTest);
        Assert.IsNotNull(encryptedTest);
        Assert.AreEqual(encryptResult, expectedResult);

        byte[] decryptedTest = await Crypto.DecryptAsyncV3(encryptedTest, Salt, Encoding.UTF8.GetBytes(passString));
        string decryptResult = DataConversionHelpers.ByteArrayToString(decryptedTest);
        Assert.IsNotNull(decryptResult);
        Assert.AreEqual(plainText, decryptResult);
    }

    [TestMethod]
    public async Task xChaCha20Poly1305Encryption()
    {
        byte[] encryptedTest = await Crypto.EncryptAsyncV3Debug(DataConversionHelpers.StringToByteArray(plainText),
            Salt,
            Encoding.UTF8.GetBytes(passString), Nonce);
        string s = DataConversionHelpers.ByteArrayToBase64String(encryptedTest);
        Assert.IsNotNull(encryptedTest);
        Assert.AreEqual(expectedResult, s);
    }

    [TestMethod]
    public async Task Hash()
    {
        char[] passArray = passString.ToCharArray();
        byte[] result = await Crypto.HashAsync(passArray, Salt) ?? Array.Empty<byte>();

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