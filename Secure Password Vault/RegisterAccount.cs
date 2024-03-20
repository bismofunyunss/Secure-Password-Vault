using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Secure_Password_Vault;

public partial class RegisterAccount : Form
{
    private static bool _isAnimating;
    public RegisterAccount()
    {
        InitializeComponent();
    }

    public static bool CheckPasswordValidity(IReadOnlyCollection<char> password, IReadOnlyCollection<char>? password2 = null)
    {
        if (password.Count is 24 or > 120)
            return false;

        if (!password.Any(char.IsUpper) || !password.Any(char.IsLower) || !password.Any(char.IsDigit))
            return false;

        if (password.Any(char.IsWhiteSpace) || (password2 != null && (password2.Any(char.IsWhiteSpace) || !password.SequenceEqual(password2))))
            return false;

        return password.Any(char.IsSymbol) || password.Any(char.IsPunctuation);
    }

    /// <summary>
    ///     Asynchronously creates a user account with specified username and password.
    /// </summary>
    private async Task CreateAccountAsync()
    {
        if (showPasswordCheckBox.Checked)
            showPasswordCheckBox.Checked = false;

        var userName = userTxt.Text;

        var password = SetArray();

        var confirmPassword = SetConfirmationArray();

        var userExists = Authentication.UserExists(userName);

        if (!userExists)
        {
            DisableUi();

            await RegisterAsync(userName, password, confirmPassword);

        }
        else
        {
            throw new ArgumentException("Username already exists.", nameof(userName));
        }
    }

    private void DisableUi()
    {
        foreach (Control c in RegisterBox.Controls)
        {
            if (c == userLbl || c == passLbl || c == confirmPassLbl || c == statusLbl || c == outputLbl)
                continue;
            c.Enabled = false;
        }
    }

    private void EnableUi()
    {
        foreach (Control c in RegisterBox.Controls)
            c.Enabled = true;
    }

    /// <summary>
    ///     Converts the password input from the passTxt TextBox to a character array.
    /// </summary>
    /// <returns>
    ///     A character array representing the password.
    /// </returns>
    private char[] SetArray()
    {
        var arrayPtr = Crypto.ConversionMethods.CreatePinnedCharArray(passTxt.Text);
        int len = passTxt.Text.Length;
        var charArray = Crypto.ConversionMethods.ConvertIntPtrToCharArray(arrayPtr, len);

        Marshal.FreeCoTaskMem(arrayPtr);
        arrayPtr = IntPtr.Zero;

        return charArray;
    }


    private char[] SetConfirmationArray()
    {
        var arrayPtr = Crypto.ConversionMethods.CreatePinnedCharArray(confirmPassTxt.Text);
        int len = confirmPassTxt.Text.Length;
        var charArray = Crypto.ConversionMethods.ConvertIntPtrToCharArray(arrayPtr, len);

        Marshal.FreeCoTaskMem(arrayPtr);
        arrayPtr = IntPtr.Zero;

        return charArray;
    }


    /// <summary>
    ///     Asynchronously registers a user with the specified username and password,
    ///     encrypts user data, and displays relevant messages to the user.
    /// </summary>
    /// <param name="username">The username of the user to be registered.</param>
    /// <param name="password">The password of the user to be registered.</param>
    /// <param name="confirmPassword">The confirmation of the user's password.</param>
    private async Task RegisterAsync(string username, char[] password, char[] confirmPassword)
    {
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;

        StartAnimation();

        ValidateUsernameAndPassword(username, ref password, ref confirmPassword);

        var userName = userTxt.Text;

        var userDirectory = CreateDirectoryIfNotExists(Path.Combine("Password Vault", "Users", userName));

        var userFile = Path.Combine(userDirectory, $"{userName}.user");
        var userSalt = Path.Combine(userDirectory, $"{userName}.salt");

        var salt = Crypto.CryptoUtilities.RndByteSized(Crypto.CryptoConstants.SaltSize);
        var hashedPassword = await Crypto.HashingMethods.Argon2Id(password, salt, 32);

        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

        if (hashedPassword == null)
            throw new ArgumentException("Value was null or empty.", nameof(hashedPassword));

        var saltString = DataConversionHelpers.ByteArrayToBase64String(salt);

        Crypto.CryptoConstants.Hash = hashedPassword;

        await File.WriteAllTextAsync(userFile,
           $"User:\n{username}\nHash:\n{DataConversionHelpers.ByteArrayToHexString(hashedPassword)}");

        await File.WriteAllTextAsync(userSalt, saltString);

        var encrypted = await Crypto.EncryptFile(username, password, Authentication.GetUserFilePath(username));

        if (encrypted == Array.Empty<byte>())
            throw new InvalidOperationException("Value returned null or empty.");

        await File.WriteAllTextAsync(userFile, DataConversionHelpers.ByteArrayToBase64String(encrypted));

        Crypto.CryptoUtilities.ClearMemory(hashedPassword);

        outputLbl.Text = "Account created";
        outputLbl.ForeColor = Color.LimeGreen;
        _isAnimating = false;

        MessageBox.Show("Registration successful! Make sure you do NOT forget your password or you will lose access " +
                        "to all of your files.", "Registration Complete", MessageBoxButtons.OK,
            MessageBoxIcon.Information);

        Crypto.CryptoUtilities.ClearMemory(passTxt.Text, confirmPassTxt.Text);
        passTxt.Clear();
        confirmPassTxt.Clear();
        Crypto.CryptoUtilities.ClearMemory(password, confirmPassword);

        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

        Hide();
        using Login form = new();
        form.ShowDialog();
        Close();
    }


    /// <summary>
    ///     Validates the provided username and password for registration.
    /// </summary>
    /// <param name="userName">The username to be validated.</param>
    /// <param name="password">The password to be validated.</param>
    /// <param name="password2">The confirmation password to be validated.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown if the username or password does not meet the specified criteria.
    /// </exception>
    private static void ValidateUsernameAndPassword(string userName, ref char[] password, ref char[] password2)
    {
        if (!userName.All(c => char.IsLetterOrDigit(c) || c == '_' || c == ' '))
            throw new ArgumentException(
                "Value contains illegal characters. Valid characters are letters, digits, underscores, and spaces.",
                nameof(userName));

        if (string.IsNullOrEmpty(userName) || userName.Length > 20)
            throw new ArgumentException("Invalid username.", nameof(userName));

        if (password == Array.Empty<char>())
            throw new ArgumentException("Invalid password.", nameof(password));

        if (!CheckPasswordValidity(password, password2))
            throw new ArgumentException(
                "Password must contain between 24 and 120 characters. It also must include:" +
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

    private async void CreateAccountBtn_Click(object sender, EventArgs e)
    {
        MessageBox.Show(
            @"Do NOT close the program while loading. This may cause corrupted data that is NOT recoverable.", @"Info",
            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        try
        {
            await CreateAccountAsync();
        }
        catch (Exception ex)
        {
            EnableUi();

            outputLbl.ForeColor = Color.WhiteSmoke;
            _isAnimating = false;
            ErrorLogging.ErrorLog(ex);

            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            outputLbl.Text = "Idle...";
        }
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

            for (var i = 0; i < 4; i++)
            {
                outputLbl.Text += @".";
                await Task.Delay(400);
            }
        }
    }

    private void CancelBtn_Click(object sender, EventArgs e)
    {
        Hide();
        using Login form = new();
        form.ShowDialog();
        Close();
    }

    private void ShowPasswordCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        passTxt.UseSystemPasswordChar = !showPasswordCheckBox.Checked;
        confirmPassTxt.UseSystemPasswordChar = !showPasswordCheckBox.Checked;
    }
}