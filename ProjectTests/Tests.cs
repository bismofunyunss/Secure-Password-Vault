using System.Text;

namespace ProjectTests
{
    [TestClass]
    public class Tests
    {
        private static readonly string passString = "Password1234567890!!!!";
        private static readonly string plainText = "Here's some text to encrypt.";
        private static byte[] salt { get; set; }
        [TestMethod]
        public static async Task xChaCha20Poly1305Encryption()
        {
                salt = Crypto.RndByteSized(24);
                byte[] encryptedTest = await Crypto.EncryptAsyncV3(DataConversionHelpers.StringToByteArray(plainText),
                    salt,
                    Encoding.UTF8.GetBytes(passString));
                Assert.IsNotNull(encryptedTest);
        }
        public static async Task xChaCha20Poly1305Decryption()
        {
            salt = Crypto.RndByteSized(24);
            byte[] encryptedTest = await Crypto.EncryptAsyncV3(DataConversionHelpers.StringToByteArray(plainText),
                salt,
                Encoding.UTF8.GetBytes(passString));
            Assert.IsNotNull(encryptedTest);
        }
        [TestMethod]
        public static void GenerateRndBytes(int size)
        {
           byte[] testBytes = Crypto.RndByteSized(size);
           Assert.IsNotNull(testBytes);
        }
    }
}