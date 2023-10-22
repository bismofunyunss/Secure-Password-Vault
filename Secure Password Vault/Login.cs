namespace Secure_Password_Vault;

public partial class Login : Form
{
    private static bool _isAnimating;
    public static char[]? PasswordArray = Array.Empty<char>();

    public Login()
    {
        InitializeComponent();
    }

    private void createNewAccountBtn_Click(object sender, EventArgs e)
    {
        Hide();
        using RegisterAccount form = new();
        form.ShowDialog();
        Close();
    }

    private async void logInBtn_Click(object sender, EventArgs e)
    {
        if (rememberMeCheckBox.Checked)
        {
            Settings.Default.userName = userNameTxt.Text;
            Settings.Default.Save();
        }

        try
        {
            if (userNameTxt.Text == string.Empty)
                throw new ArgumentException(@"Value was null or empty.", nameof(userNameTxt));

            PasswordArray = SetArray();

            if (PasswordArray.Length == 0)
                throw new ArgumentException(@"Value was null or empty.", nameof(PasswordArray));

            MessageBox.Show(
                @"Do NOT close the program while loading. This may cause corrupted data that is NOT recoverable.",
                @"Info",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            DisableUi();

            bool userExists = Authentication.UserExists(userNameTxt.Text);

            await ProcessLoginAsync(userExists);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ErrorLogging.ErrorLog(ex);
        }
    }

    private void DisableUi()
    {
        createNewAccountBtn.Enabled = false;
        passTxt.Enabled = false;
        userNameTxt.Enabled = false;
        logInBtn.Enabled = false;
        rememberMeCheckBox.Enabled = false;
        showPasswordCheckBox.Enabled = false;
    }

    private void EnableUi()
    {
        logInBtn.Enabled = true;
        createNewAccountBtn.Enabled = true;
        passTxt.Enabled = true;
        userNameTxt.Enabled = true;
        rememberMeCheckBox.Enabled = true;
        showPasswordCheckBox.Enabled = true;
    }

    private char[] SetArray()
    {
        var buffer = passTxt.Text.Length;
        PasswordArray = new char[buffer];
        passTxt.Text.CopyTo(0, PasswordArray, 0, buffer);

        return PasswordArray;
    }

    private async Task<byte[]?> SetSalt()
    {
        var saltString = await File.ReadAllTextAsync(Authentication.GetUserSalt(userNameTxt.Text));
        var saltBytes = DataConversionHelpers.Base64StringToByteArray(saltString);

        return saltBytes;
    }

    private async Task ProcessLoginAsync(bool userExists)
    {
        switch (userExists)
        {
            case true:
                await StartLoginProcessAsync();
                break;
            case false:
                UserDoesNotExist();
                break;
        }
    }

    private async Task StartLoginProcessAsync()
    {
        StartAnimation();

        var decryptedBytes = await Crypto.DecryptFile(userNameTxt.Text, PasswordArray,
            Authentication.GetUserFilePath(userNameTxt.Text));

        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
        if (decryptedBytes != null)
        {
            var decryptedText = DataConversionHelpers.ByteArrayToString(decryptedBytes);
            await File.WriteAllTextAsync(Authentication.GetUserFilePath(userNameTxt.Text),
                decryptedText);

            var saltBytes = await SetSalt();

            if (PasswordArray != null)
            {
                Authentication.GetUserInfo(userNameTxt.Text, PasswordArray);
                {
                    var hashedInput = await Crypto.HashAsync(PasswordArray, saltBytes);
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
                    if (hashedInput == null)
                        throw new ArgumentException(@"Hash value returned null.", nameof(hashedInput));

                    var loginSuccessful = await Crypto.ComparePassword(hashedInput);
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
                    Array.Clear(hashedInput, 0, hashedInput.Length);
                    if (Crypto.Hash != null)
                        Array.Clear(Crypto.Hash, 0, Crypto.Hash.Length);
                    switch (loginSuccessful)
                    {
                        case true:
                            HandleLogin();
                            break;
                        case false:
                            HandleFailedLogin();
                            break;
                    }
                }
            }
        }
        else
        {
            HandleFailedLogin();
        }
    }

    private void UserDoesNotExist()
    {
        EnableUi();
        if (PasswordArray != null)
            Array.Clear(PasswordArray, 0, PasswordArray.Length);
        _isAnimating = false;
        outputLbl.ForeColor = Color.WhiteSmoke;
        outputLbl.Text = @"Idle...";
        MessageBox.Show(@"Username does not exist.", @"Error", MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    private async void HandleLogin()
    {
        if (!File.Exists(Authentication.GetUserFilePath(userNameTxt.Text)))
            return;
        Authentication.CurrentLoggedInUser = userNameTxt.Text;
        if (PasswordArray != null)
        {
            var encryptedUserInfo = await Crypto.EncryptFile(userNameTxt.Text,
                PasswordArray,
                Authentication.GetUserFilePath(userNameTxt.Text));

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
            if (encryptedUserInfo == null)
                throw new ArgumentException(@"Value returned empty or null.",
                    nameof(encryptedUserInfo));
            if (Crypto.Hash != null)
                Array.Clear(Crypto.Hash, 0, Crypto.Hash.Length);

            await File.WriteAllTextAsync(Authentication.GetUserFilePath(userNameTxt.Text),
                DataConversionHelpers.ByteArrayToBase64String(encryptedUserInfo));


            if (File.Exists(Authentication.GetUserVault(userNameTxt.Text)))
            {
                var decryptedVault = await Crypto.DecryptFile(userNameTxt.Text,
                    PasswordArray, Authentication.GetUserVault(userNameTxt.Text));
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
                await File.WriteAllTextAsync(Authentication.GetUserVault(userNameTxt.Text),
                    DataConversionHelpers.ByteArrayToString(decryptedVault));
                using var userVault = new Vault();
                userVault.LoadVault();
                if (PasswordArray != null)
                {
                    var encryptedBytes = await
                        Crypto.EncryptFile(userNameTxt.Text, PasswordArray,
                            Authentication.GetUserVault(userNameTxt.Text));
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
                    if (encryptedBytes == null)
                        throw new ArgumentException(@"Value returned empty or null.",
                            nameof(encryptedBytes));
                    if (Crypto.Hash != null)
                        Array.Clear(Crypto.Hash, 0, Crypto.Hash.Length);

                    await File.WriteAllTextAsync(Authentication.GetUserVault(userNameTxt.Text),
                        DataConversionHelpers.ByteArrayToBase64String(encryptedBytes));
                }
                outputLbl.ForeColor = Color.LimeGreen;
                outputLbl.Text = @"Access granted";
                _isAnimating = false;
                UserLog.LogUser(Authentication.CurrentLoggedInUser);
                MessageBox.Show(@"Login successful. Loading vault...", @"Login success.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Hide();
                userVault.ShowDialog();
                Close();
                return;
            }
            outputLbl.ForeColor = Color.LimeGreen;
            outputLbl.Text = @"Access granted";
            _isAnimating = false;
            UserLog.LogUser(Authentication.CurrentLoggedInUser);
            MessageBox.Show(@"Login successful. Loading vault...", @"Login success.",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            Hide();
            using var blankVault = new Vault();
            blankVault.ShowDialog();
            Close();
        }
    }

    private void HandleFailedLogin()
    {
        _isAnimating = false;
        EnableUi();
        outputLbl.ForeColor = Color.WhiteSmoke;
        outputLbl.Text = @"Idle...";
        MessageBox.Show(@"Log in failed! Please recheck your login credentials and try again.", @"Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            outputLbl.Text = @"Logging in";
            // Add animated periods
            for (var i = 0; i < 4; i++)
            {
                outputLbl.Text += @".";
                await Task.Delay(400); // Delay between each period
            }
        }
    }

    private void showPasswordCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        passTxt.UseSystemPasswordChar = !showPasswordCheckBox.Checked;
    }

    private void Login_Load(object sender, EventArgs e)
    {
        if (Settings.Default.userName == string.Empty)
            return;
        userNameTxt.Text = Settings.Default.userName;
        rememberMeCheckBox.Checked = true;
    }
}