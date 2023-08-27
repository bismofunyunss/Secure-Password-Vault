using System.Data;
using System.Text;
using MessagePack;
using System.IO;
namespace Secure_Password_Vault
{
    public partial class RegisterAccount : Form
    {
        public RegisterAccount()
        {
            InitializeComponent();
        }
        private static bool isAnimating = false;
        private static bool CheckPasswordValidity(string password, string password2)
        {
            if (password.Length < 8 || password.Length > 64)
                return false;

            if (!password.Any(char.IsUpper) || !password.Any(char.IsLower) || !password.Any(char.IsDigit))
                return false;

            if (password.Contains(' ') || password != password2)
                return false;

            return password.Any(char.IsSymbol) || password.Any(char.IsPunctuation);
        }


        private async void CreateAccount()
        {
            try
            {
                string userName = userTxt.Text;
                string password = passTxt.Text;

                string userDirectory = CreateDirectoryIfNotExists(Path.Combine("Password Vault", "Users", userName));
                string userFile = Path.Combine(userDirectory, $"{userName}.user");
                string userInfo = Path.Combine(userDirectory, "UserInfo.info");

                bool userExists = Authentication.UserExists(userName);


                if (!userExists)
                {
                    StartAnimation();
                    ValidateUsernameAndPassword(userName, password);

                    string userID = Guid.NewGuid().ToString();
                    Crypto.Salt = Crypto.RndByteSized(Crypto.SaltSize);
                    Crypto.IV = Crypto.RndByteSized(Crypto.IVBit / 8);
                    string? hashedPassword = await Crypto.HashAsync(password, Crypto.Salt);
                    string? saltString = DataConversionHelpers.ByteArrayToBase64String(Crypto.Salt);

                    File.WriteAllText(userFile, $"User:\n{userName}\nUserID:\n{userID}\nSalt:\n{saltString}\nHash:\n{hashedPassword.Trim()}\n");

                    DialogResult dialogResult = MessageBox.Show("Registration successful! Make sure you do NOT forget your password or you will lose access " +
                        "to all of your files.", "Registration Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    string textString = File.ReadAllText(userFile);
                    byte[]? textBytes = DataConversionHelpers.StringToByteArray(textString);
                    string? derivedKey = await Crypto.DeriveAsync(password, Crypto.Salt);
                    if (derivedKey == null)
                        throw new ArgumentException("Value returned null or empty.",  nameof(derivedKey));
                    byte[] keyBytes = Encoding.UTF8.GetBytes(derivedKey);
                    byte[]? encrypted = Crypto.Encrypt(textBytes, keyBytes);
                    if (encrypted == null)
                        throw new ArgumentException("Value returned null or empty.", nameof(encrypted));
                    File.WriteAllText(userFile, DataConversionHelpers.ByteArrayToBase64String(encrypted));
                    File.AppendAllText(userInfo, DataConversionHelpers.ByteArrayToBase64String(Crypto.Salt) + DataConversionHelpers.ByteArrayToBase64String(Crypto.IV));

                    if (dialogResult == DialogResult.OK)
                    {
                        this.Hide();
                        using Login form = new();
                        form.ShowDialog();
                        this.Close();
                    }
                }
                else
                {
                    isAnimating = false;
                    throw new ArgumentException("Username already exists", userTxt.Text);
                }
            }
            catch (ArgumentException ex)
            {
                isAnimating |= false;
                ErrorLogging.ErrorLog(ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ValidateUsernameAndPassword(string userName, string password)
        {
            if (!userName.All(c => char.IsLetterOrDigit(c) || c == '_' || c == ' '))
                throw new ArgumentException("Value contains illegal characters. Valid characters are letters, digits, underscores, and spaces.", nameof(userName));

            if (string.IsNullOrEmpty(userName) || userName.Length > 20)
                throw new ArgumentException("Invalid username.", nameof(userName));

            if (string.IsNullOrEmpty(password) || !CheckPasswordValidity(password, confirmPassTxt.Text))
                throw new ArgumentException("Invalid password.", nameof(password));

            if (!CheckPasswordValidity(passTxt.Text, confirmPassTxt.Text))
                throw new ArgumentException("Password must contain between 8 and 64 characters. " +
                                          "It also must include:\n1.) At least one uppercase letter.\n2.) At least one lowercase letter.\n" +
                                          "3.) At least one number.\n4.) At least one special character.\n5.) Must not contain any spaces.\n" +
                                          "6.) Both passwords must match.\n", nameof(passTxt));
        }




        private static string CreateDirectoryIfNotExists(string directoryPath)
        {
            string fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), directoryPath);
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);

            return fullPath;
        }


        private void createAccountBtn_Click(object sender, EventArgs e)
        {
            CreateAccount();
        }


        private async void StartAnimation()
        {
            isAnimating = true;
            await AnimateLabel();
        }


        private async Task AnimateLabel()
        {
            while (isAnimating)
            {
                outputLbl.Text = "Creating account";

                // Add animated periods
                for (int i = 0; i < 4; i++)
                {
                    outputLbl.Text += ".";
                    await Task.Delay(400); // Delay between each period
                }
            }
        }
    }
}
