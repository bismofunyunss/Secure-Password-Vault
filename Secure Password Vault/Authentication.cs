using System.Security;
using System.Text;

namespace Secure_Password_Vault
{
    public static class Authentication
    {
        public static string CurrentLoggedInUser { get; set; } = string.Empty;
        private static string UserID { get; set; } = string.Empty;
     
        public static string GetUserFilePath(string userName) =>         
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Password Vault", "Users", userName, $"{userName}.user");

        public static string GetUserVault(string userName) =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Password Vault", "Users", userName, $"{userName}.vault");

        public static bool UserExists(string userName)
        {
            string path = GetUserFilePath(userName);
            return File.Exists(path);
        }


        public static string GetUserInfoPath(string userName) =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Password Vault",
            "Users", userName, $"{userName}.info");


        public static void GetUserInfo(string userName)
        {
            try
            {
                string path = GetUserFilePath(userName);

                if (!File.Exists(path))
                    throw new IOException("File does not exist.");

                string[] lines = File.ReadAllLines(path);
                int index = Array.IndexOf(lines, "User:");
                if (index != -1)
                {
                    UserID = lines[index + 3];
                    byte[]? userHash = DataConversionHelpers.HexStringToByteArray(lines[index + 7]);
                    Crypto.Hash = userHash;
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogging.ErrorLog(ex);
            }
        }
    }
}
