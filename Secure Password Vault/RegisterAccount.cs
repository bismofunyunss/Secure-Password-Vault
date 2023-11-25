using System.Diagnostics;

namespace Secure_Password_Vault;

public partial class RegisterAccount : Form
{
    private static bool _isAnimating;

    public RegisterAccount()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Checks the validity of a password based on specified criteria.
    /// </summary>
    /// <param name="password">The first password to be checked, as an IReadOnlyCollection of characters.</param>
    /// <param name="password2">The second password to be checked, as an array of characters.</param>
    /// <returns>
    /// True if the passwords meet the validity criteria, otherwise false.
    /// </returns>
    private static bool CheckPasswordValidity(IReadOnlyCollection<char> password, char[] password2)
    {
        // Check password length: must be between 16 and 64 characters (inclusive).
        if (password.Count is < 16 or > 64)
            return false;

        // Check for at least one uppercase letter, one lowercase letter, and one digit.
        if (!password.Any(char.IsUpper) || !password.Any(char.IsLower) || !password.Any(char.IsDigit))
            return false;

        // Check for the absence of whitespaces in both passwords and if they are equal.
        if (password.Any(char.IsWhiteSpace) || password2.Any(char.IsWhiteSpace) || !password.SequenceEqual(password2))
            return false;

        // Check for at least one symbol or punctuation character.
        return password.Any(char.IsSymbol) || password.Any(char.IsPunctuation);
    }

    /// <summary>
    /// Asynchronously creates a user account with specified username and password.
    /// </summary>
    private async Task CreateAccountAsync()
    {
        // Uncheck the showPasswordCheckBox if it is checked.
        if (showPasswordCheckBox.Checked)
            showPasswordCheckBox.Checked = false;

        // Retrieve the username from the userTxt TextBox.
        var userName = userTxt.Text;

        // Generate an array of characters for the user's password.
        var passArray = SetArray();

        // Generate an array of characters for confirming the user's password.
        var confirmPassArray = SetConfirmationArray();

        // Create or retrieve the user's directory.
        var userDirectory = CreateDirectoryIfNotExists(Path.Combine("Password Vault", "Users", userName));

        // Construct paths for the user's data files.
        var userFile = Path.Combine(userDirectory, $"{userName}.user");
        var userSalt = Path.Combine(userDirectory, $"{userName}.salt");

        // Check if the user already exists.
        var userExists = Authentication.UserExists(userName);

        try
        {
            if (!userExists)
            {
                // Disable the UI during the registration process.
                DisableUi();

                // Perform asynchronous user registration.
                await RegisterAsync(userName, passArray, confirmPassArray, userFile, userSalt);
            }
            else
            {
                // Throw an exception if the username already exists.
                throw new ArgumentException("Username already exists", userTxt.Text);
            }
        }
        catch (Exception ex)
        {
            // Enable the UI after an exception occurs.
            EnableUi();

            // Update UI elements and log the error.
            outputLbl.ForeColor = Color.WhiteSmoke;
            _isAnimating = false;
            ErrorLogging.ErrorLog(ex);

            // Show an error message to the user.
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // Update the output label text.
            outputLbl.Text = "Idle...";
        }
    }

    private void DisableUi()
    {
        showPasswordCheckBox.Enabled = false;
        passTxt.Enabled = false;
        confirmPassTxt.Enabled = false;
        userTxt.Enabled = false;
        createAccountBtn.Enabled = false;
        cancelBtn.Enabled = false;
    }

    private void EnableUi()
    {
        showPasswordCheckBox.Enabled = true;
        userTxt.Enabled = true;
        createAccountBtn.Enabled = true;
        cancelBtn.Enabled = true;
        passTxt.Enabled = true;
        confirmPassTxt.Enabled = true;
    }

    /// <summary>
    /// Converts the password input from the passTxt TextBox to a character array.
    /// </summary>
    /// <returns>
    /// A character array representing the password.
    /// </returns>
    private char[] SetArray()
    {
        // Get the length of the password from the passTxt TextBox.
        var buffer = passTxt.Text.Length;

        // Create a character array with the same length as the password.
        var passArray = new char[buffer];

        // Copy the characters from passTxt to the passArray.
        passTxt.Text.CopyTo(0, passArray, 0, buffer);

        // Return the character array representing the password.
        return passArray;
    }

    private char[] SetConfirmationArray()
    {
        var confirmPassArray = new char[confirmPassTxt.Text.Length];
        confirmPassTxt.Text.CopyTo(0, confirmPassArray, 0, confirmPassArray.Length);

        return confirmPassArray;
    }

