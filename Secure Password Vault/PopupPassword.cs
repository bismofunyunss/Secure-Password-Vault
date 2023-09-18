namespace Secure_Password_Vault;

public partial class PopupPassword : Form
{
    private static bool _isAnimating;
    public static char[] PasswordArray = Array.Empty<char>();

    public PopupPassword()
    {
        InitializeComponent();
    }

    public static bool Save { get; set; }
    public new static bool Load { get; set; }

    private async void confirmPassBtn_ClickAsync(object sender, EventArgs e)
    {
        StartAnimation();
        var buffer = passTxt.Text.Length;
        PasswordArray = new char[buffer];
        passTxt.Text.CopyTo(0, PasswordArray, 0, buffer);
        try
        {
            var decryptedBytes = await Crypto.DecryptUserFiles(Authentication.CurrentLoggedInUser, PasswordArray,
                Authentication.GetUserFilePath(Authentication.CurrentLoggedInUser));

            if (decryptedBytes == null)
                return;
            var decryptedText = DataConversionHelpers.ByteArrayToString(decryptedBytes);
            await File.WriteAllTextAsync(Authentication.GetUserFilePath(Authentication.CurrentLoggedInUser), decryptedText);
            var hashedInput = await Task.Run(() => Crypto.HashAsync(PasswordArray, Crypto.Salt));
            if (hashedInput == null)
                throw new ArgumentException(@"Hash value returned null.", nameof(hashedInput));

            Authentication.GetUserInfo(Authentication.CurrentLoggedInUser);

            var loginSuccessful = await Task.Run(() => Crypto.ComparePassword(hashedInput));
            var encryptedBytes = await Task.Run(() => Crypto.EncryptUserFiles(Authentication.CurrentLoggedInUser,
                PasswordArray, Authentication.GetUserFilePath(Authentication.CurrentLoggedInUser)));
            if (encryptedBytes == null)
                throw new ArgumentException(@"Value returned empty or null.", nameof(encryptedBytes));

            var encryptedText = DataConversionHelpers.ByteArrayToBase64String(encryptedBytes);
            await File.WriteAllTextAsync(Authentication.GetUserFilePath(Authentication.CurrentLoggedInUser), encryptedText);

            switch (loginSuccessful)
            {
                case true:
                {
                    Array.Clear(hashedInput, 0, hashedInput.Length);
                    if (Save)
                    {
                        _isAnimating = false;
                        outputLbl.ForeColor = Color.LimeGreen;
                        outputLbl.Text = @"Vault saved.";
                        var encryptedVault = await Task.Run(() =>
                            Crypto.EncryptUserFiles(Authentication.CurrentLoggedInUser, PasswordArray,
                                Authentication.GetUserVault(Authentication.CurrentLoggedInUser)));
                        if (encryptedVault != null)
                            await File.WriteAllTextAsync(Authentication.GetUserVault(Authentication.CurrentLoggedInUser),
                                DataConversionHelpers.ByteArrayToBase64String(encryptedVault));
                        Array.Clear(PasswordArray, 0, PasswordArray.Length);
                        MessageBox.Show(@"Vault saved successfully.", @"Success", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        Save = false;
                    }

                    if (Load)
                    {
                        if (File.Exists(Authentication.GetUserVault(Authentication.CurrentLoggedInUser)))
                        {
                            _isAnimating = false;
                            outputLbl.ForeColor = Color.LimeGreen;
                            outputLbl.Text = @"Access granted.";
                            var decryptedVault = await Task.Run(() =>
                                Crypto.DecryptUserFiles(Authentication.CurrentLoggedInUser, PasswordArray,
                                    Authentication.GetUserVault(Authentication.CurrentLoggedInUser)));
                            if (decryptedVault != null)
                                await File.WriteAllTextAsync(Authentication.GetUserVault(Authentication.CurrentLoggedInUser),
                                    DataConversionHelpers.ByteArrayToString(decryptedVault));
                            MessageBox.Show(@"Vault loaded successfully.", @"Success", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            Load = false;
                        }
                        else
                        {
                            _isAnimating = false;
                            outputLbl.ForeColor = Color.DarkRed;
                            outputLbl.Text = @"Idle...";
                            throw new ArgumentException("Unable to locate vault file.");
                        }
                    }

                    if (Crypto.Hash != null)
                        Array.Clear(Crypto.Hash, 0, Crypto.Hash.Length);
                    Close();
                    break;
                }
                case false:
                    _isAnimating = false;
                    outputLbl.ForeColor = Color.DarkRed;
                    outputLbl.Text = @"Idle...";
                    Array.Clear(PasswordArray, 0, PasswordArray.Length);
                    MessageBox.Show(@"Please recheck your login credentials and try again.", @"Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }
        catch (Exception ex)
        {
            _isAnimating = false;
            outputLbl.ForeColor = Color.DarkRed;
            outputLbl.Text = @"Idle...";
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ErrorLogging.ErrorLog(ex);
        }
    }

    private void cancelBtn_Click(object sender, EventArgs e)
    {
        Load = false;
        Save = false;
        Close();
    }

    private void PopupPassword_FormClosing(object sender, FormClosingEventArgs e)
    {
        Load = false;
        Save = false;
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
            outputLbl.Text = @"Confirming password";
            // Add animated periods
            for (var i = 0; i < 4; i++)
            {
                outputLbl.Text += @".";
                await Task.Delay(400); // Delay between each period
            }
        }
    }
}