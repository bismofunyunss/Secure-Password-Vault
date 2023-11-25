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

    /// <summary>
    /// Retrieves the AES and ChaCha salts associated with a user.
    /// </summary>
    /// <param name="userName">The username of the user.</param>
    /// <returns>
    /// A tuple containing the AES salt and ChaCha salt as byte arrays.
    /// If an error occurs, returns empty byte arrays.
    /// </returns>
    /// <remarks>
    /// The salts are retrieved from files stored in the local application data folder.
    /// The file is named "{userName}.salt".
    /// </remarks>
    public static async Task<byte[]> GetUserSaltAsync(string userName)
    {
        try
        {
            // Construct the path to the user's salt file
            string userSaltFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Password Vault", "Users", userName, $"{userName}.salt");

            // Read salt value from file asynchronously and convert it to a byte array
            var salt = DataConversionHelpers.Base64StringToByteArray(await File.ReadAllTextAsync(userSaltFilePath));

            return salt;
        }
        catch (Exception ex)
        {
            // Handle other unexpected exceptions
            MessageBox.Show("An error occurred while attempting to retrieve salt value.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ErrorLogging.ErrorLog(ex);
            throw;
        }
    }

    /// <summary>
    /// Asynchronously retrieves user information from a file and updates CryptoConstants.Hash.
    /// </summary>
    /// <param name="userName">The user name for which information is retrieved.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="IOException">Thrown if the file specified by the user name does not exist.</exception>
    /// <remarks>
    /// This method constructs a file path using the provided user name and reads information
    /// from the file. It searches for the line containing "User:" and converts the hexadecimal
    /// string from the subsequent line to a byte array, updating CryptoConstants.Hash.
    /// Any exceptions during file reading or processing are logged and rethrown.
    /// </remarks>
    public static async Task GetUserInfo(string userName)
    {
        try
        {
            var path = GetUserFilePath(userName);

            if (!File.Exists(path))
                throw new IOException("File does not exist.");

            var lines = await File.ReadAllLinesAsync(path);

            // Find the line containing "User:"
            var index = Array.IndexOf(lines, "User:");
            if (index == -1)
                return;

            // Convert the hexadecimal string to a byte array and assign it to CryptoConstants.Hash
            Crypto.CryptoConstants.Hash = DataConversionHelpers.HexStringToByteArray(lines[index + 3]);
        }
        catch (Exception ex)
        {
            // Log and rethrow any exceptions
            ErrorLogging.ErrorLog(ex);
            throw;
        }
    }
}