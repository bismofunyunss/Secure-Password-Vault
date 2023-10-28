using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using System.Text;

namespace Test
{
    public class Extensions
    {
        private static readonly RandomNumberGenerator RndNum = RandomNumberGenerator.Create();
        public static byte[] RndByteSized(int size)
        {
            var buffer = new byte[size];
            RndNum.GetBytes(buffer);
            return buffer;
        }
    }
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void EncryptionTest()
        {
            // Arrange
            string str = "Here is some text!";
            byte[] strBytes = Encoding.UTF8.GetBytes(str);

            string passWord = "Password123";
            byte[] passWordBytes = Encoding.UTF8.GetBytes(passWord);

            byte[] iv = Extensions.RndByteSized(128 / 8);
            byte[] salt = Extensions.RndByteSized(512 / 8);


            
        }
    }
}
