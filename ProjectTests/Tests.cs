using System.Text;

namespace ProjectTests;

[TestClass]
public class Tests
{
    public static readonly string passString = "Password1234567890!!!!";
    public static readonly string plainText = "Here's some text to encrypt.";
    public static byte[] salt = new byte[]
    {
        0x01, 0x23, 0x45, 0x67,
        0x89, 0xAB, 0xCD, 0xEF,
        0xFE, 0xDC, 0xBA, 0x98,
        0x76, 0x54, 0x32, 0x10,
        0x25, 0x36, 0x47, 0x58,
        0x69, 0x7A, 0x8B, 0x9C
    };
    
    [TestMethod]
    public void GenerateRndBytes()
    {
        byte[] testBytes = Crypto.RndByteSized(24);
        Assert.IsNotNull(testBytes);
    }
    
    [TestMethod]
    public async Task xChaCha20Poly1305Decryption()
    {
        byte[] encryptedTest = await Crypto.EncryptAsyncV3(DataConversionHelpers.StringToByteArray(plainText),
            salt,
            Encoding.UTF8.GetBytes(passString));

        byte[] decryptedTest = await Crypto.DecryptAsyncV3(encryptedTest,
            salt,
            Encoding.UTF8.GetBytes(passString));
        string s = Encoding.UTF8.GetString(decryptedTest);
        Assert.IsNotNull(decryptedTest);
    }

    [TestMethod]
    public async Task xChaCha20Poly1305Encryption()
    {
        byte[] encryptedTest = await Crypto.EncryptAsyncV3(DataConversionHelpers.StringToByteArray(plainText),
            salt,
            Encoding.UTF8.GetBytes(passString));
        string s = DataConversionHelpers.ByteArrayToBase64String(encryptedTest);
        Assert.IsNotNull(encryptedTest);
    }
}