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


    public static string GetUserSalt(string userName)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Password Vault",
            "Users", userName, $"{userName}.salt");
    }

    public static string GetUserSalt2(string userName)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Password Vault",
            "Users", userName, $"{userName}-Salt2.salt");
    }

    public static string GetUserSalt3(string userName)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Password Vault",
            "Users", userName, $"{userName}-Salt3.salt");
    }

    public static string GetUserSalt4(string userName)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Password Vault",
            "Users", userName, $"{userName}-Salt4.salt");
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
            Crypto.Hash = DataConversionHelpers.HexStringToByteArray(lines[index + 6]) ?? Array.Empty<byte>();
        }
        catch (IOException ex)
        {
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ErrorLogging.ErrorLog(ex);
        }
    }
}