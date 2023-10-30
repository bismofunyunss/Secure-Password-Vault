namespace Secure_Password_Vault;

public partial class EnterPassword : Form
{
    public static bool MatchedHash;
    public static bool FormClose;
    public static char[] PasswordArray = Array.Empty<char>();
    private static bool _isAnimating;

    public EnterPassword()
    {
        InitializeComponent();
    }

    private async Task<byte[]?> SetSalt()
    {
        var saltString = await File.ReadAllTextAsync(Authentication.GetUserSalt(Authentication.CurrentLoggedInUser));
        var saltBytes = DataConversionHelpers.Base64StringToByteArray(saltString);

        return saltBytes;
    }

    private void EnableUi()
    {
        PassWordBox.Enabled = true;
        EnterPasswordBtn.Enabled = true;
        showPasswordCheckBox.Enabled = true;
    }

    private void DisableUi()
    {
        PassWordBox.Enabled = false;
        EnterPasswordBtn.Enabled = false;
        showPasswordCheckBox.Enabled = false;
    }

    private async void EnterPasswordBtn_Click(object sender, EventArgs e)
    {
        PasswordArray = PassWordBox.Text.ToCharArray();
        StartAnimation();
        DisableUi();
        try
        {
            if (showPasswordCheckBox.Checked)
                showPasswordCheckBox.Checked = false;

            var decryptedBytes = await Crypto.DecryptFile(Authentication.CurrentLoggedInUser, PasswordArray,
                Authentication.GetUserFilePath(Authentication.CurrentLoggedInUser));

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
            if (decryptedBytes == Array.Empty<byte>())
                return;

            var decryptedText = DataConversionHelpers.ByteArrayToString(decryptedBytes);
            await File.WriteAllTextAsync(Authentication.GetUserFilePath(Authentication.CurrentLoggedInUser),
                decryptedText);

            var saltBytes = await SetSalt();

            if (PasswordArray == Array.Empty<char>())
                return;

            Authentication.GetUserInfo(Authentication.CurrentLoggedInUser, PasswordArray);
            {
                var hashedInput = await Crypto.HashAsync(PasswordArray, saltBytes);
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
                if (hashedInput == null)
                    throw new ArgumentException(@"Hash value returned null.", nameof(hashedInput));

                MatchedHash = await Crypto.ComparePassword(hashedInput);
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
                Array.Clear(hashedInput, 0, hashedInput.Length);

                var encryptedUserInfo = await Crypto.EncryptFile(Authentication.CurrentLoggedInUser,
                    PasswordArray,
                    Authentication.GetUserFilePath(Authentication.CurrentLoggedInUser));

                GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
                if (encryptedUserInfo == Array.Empty<byte>())
                    throw new ArgumentException(@"Value returned empty or null.",
                        nameof(encryptedUserInfo));
                if (Crypto.Hash != null)
                    Array.Clear(Crypto.Hash, 0, Crypto.Hash.Length);

                await File.WriteAllTextAsync(Authentication.GetUserFilePath(Authentication.CurrentLoggedInUser),
                    DataConversionHelpers.ByteArrayToBase64String(encryptedUserInfo));
                _isAnimating = false;
                Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ErrorLogging.ErrorLog(ex);
            Close();
        }
    }

    private void EnterPassword_FormClosing(object sender, FormClosingEventArgs e)
    {
        FormClose = true;
    }

    private void showPasswordCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        PassWordBox.UseSystemPasswordChar = !showPasswordCheckBox.Checked;
    }

    private async void StartAnimation()
    {
        _isAnimating = true;
        await AnimateLabel();
    }

    private async Task AnimateLabel()
    {
        while (_isAnimating)
        {
            outputLbl.Text = @"Verifying password";
            // Add animated periods
            for (var i = 0; i < 4; i++)
            {
                outputLbl.Text += @".";
                await Task.Delay(400); // Delay between each period
            }
        }
    }
}