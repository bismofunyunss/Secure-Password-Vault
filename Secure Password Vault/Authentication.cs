using Microsoft.VisualBasic.ApplicationServices;
using System.DirectoryServices.ActiveDirectory;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace Secure_Password_Vault
{
    public static class Authentication
    {
        public static string CurrentLoggedInUser { get; set; } = string.Empty;
  
        private static string UserID { get; set; } = string.Empty;
     
        public static string GetUserFilePath(string userName) =>         
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Password Vault", "Users", userName, $"{userName}.user");


        public static bool UserExists(string userName)
        {
            string path = GetUserFilePath(userName);
            return File.Exists(path);
        }


        public static string GetUserInfoPath(string userName) =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Password Vault",
            "Users", userName, "UserInfo.info");


        public static void GetUserInfo(string userName)
        {
            try
            {
                string path = GetUserFilePath(userName);

                if (!File.Exists(path))
                    throw new IOException("File does not exist.");

                string[] lines = File.ReadAllLines(path);
                int index = Array.IndexOf(lines, userName);
                if (index != -1)
                {
                    UserID = lines[index + 4];
                    Crypto.Salt = DataConversionHelpers.StringToByteArray(lines[index + 6]);
                    Crypto.Hash = lines[index + 8];
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
