using Microsoft.VisualStudio.TestTools.UnitTesting;
using Secure_Password_Vault;

namespace Secure_Password_VaultTests;

[TestClass]
public class CryptoTests
{
    [TestMethod]
    public void EncryptFileTest()
    {
        var passWord = new[] { 'F', 'u', 'c', 'k' };
        var file = Authentication.GetUserFilePath("Test");
        var result = Crypto.EncryptUserFiles("Test", passWord, file);

        Assert.IsNotNull(result);
    }
    [TestMethod]
    public void DecryptUserFilesTest()
    {
        var passWord = new[] { 'F', 'u', 'c', 'k' };
        var file = Authentication.GetUserFilePath("Test");
        var result = Crypto.EncryptUserFiles("Test", passWord, file);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void RegisterTest()
    {
        RegisterAccount.
        

    }
}