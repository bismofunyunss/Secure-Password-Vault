using System;

namespace Secure_Password_Vault;

public static class Authentication
{
    public static string CurrentLoggedInUser { get; set; } = string.Empty;

    public static string GetUserFilePath(string userName)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Password Vault",
            "Users", userName, $"{userName}.user");
    }

    public static string GetUserVault(string userName)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Password Vault",
            "Users", userName, $"{userName}.vault");
    }

    public static bool UserExists(string userName)
    {
        var path = GetUserFilePath(userName);
        return File.Exists(path);
    }


    public static async Task<(byte[] Salt, byte[] Salt2, byte[] Salt3, byte[] Salt4)> GetUserSaltAsync(string userName)
    { 
        try
        {
            var salt = await File.ReadAllTextAsync(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                "Password Vault", "Users", userName, $"{userName}.salt"));
            var salt2 = await File.ReadAllTextAsync(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Password Vault", "Users", userName, $"{userName}-Salt2.salt"));
            var salt3 = await File.ReadAllTextAsync(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Password Vault", "Users", userName, $"{userName}-Salt3.salt"));
            var salt4 = await File.ReadAllTextAsync(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Password Vault", "Users", userName, $"{userName}-Salt4.salt"));

            var saltResult = DataConversionHelpers.Base64StringToByteArray(salt);
            var saltResult2 = DataConversionHelpers.Base64StringToByteArray(salt2);
            var saltResult3 = DataConversionHelpers.Base64StringToByteArray(salt3);
            var saltResult4 = DataConversionHelpers.Base64StringToByteArray(salt4);

            return (saltResult, saltResult2, saltResult3, saltResult4);
        }
        catch (IOException ex)
        {
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ErrorLogging.ErrorLog(ex);
            return (Array.Empty<byte>(), Array.Empty<byte>(), Array.Empty<byte>(), Array.Empty<byte>());
        }
    }

    public static async void GetUserInfo(string userName, char[] passWord)
    {
        var path = GetUserFilePath(userName);

        if (!File.Exists(path))
            throw new IOException("File does not exist.");
        try
        {
            var lines = await File.ReadAllLinesAsync(path);
            var index = Array.IndexOf(lines, "User:");
            if (index == -1) return;
            Crypto.Hash = DataConversionHelpers.HexStringToByteArray(lines[index + 3]) ?? Array.Empty<byte>();
        }
        catch (IOException ex)
        {
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ErrorLogging.ErrorLog(ex);
        }
    }
}