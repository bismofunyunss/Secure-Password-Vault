using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Secure_Password_Vault;

public partial class Login : Form
{
    private static bool _isAnimating;
    private static char[] _passwordArray = Array.Empty<char>();
    private static int _attemptsRemaining;
    public Login()
    {
        InitializeComponent();
    }

    private void CreateNewAccountBtn_Click(object sender, EventArgs e)
    {
        Hide();
        using RegisterAccount form = new();
        form.ShowDialog();
        Close();
    }

    /// <summary>
    ///     Handles the click event for the logInBtn, attempts user login, and performs necessary UI updates.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    private async void LogInBtn_Click(object sender, EventArgs e)
    {
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

        var canParse = int.TryParse(AttemptsNumber.Text, out _attemptsRemaining);

        if (canParse)
        {
            if (_attemptsRemaining == 0)
            {
                MessageBox.Show("No attempts remaining. Please restart the program and try again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        else
        {
            throw new Exception("Unable to parse attempts remaining value.");
        }

        try
        {
            if (userNameTxt.Text == string.Empty)
                throw new ArgumentException("Value was empty.", nameof(userNameTxt));

            _passwordArray = CreateArray();

            if (_passwordArray.Length == 0)
                throw new ArgumentException("Value was empty.", nameof(_passwordArray));

            MessageBox.Show(
                "Do NOT close the program while loading. This may cause corrupted data that is NOT recoverable.",
                "Info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            DisableUi();

            var userExists = Authentication.UserExists(userNameTxt.Text);

            await ProcessLogin(userExists);
        }
        catch (Exception ex)
        {
            Crypto.CryptoUtilities.ClearMemory(_passwordArray ?? Array.Empty<char>());
            ErrorLogging.ErrorLog(ex);
            _isAnimating = false;
            outputLbl.Text = "Login failed.";
            outputLbl.ForeColor = Color.Red;
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            _attemptsRemaining--;
            AttemptsNumber.Text = _attemptsRemaining.ToString();
            outputLbl.ForeColor = Color.WhiteSmoke;
            outputLbl.Text = "Idle...";
            EnableUi();
        }
    }


    private void DisableUi()
    {
        CreateNewAccountBtn.Enabled = false;
        passTxt.Enabled = false;
        userNameTxt.Enabled = false;
        LogInBtn.Enabled = false;
        rememberMeCheckBox.Enabled = false;
        showPasswordCheckBox.Enabled = false;
    }

    private void EnableUi()
    {
        LogInBtn.Enabled = true;
        CreateNewAccountBtn.Enabled = true;
        passTxt.Enabled = true;
        userNameTxt.Enabled = true;
        rememberMeCheckBox.Enabled = true;
        showPasswordCheckBox.Enabled = true;
    }

    /// <summary>
    /// Performs continuous checks and terminates the process after a random delay.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static async Task Checks()
    {
        bool result = false;

        while (true)
        {
            result = await AntiTamper.PerformChecks();

            if (result == true)
                break;
        }

        await Task.Delay(Crypto.CryptoUtilities.BoundedInt(5000, 15000));
        Process.GetCurrentProcess().Kill();
    }

    private char[] CreateArray()
    {
        var arrayPtr = Crypto.ConversionMethods.CreatePinnedCharArray(passTxt.Text);
        int len = passTxt.Text.Length;
        var charArray = Crypto.ConversionMethods.ConvertIntPtrToCharArray(arrayPtr, len);

        Marshal.FreeCoTaskMem(arrayPtr);
        arrayPtr = IntPtr.Zero;

        return charArray;
    }

    private async Task ProcessLogin(bool userExists)
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

    /// <summary>
    ///     Asynchronously initiates the login process by performing decryption, password hashing,
    ///     and handling login success or failure accordingly.
    /// </summary>
    private async Task StartLoginProcessAsync()
    {
        StartAnimation();

        Authentication.CurrentLoggedInUser = userNameTxt.Text;

        if (showPasswordCheckBox.Checked)
            showPasswordCheckBox.Checked = false;

        _passwordArray = CreateArray();

        var decryptedBytes = await Crypto.DecryptFile(Authentication.CurrentLoggedInUser, _passwordArray,
            Authentication.GetUserFilePath(Authentication.CurrentLoggedInUser));

        if (decryptedBytes == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(decryptedBytes));

        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

        var decryptedText = DataConversionHelpers.ByteArrayToString(decryptedBytes);
        await File.WriteAllTextAsync(Authentication.GetUserFilePath(Authentication.CurrentLoggedInUser), decryptedText);

        var salt = Authentication.GetUserSalt(userNameTxt.Text);

        Authentication.GetUserInfo(userNameTxt.Text);

        var hashedInput = await Crypto.HashingMethods.Argon2Id(_passwordArray, salt, 32);

        var encryptedBytes = await Crypto.EncryptFile(Authentication.CurrentLoggedInUser, _passwordArray, Authentication.GetUserFilePath(Authentication.CurrentLoggedInUser));

        if (encryptedBytes == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(decryptedBytes));

        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

        var encryptedText = DataConversionHelpers.ByteArrayToBase64String(encryptedBytes);
        await File.WriteAllTextAsync(Authentication.GetUserFilePath(Authentication.CurrentLoggedInUser), encryptedText);

        Crypto.CryptoUtilities.ClearMemory(_passwordArray);

        if (hashedInput == Array.Empty<byte>())
            throw new ArgumentException("Value was empty.", nameof(hashedInput));

        var loginSuccessful = await Crypto.CryptoUtilities.ComparePassword(hashedInput, Crypto.CryptoConstants.Hash);

        Crypto.CryptoUtilities.ClearMemory(Crypto.CryptoConstants.Hash, hashedInput);

        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

        switch (loginSuccessful)
        {
            case true:
                await HandleLogin();
                break;
            case false:
                HandleFailedLogin();
                break;
        }
    }


    private void UserDoesNotExist()
    {
        EnableUi();
        Crypto.CryptoUtilities.ClearMemory(_passwordArray ?? Array.Empty<char>());
        _isAnimating = false;
        outputLbl.ForeColor = Color.WhiteSmoke;
        outputLbl.Text = @"Idle...";
        throw new ArgumentException("Username does not exist.", nameof(userNameTxt));
    }

    /// <summary>
    ///     Handles actions and processes for a successful login.
    /// </summary>
    private async Task HandleLogin()
    {
        if (!File.Exists(Authentication.GetUserFilePath(userNameTxt.Text)))
            return;

        if (File.Exists(Authentication.GetUserVault(userNameTxt.Text)))
        {
            var decryptedVault = await Crypto.DecryptFile(userNameTxt.Text,
                 _passwordArray, Authentication.GetUserVault(userNameTxt.Text));

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

            if (decryptedVault == Array.Empty<byte>())
                throw new ArgumentException("Value returned empty or null", nameof(decryptedVault));

            await File.WriteAllTextAsync(Authentication.GetUserVault(userNameTxt.Text),
               DataConversionHelpers.ByteArrayToString(decryptedVault));

            using var userVault = new Vault();
            userVault.LoadVault();

            var encryptedBytes = await Crypto.EncryptFile(userNameTxt.Text, _passwordArray,
                Authentication.GetUserVault(userNameTxt.Text));

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

            if (encryptedBytes == Array.Empty<byte>())
                throw new ArgumentException("Value returned empty or null.", nameof(encryptedBytes));

            await File.WriteAllTextAsync(Authentication.GetUserVault(userNameTxt.Text),
                DataConversionHelpers.ByteArrayToBase64String(encryptedBytes));

            outputLbl.ForeColor = Color.LimeGreen;
            outputLbl.Text = "Access granted";
            _isAnimating = false;
            UserLog.LogUser(Authentication.CurrentLoggedInUser);

            Crypto.CryptoConstants.SecurePassword = Crypto.ConversionMethods.ConvertCharArrayToSecureString(_passwordArray);

            MessageBox.Show("Login successful. Loading vault...", "Login success.",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            Crypto.CryptoUtilities.ClearMemory(passTxt.Text);
            passTxt.Clear();
            Crypto.CryptoUtilities.ClearMemory(_passwordArray);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
            Hide();
            userVault.ShowDialog();
            Close();
            return;
        }

        Crypto.CryptoConstants.SecurePassword = Crypto.ConversionMethods.ConvertCharArrayToSecureString(_passwordArray);
        if (Crypto.CryptoConstants.SecurePassword == null)
            throw new Exception("Value returned empty.");

        Crypto.CryptoUtilities.ClearMemory(Crypto.CryptoConstants.Hash);

        outputLbl.ForeColor = Color.LimeGreen;
        outputLbl.Text = "Access granted";
        _isAnimating = false;
        UserLog.LogUser(Authentication.CurrentLoggedInUser);

        MessageBox.Show("Login successful. Loading vault...", "Login success.",
            MessageBoxButtons.OK, MessageBoxIcon.Information);

        Crypto.CryptoUtilities.ClearMemory(passTxt.Text);
        passTxt.Clear();
        Crypto.CryptoUtilities.ClearMemory(_passwordArray);

        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

        Hide();
        using var blankVault = new Vault();
        blankVault.ShowDialog();
        Close();
    }


    private void HandleFailedLogin()
    {
        Crypto.CryptoUtilities.ClearMemory(_passwordArray);
        _isAnimating = false;
        EnableUi();
        outputLbl.ForeColor = Color.Red;
        outputLbl.Text = @"Login failed.";
        MessageBox.Show(@"Log in failed! Please recheck your login credentials and try again.", @"Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
        passTxt.Clear();
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
            for (var i = 0; i < 4; i++)
            {
                outputLbl.Text += @".";
                await Task.Delay(400);
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

        using (var p = Process.GetCurrentProcess())
        {
            p.PriorityClass = ProcessPriorityClass.Normal;
        }

        userNameTxt.Text = Settings.Default.userName;
        rememberMeCheckBox.Checked = true;
    }

    private async void Login_Shown(object sender, EventArgs e)
    {
     //  await Checks();
    }
}