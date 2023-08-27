
using System.Text;

namespace Secure_Password_Vault
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void createNewAccountBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            using RegisterAccount form = new();
            form.ShowDialog();
            this.Close();
        }

        private async void logInBtn_Click(object sender, EventArgs e)
        {
            bool? userExists = Authentication.UserExists(userNameTxt.Text);
            try
            {
                if ((bool)userExists)
                {
                   byte[]? result = await Crypto.DecryptUserFiles(userNameTxt.Text, passTxt.Text);
                    if (result != null)
                    {
                        File.WriteAllText(Authentication.GetUserFilePath(userNameTxt.Text), DataConversionHelpers.ByteArrayToString(result));
                        MessageBox.Show("Decryption successful");
                        string decryptedString = File.ReadAllText(Authentication.GetUserFilePath(userNameTxt.Text));
                        MessageBox.Show(decryptedString);
                        byte[]? result2 = await Crypto.EncryptUserFiles(userNameTxt.Text, passTxt.Text);
                        if (result2 != null)
                        {
                            File.WriteAllText(Authentication.GetUserFilePath(userNameTxt.Text), DataConversionHelpers.ByteArrayToBase64String(result2));
                            MessageBox.Show("Encryption successful");
                            string? encryptedString = File.ReadAllText(Authentication.GetUserFilePath(userNameTxt.Text));
                            MessageBox.Show(encryptedString);
                            byte[]? result3 = await Crypto.DecryptUserFiles(userNameTxt.Text, passTxt.Text);
                            if (result3 != null)
                            {
                                File.WriteAllText(Authentication.GetUserFilePath(userNameTxt.Text), DataConversionHelpers.ByteArrayToString(result3));
                                MessageBox.Show("Decryption successful");
                                decryptedString = File.ReadAllText(Authentication.GetUserFilePath(userNameTxt.Text));
                                MessageBox.Show(decryptedString);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}