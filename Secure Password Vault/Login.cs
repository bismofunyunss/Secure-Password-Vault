using System.Diagnostics;
using System.Security;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Secure_Password_Vault;

public partial class Login : Form
{
    private static bool _isAnimating;
    private static char[] _passwordArray = Array.Empty<char>();
    private static int _attemptsRemaining;

    public static SecureString SecurePassword = new();

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
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;
        switch (rememberMeCheckBox.Checked)
        {
            case true:
                Settings.Default.userName = userNameTxt.Text;
                Settings.Default.Save();
                break;
            case false:
                Settings.Default.userName = string.Empty;
                Settings.Default.Save();
                break;
        }

        _attemptsRemaining = int.Parse(AttemptsNumber.Text);

        if (_attemptsRemaining == 0)
        {
            MessageBox.Show(@"No attempts remaining. Please restart the program and try again.", @"Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        try
        {
            if (userNameTxt.Text == string.Empty)
                throw new ArgumentException(@"Value was null or empty.", nameof(userNameTxt));

            _passwordArray = SetArray();

            if (_passwordArray.Length == 0)
                throw new ArgumentException(@"Value was null or empty.", nameof(_passwordArray));

            MessageBox.Show(
                @"Do NOT close the program while loading. This may cause corrupted data that is NOT recoverable.",
                @"Info",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            DisableUi();

            var userExists = Authentication.UserExists(userNameTxt.Text);

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

    public static SecureString ConvertCharArrayToSecureString(char[] charArray)
    {
        if (charArray == Array.Empty<char>())
        {
            throw new ArgumentNullException(nameof(charArray), @"Value was empty.");
        }

        var secureString = new SecureString();

        try
        {
            foreach (var c in charArray)
            {
                secureString.AppendChar(c);
            }

            // Make sure the SecureString is read-only to enhance security.
            secureString.MakeReadOnly();
        }
        catch
        {
            secureString.Dispose(); // Dispose if there is an exception to avoid memory leaks.
            return secureString;
        }

        return secureString;
    }

    private char[] SetArray()
    {
        var buffer = passTxt.Text.Length;
        _passwordArray = new char[buffer];
        passTxt.Text.CopyTo(0, _passwordArray, 0, buffer);

        return _passwordArray;
    }

    private async Task<(byte[] s, byte[] s2, byte[] s3, byte[] s4)> SetSalt()
    {
        var saltBytes = await Authentication.GetUserSaltAsync(userNameTxt.Text);

        return (saltBytes.Salt, saltBytes.Salt2, saltBytes.Salt3, saltBytes.Salt4);
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

        if (showPasswordCheckBox.Checked)
            showPasswordCheckBox.Checked = false;
        _passwordArray = SetArray();

        var decryptedBytes = await Crypto.DecryptFile(userNameTxt.Text, _passwordArray,
            Authentication.GetUserFilePath(userNameTxt.Text));

        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
        if (decryptedBytes != Array.Empty<byte>())
        {
            var decryptedText = DataConversionHelpers.ByteArrayToString(decryptedBytes);
            await File.WriteAllTextAsync(Authentication.GetUserFilePath(userNameTxt.Text),
                decryptedText);

            var saltBytes = await SetSalt();

            _passwordArray = SetArray();

            Authentication.GetUserInfo(userNameTxt.Text, _passwordArray);

            var hashedInput = await Crypto.HashAsync(_passwordArray, saltBytes.s);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
            if (hashedInput == null)
                throw new ArgumentException(@"Hash value returned null.", nameof(hashedInput));

            Authentication.GetUserInfo(userNameTxt.Text, _passwordArray);

            var loginSuccessful = await Crypto.ComparePassword(hashedInput, Crypto.Hash);

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
        else
        {
            HandleFailedLogin();
        }
    }

    private void UserDoesNotExist()
    {
        EnableUi();
        Array.Clear(_passwordArray, 0, _passwordArray.Length);
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
        try
        {
            _passwordArray = SetArray();
            var encryptedUserInfo = await Crypto.EncryptFile(userNameTxt.Text,
                _passwordArray,
                Authentication.GetUserFilePath(userNameTxt.Text));

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
            if (encryptedUserInfo == Array.Empty<byte>())
                throw new ArgumentException(@"Value returned empty or null.",
                    nameof(encryptedUserInfo));
            if (Crypto.Hash != null)
                Array.Clear(Crypto.Hash, 0, Crypto.Hash.Length);

            await File.WriteAllTextAsync(Authentication.GetUserFilePath(userNameTxt.Text),
                DataConversionHelpers.ByteArrayToBase64String(encryptedUserInfo));


            if (File.Exists(Authentication.GetUserVault(userNameTxt.Text)))
            {
                _passwordArray = SetArray();
                var decryptedVault = await Crypto.DecryptFile(userNameTxt.Text,
                    _passwordArray, Authentication.GetUserVault(userNameTxt.Text));

                GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
                if (decryptedVault == Array.Empty<byte>())
                    throw new ArgumentException(@"Value returned empty or null",
                        nameof(decryptedVault));

                await File.WriteAllTextAsync(Authentication.GetUserVault(userNameTxt.Text),
                    DataConversionHelpers.ByteArrayToString(decryptedVault));
                using var userVault = new Vault();
                userVault.LoadVault();
                _passwordArray = SetArray();
                var encryptedBytes = await
                    Crypto.EncryptFile(userNameTxt.Text, _passwordArray,
                        Authentication.GetUserVault(userNameTxt.Text));

                GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
                if (encryptedBytes == Array.Empty<byte>())
                    throw new ArgumentException(@"Value returned empty or null.",
                        nameof(encryptedBytes));
                if (Crypto.Hash != null)
                    Array.Clear(Crypto.Hash, 0, Crypto.Hash.Length);

                await File.WriteAllTextAsync(Authentication.GetUserVault(userNameTxt.Text),
                    DataConversionHelpers.ByteArrayToBase64String(encryptedBytes));
                outputLbl.ForeColor = Color.LimeGreen;
                outputLbl.Text = @"Access granted";
                _isAnimating = false;
                UserLog.LogUser(Authentication.CurrentLoggedInUser);

                _passwordArray = SetArray();
                SecurePassword = ConvertCharArrayToSecureString(_passwordArray);

                Array.Clear(_passwordArray, 0, _passwordArray.Length);
                MessageBox.Show(@"Login successful. Loading vault...", @"Login success.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Hide();
                userVault.ShowDialog();
                Close();
                return;
            }

            _passwordArray = SetArray();
            SecurePassword = ConvertCharArrayToSecureString(_passwordArray);
            Array.Clear(_passwordArray, 0, _passwordArray.Length);
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
        catch (Exception e)
        {
            Array.Clear(_passwordArray, 0, _passwordArray.Length);
            ErrorLogging.ErrorLog(e);
            outputLbl.ForeColor = Color.Red;
            outputLbl.Text = @"Login failed.";
            MessageBox.Show(@"Log in failed. Please check your login credentials and try again.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            outputLbl.ForeColor = Color.WhiteSmoke;
            outputLbl.Text = @"Idle...";
            _isAnimating = false;
            EnableUi();
        }
    }

    private void HandleFailedLogin()
    {
        Array.Clear(_passwordArray, 0, _passwordArray.Length);
        _isAnimating = false;
        EnableUi();
        outputLbl.ForeColor = Color.Red;
        outputLbl.Text = @"Login failed.";
        MessageBox.Show(@"Log in failed! Please recheck your login credentials and try again.", @"Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
        outputLbl.ForeColor = Color.WhiteSmoke;
        outputLbl.Text = @"Idle...";
        _attemptsRemaining--;
        AttemptsNumber.Text = _attemptsRemaining.ToString();
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
        {
            userNameTxt.Text = string.Empty;
            rememberMeCheckBox.Checked = false;
            return;
        }

        userNameTxt.Text = Settings.Default.userName;
        rememberMeCheckBox.Checked = true;
    }
}