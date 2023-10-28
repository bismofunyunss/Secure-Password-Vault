using System.Text;

namespace TestsProject
{
    [TestClass]
    public class Tests
    {
        /// <summary>
        /// Encrypts text using argon2id.
        /// </summary>
        /// <returns>
        /// An encrypted string in Base64 format.
        /// </returns>
        [TestMethod]
        public async Task EncryptionTest()
        {
            // Arrange
            string userName = "Tester123";
            byte[] userNameBytes = Encoding.UTF8.GetBytes(userName);

            string passWord = "Password1234!";
            byte[] passWordBytes = Encoding.UTF8.GetBytes(passWord);

            string str = "Here is some text to encrypt!";
            byte[] strBytes = Encoding.UTF8.GetBytes(str);

            // Act
            byte[]? result = await Crypto.EncryptAsync(strBytes, passWordBytes, Crypto.RndByteSized(Crypto.IvBit / 8), Crypto.RndByteSized(Crypto.SaltSize));
            
            // Assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Decrypts text using argon2id.
        /// </summary>
        /// <returns>
        /// A decrypted string in Base64 format.
        /// </returns>
        [TestMethod]
        public async Task DecryptionTest()
        {
            // Arrange
            string userName = "Tester123";
            byte[] userNameBytes = Encoding.UTF8.GetBytes(userName);

            string passWord = "Password1234!";
            byte[] passWordBytes = Encoding.UTF8.GetBytes(passWord);

            string str = "Here is some text to encrypt!";
            byte[] strBytes = Encoding.UTF8.GetBytes(str);

            // Act
            byte[]? result = await Crypto.DecryptAsync(strBytes, passWordBytes, Crypto.RndByteSized(Crypto.SaltSize));

            // Assert
            Assert.IsNotNull(result);
        }
    }
}