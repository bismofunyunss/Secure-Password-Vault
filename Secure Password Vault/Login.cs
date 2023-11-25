using System.Diagnostics;
using System.Security;

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

    /// <summary>
    /// Handles the click event for the logInBtn, attempts user login, and performs necessary UI updates.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    private async void logInBtn_Click(object sender, EventArgs e)
    {
        // Set the priority class of the current process to AboveNormal.
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;

        // Save the username in settings based on the rememberMeCheckBox state.
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

        // Parse the attempts remaining from the AttemptsNumber TextBox.
        _attemptsRemaining = int.Parse(AttemptsNumber.Text);

        // Check if the attempts remaining is zero and throw an exception if true.
        if (_attemptsRemaining == 0)
            throw new Exception("Input was not in the correct format.");

        // Display an error message if no attempts remaining and return.
        if (_attemptsRemaining == 0)
        {
            MessageBox.Show("No attempts remaining. Please restart the program and try again.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        try
        {
            // Throw an exception if the username is null or empty.
            if (userNameTxt.Text == string.Empty)
                throw new ArgumentException("Value was null or empty.", nameof(userNameTxt));

            // Set the passwordArray using the SetArray method.
            _passwordArray = SetArray();

            // Throw an exception if the passwordArray is null or empty.
            if (_passwordArray.Length == 0)
                throw new ArgumentException("Value was null or empty.", nameof(_passwordArray));

            // Display an information message to the user.
            MessageBox.Show("Do NOT close the program while loading. This may cause corrupted data that is NOT recoverable.",
                "Info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            // Disable the UI during login processing.
            DisableUi();

            // Check if the user exists.
            var userExists = Authentication.UserExists(userNameTxt.Text);

            // Call the asynchronous login method.
            await ProcessLoginAsync(userExists);
        }
        catch (Exception ex)
        {
            // Clear sensitive data from memory.
            Array.Clear(_passwordArray, 0, _passwordArray.Length);

            // Log the error.
            ErrorLogging.ErrorLog(ex);

            // Update UI elements and display a login failure message.
            _isAnimating = false;
            outputLbl.Text = "Login failed.";
            outputLbl.ForeColor = Color.Red;
            MessageBox.Show("Please recheck your credentials and try again.", "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            // Decrement the attempts remaining count and reset UI elements.
            _attemptsRemaining--;
            outputLbl.ForeColor = Color.WhiteSmoke;
            outputLbl.Text = "Idle...";
            EnableUi();
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
            throw new ArgumentNullException(nameof(charArray), @"Value was empty.");

        var secureString = new SecureString();

        try
        {
            foreach (var c in charArray) secureString.AppendChar(c);

            // Make sure the SecureString is read-only to enhance security.
            secureString.MakeReadOnly();
        }
        catch (Exception ex)
        {
            secureString.Dispose(); // Dispose if there is an exception to avoid memory leaks.
            ErrorLogging.ErrorLog(ex);
            throw;
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

    /// <summary>
    /// Asynchronously initiates the login process by performing decryption, password hashing,
    /// and handling login success or failure accordingly.
    /// </summary>
    private async Task StartLoginProcessAsync()
    {
        // Start an animation (assuming there's a method named StartAnimation).
        StartAnimation();

        // Uncheck the showPasswordCheckBox if it is checked.
        if (showPasswordCheckBox.Checked)
            showPasswordCheckBox.Checked = false;

        // Set the passwordArray using the SetArray method.
        _passwordArray = SetArray();

        // Decrypt user data using the provided username and password.
        var decryptedBytes = await Crypto.DecryptFile(userNameTxt.Text, _passwordArray,
            Authentication.GetUserFilePath(userNameTxt.Text));

        // Throw an exception if the decryptedBytes is empty or null.
        if (decryptedBytes == Array.Empty<byte>())
            throw new Exception("Value returned empty or null.");

        // Perform aggressive garbage collection.
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

        // Convert decryptedBytes to text and write it back to the user data file.
        var decryptedText = DataConversionHelpers.ByteArrayToString(decryptedBytes);
        await File.WriteAllTextAsync(Authentication.GetUserFilePath(userNameTxt.Text), decryptedText);

        // Retrieve the user's salt.
        var salt = await Authentication.GetUserSaltAsync(userNameTxt.Text);

        // Set the passwordArray using the SetArray method.
        _passwordArray = SetArray();

        // Retrieve user information.
        await Authentication.GetUserInfo(userNameTxt.Text);

        // Hash the input password using Argon2id.
        var hashedInput = await Crypto.Argon2Id(_passwordArray, salt, 24);

        // Perform aggressive garbage collection.
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

        // Throw an exception if the hashedInput is null.
        if (hashedInput == null)
            throw new ArgumentException("Hash value returned null.", nameof(hashedInput));

        // Retrieve user information again.
        await Authentication.GetUserInfo(userNameTxt.Text);

        // Compare the hashed input password with the stored hash.
        var loginSuccessful = await Crypto.ComparePassword(hashedInput, Crypto.CryptoConstants.Hash);

        // Perform aggressive garbage collection.
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

        // Clear sensitive data from memory.
        Array.Clear(hashedInput, 0, hashedInput.Length);
        if (Crypto.CryptoConstants.Hash != null)
            Array.Clear(Crypto.CryptoConstants.Hash, 0, Crypto.CryptoConstants.Hash.Length);

        // Handle the login success or failure based on the result.
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


    private void UserDoesNotExist()
    {
        EnableUi();
        Array.Clear(_passwordArray, 0, _passwordArray.Length);
        _isAnimating = false;
        outputLbl.ForeColor = Color.WhiteSmoke;
        MessageBox.Show(@"Username does not exist.", @"Error", MessageBoxButtons.OK,
            MessageBoxIcon.Error);
        outputLbl.Text = @"Idle...";
    }

    /// <summary>
    /// Handles actions and processes for a successful login.
    /// </summary>
    private async void HandleLogin()
    {
        // Check if the user data file exists, return if not.
        if (!File.Exists(Authentication.GetUserFilePath(userNameTxt.Text)))
            return;

        // Set the currently logged-in user.
        Authentication.CurrentLoggedInUser = userNameTxt.Text;

        // Set the passwordArray using the SetArray method.
        _passwordArray = SetArray();

        // Encrypt user information and write it back to the user data file.
        var encryptedUserInfo = await Crypto.EncryptFile(userNameTxt.Text, _passwordArray,
            Authentication.GetUserFilePath(userNameTxt.Text));

        // Perform aggressive garbage collection.
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

        // Throw an exception if the encryptedUserInfo is empty or null.
        if (encryptedUserInfo == Array.Empty<byte>())
            throw new ArgumentException("Value returned empty or null.", nameof(encryptedUserInfo));

        // Clear the hash value from memory if it is not null.
        if (Crypto.CryptoConstants.Hash != null)
            Array.Clear(Crypto.CryptoConstants.Hash, 0, Crypto.CryptoConstants.Hash.Length);

        // Write the encrypted user information back to the user data file.
        await File.WriteAllTextAsync(Authentication.GetUserFilePath(userNameTxt.Text),
            DataConversionHelpers.ByteArrayToBase64String(encryptedUserInfo));

        // Check if the user vault file exists.
        if (File.Exists(Authentication.GetUserVault(userNameTxt.Text)))
        {
            // Set the passwordArray using the SetArray method.
            _passwordArray = SetArray();

            // Decrypt the user vault.
            var decryptedVault = await Crypto.DecryptFile(userNameTxt.Text,
                _passwordArray, Authentication.GetUserVault(userNameTxt.Text));

            // Perform aggressive garbage collection.
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

            // Throw an exception if the decryptedVault is empty or null.
            if (decryptedVault == Array.Empty<byte>())
                throw new ArgumentException("Value returned empty or null", nameof(decryptedVault));

            // Write the decrypted vault contents back to the user vault file.
            await File.WriteAllTextAsync(Authentication.GetUserVault(userNameTxt.Text),
                DataConversionHelpers.ByteArrayToString(decryptedVault));

            // Create and load the Vault form.
            using var userVault = new Vault();
            userVault.LoadVault();

            // Set the passwordArray using the SetArray method.
            _passwordArray = SetArray();

            // Encrypt the user vault and write it back to the user vault file.
            var encryptedBytes = await Crypto.EncryptFile(userNameTxt.Text, _passwordArray,
                Authentication.GetUserVault(userNameTxt.Text));

            // Perform aggressive garbage collection.
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

            // Throw an exception if the encryptedBytes is empty or null.
            if (encryptedBytes == Array.Empty<byte>())
                throw new ArgumentException("Value returned empty or null.", nameof(encryptedBytes));

            // Clear the hash value from memory if it is not null.
            if (Crypto.CryptoConstants.Hash != null)
                Array.Clear(Crypto.CryptoConstants.Hash, 0, Crypto.CryptoConstants.Hash.Length);

            // Write the encrypted user vault back to the user vault file.
            await File.WriteAllTextAsync(Authentication.GetUserVault(userNameTxt.Text),
                DataConversionHelpers.ByteArrayToBase64String(encryptedBytes));

            // Update UI elements for a successful login.
            outputLbl.ForeColor = Color.LimeGreen;
            outputLbl.Text = "Access granted";
            _isAnimating = false;
            UserLog.LogUser(Authentication.CurrentLoggedInUser);

            // Set the secure password and clear sensitive data from memory.
            _passwordArray = SetArray();
            SecurePassword = ConvertCharArrayToSecureString(_passwordArray);
            Array.Clear(_passwordArray, 0, _passwordArray.Length);

            // Show a message and hide the current form, then show the Vault form.
            MessageBox.Show("Login successful. Loading vault...", "Login success.",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            Hide();
            userVault.ShowDialog();
            Close();
            return;
        }

        // Set the passwordArray using the SetArray method.
        _passwordArray = SetArray();

        // Set the secure password and throw an exception if it is null.
        SecurePassword = ConvertCharArrayToSecureString(_passwordArray);
        if (SecurePassword == null)
            throw new Exception("Value returned empty.");

        // Clear sensitive data from memory.
        Array.Clear(_passwordArray, 0, _passwordArray.Length);

        // Update UI elements for a successful login.
        outputLbl.ForeColor = Color.LimeGreen;
        outputLbl.Text = "Access granted";
        _isAnimating = false;
        UserLog.LogUser(Authentication.CurrentLoggedInUser);

        // Show a message and hide the current form, then show a blank Vault form.
        MessageBox.Show("Login successful. Loading vault...", "Login success.",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        Hide();
        using var blankVault = new Vault();
        blankVault.ShowDialog();
        Close();
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