    /// <summary>
    /// Asynchronously registers a user with the specified username and password,
    /// encrypts user data, and displays relevant messages to the user.
    /// </summary>
    /// <param name="username">The username of the user to be registered.</param>
    /// <param name="password">The password of the user to be registered.</param>
    /// <param name="confirmPassword">The confirmation of the user's password.</param>
    /// <param name="userFile">The path to the user data file.</param>
    /// <param name="userSalt">The path to the user's salt file.</param>
    private async Task RegisterAsync(string username, char[] password, char[] confirmPassword, string userFile,
        string userSalt)
    {
        // Set the priority class of the current process to AboveNormal.
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;

        // Start an animation (assuming there's a method named StartAnimation).
        StartAnimation();

        // Validate the username and password.
        ValidateUsernameAndPassword(username, password, confirmPassword);

        // Generate a random salt and hash the password using Argon2id.
        var salt = Crypto.RndByteSized(Crypto.CryptoConstants.SaltSize);
        var hashedPassword = await Crypto.Argon2Id(password, salt, 24);

        // Perform aggressive garbage collection.
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

        // Throw an exception if the hashed password is null.
        if (hashedPassword == null)
            throw new ArgumentException("Value was null or empty.", nameof(hashedPassword));

        // Convert the salt to a base64-encoded string.
        var saltString = DataConversionHelpers.ByteArrayToBase64String(salt);

        // Set the global hash value to the hashed password.
        Crypto.CryptoConstants.Hash = hashedPassword;

        // Write user information to the user data file.
        await File.WriteAllTextAsync(userFile,
            $"User:\n{username}\nHash:\n{DataConversionHelpers.ByteArrayToHexString(hashedPassword)}");

        // Write the salt to the user's salt file.
        await File.WriteAllTextAsync(userSalt, saltString);

        // Encrypt user data and write it to the user data file.
        var encrypted = await Crypto.EncryptFile(username, password, Authentication.GetUserFilePath(username));

        // Throw an exception if the encrypted value is null or empty.
        if (encrypted == Array.Empty<byte>())
            throw new ArgumentException("Value returned null or empty.", nameof(encrypted));

        await File.WriteAllTextAsync(userFile, DataConversionHelpers.ByteArrayToBase64String(encrypted));

        // Perform aggressive garbage collection.
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

        // Clear sensitive data from memory.
        Array.Clear(hashedPassword, 0, hashedPassword.Length);
        Array.Clear(password);
        Array.Clear(confirmPassword);

        // Update UI elements and display success message to the user.
        outputLbl.Text = "Account created";
        outputLbl.ForeColor = Color.LimeGreen;
        _isAnimating = false;

        // Show a success message to the user.
        MessageBox.Show("Registration successful! Make sure you do NOT forget your password or you will lose access " +
                        "to all of your files.", "Registration Complete", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

        // Hide the current form, show the login form, and close the current form.
        Hide();
        using Login form = new();
        form.ShowDialog();
        Close();
    }


    /// <summary>
    /// Validates the provided username and password for registration.
    /// </summary>
    /// <param name="userName">The username to be validated.</param>
    /// <param name="password">The password to be validated.</param>
    /// <param name="password2">The confirmation password to be validated.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if the username or password does not meet the specified criteria.
    /// </exception>
    private static void ValidateUsernameAndPassword(string userName, char[] password, char[] password2)
    {
        // Validate the username for legal characters.
        if (!userName.All(c => char.IsLetterOrDigit(c) || c == '_' || c == ' '))
            throw new ArgumentException(
                "Value contains illegal characters. Valid characters are letters, digits, underscores, and spaces.",
                nameof(userName));

        // Validate the length of the username.
        if (string.IsNullOrEmpty(userName) || userName.Length > 20)
            throw new ArgumentException("Invalid username.", nameof(userName));

        // Validate that the password is not empty.
        if (password == Array.Empty<char>())
            throw new ArgumentException("Invalid password.", nameof(password));

        // Validate the password using the CheckPasswordValidity method.
        if (!CheckPasswordValidity(password, password2))
            throw new ArgumentException(
                "Password must contain between 16 and 64 characters. It also must include:" +
                " 1.) At least one uppercase letter." +
                " 2.) At least one lowercase letter." +
                " 3.) At least one number." +
                " 4.) At least one special character." +
                " 5.) Must not contain any spaces." +
                " 6.) Both passwords must match.",
                nameof(passTxt));
    }


    private static string CreateDirectoryIfNotExists(string directoryPath)
    {
        var fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            directoryPath);
        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);

        return fullPath;
    }

    private async void createAccountBtn_Click(object sender, EventArgs e)
    {
        MessageBox.Show(
            @"Do NOT close the program while loading. This may cause corrupted data that is NOT recoverable.", @"Info",
            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        await CreateAccountAsync();
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
            outputLbl.Text = @"Creating account";

            // Add animated periods
            for (var i = 0; i < 4; i++)
            {
                outputLbl.Text += @".";
                await Task.Delay(400); // Delay between each period
            }
        }
    }

    private void cancelBtn_Click(object sender, EventArgs e)
    {
        Hide();
        using Login form = new();
        form.ShowDialog();
        Close();
    }

    private void showPasswordCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        passTxt.UseSystemPasswordChar = !showPasswordCheckBox.Checked;
        confirmPassTxt.UseSystemPasswordChar = !showPasswordCheckBox.Checked;
    }
}