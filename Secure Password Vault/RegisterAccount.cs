using System.Diagnostics;

namespace Secure_Password_Vault;

public partial class RegisterAccount : Form
{
    private static bool _isAnimating;

    public RegisterAccount()
    {
        InitializeComponent();
    }

    private static bool CheckPasswordValidity(IReadOnlyCollection<char> password, char[] password2)
    {
        if (password.Count is < 16 or > 64)
            return false;

        if (!password.Any(char.IsUpper) || !password.Any(char.IsLower) || !password.Any(char.IsDigit))
            return false;

        if (password.Any(char.IsWhiteSpace) || password2.Any(char.IsWhiteSpace) || !password.SequenceEqual(password2))
            return false;

        return password.Any(char.IsSymbol) || password.Any(char.IsPunctuation);
    }

    private async Task CreateAccountAsync()
    {
        if (showPasswordCheckBox.Checked)
            showPasswordCheckBox.Checked = false;

        var userName = userTxt.Text;
        var passArray = SetArray();

        var confirmPassArray = SetConfirmationArray();

        var userDirectory = CreateDirectoryIfNotExists(Path.Combine("Password Vault", "Users", userName));
        var userFile = Path.Combine(userDirectory, $"{userName}.user");
        var userSalt = Path.Combine(userDirectory, $"{userName}.salt");
        var userSalt2 = Path.Combine(userDirectory, $"{userName}-Salt2.salt");
        var userSalt3 = Path.Combine(userDirectory, $"{userName}-Salt3.salt");
        var userSalt4 = Path.Combine(userDirectory, $"{userName}-Salt4.salt");
        var userExists = Authentication.UserExists(userName);

        try
        {
            if (!userExists)
            {
                DisableUi();
                await RegisterAsync(userName, passArray, confirmPassArray, userFile, userSalt, userSalt2, userSalt3, userSalt4);
            }
            else
            {
                throw new ArgumentException(@"Username already exists", userTxt.Text);
            }
        }
        catch (Exception ex)
        {
            EnableUi();
            outputLbl.ForeColor = Color.WhiteSmoke;
            outputLbl.Text = @"Idle...";
            _isAnimating = false;
            ErrorLogging.ErrorLog(ex);
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

    private char[] SetArray()
    {
        var buffer = passTxt.Text.Length;
        var passArray = new char[buffer];
        passTxt.Text.CopyTo(0, passArray, 0, buffer);

        return passArray;
    }

    private char[] SetConfirmationArray()
    {
        var confirmPassArray = new char[confirmPassTxt.Text.Length];
        confirmPassTxt.Text.CopyTo(0, confirmPassArray, 0, confirmPassArray.Length);

        return confirmPassArray;
    }

    private async Task RegisterAsync(string username, char[] password, char[] confirmPassword, string userFile,
        string userSalt, string userSalt2, string userSalt3, string userSalt4)
    {
        try
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;

            StartAnimation();

            ValidateUsernameAndPassword(username, password, confirmPassword);

            var salt = Crypto.RndByteSized(Crypto.SaltSize);
            var salt2 = Crypto.RndByteSized(Crypto.SaltSize);
            var salt3 = Crypto.RndByteSized(Crypto.SaltSize);
            var salt4 = Crypto.RndByteSized(Crypto.SaltSize);
            var hashedPassword = await Crypto.HashAsync(password, salt);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
            if (hashedPassword == null)
                throw new ArgumentException(@"Value was null or empty.", nameof(hashedPassword));

            var saltString = DataConversionHelpers.ByteArrayToBase64String(salt);
            var saltString2 = DataConversionHelpers.ByteArrayToBase64String(salt2);
            var saltstring3 = DataConversionHelpers.ByteArrayToBase64String(salt3);
            var saltString4 = DataConversionHelpers.ByteArrayToBase64String(salt4);

            Crypto.Hash = hashedPassword;
            await File.WriteAllTextAsync(userFile,
                $"User:\n{username}\nHash:\n{DataConversionHelpers.ByteArrayToHexString(hashedPassword)}");

            await File.WriteAllTextAsync(userSalt,
                saltString);
            await File.WriteAllTextAsync(userSalt2, saltString2);
            await File.WriteAllTextAsync(userSalt3, saltstring3);
            await File.WriteAllTextAsync(userSalt4, saltString4);

            var encrypted = await Crypto.EncryptFile(username, password, Authentication.GetUserFilePath(username));
            if (encrypted == Array.Empty<byte>())
                throw new ArgumentException(@"Value returned null or empty.", nameof(encrypted));

            await File.WriteAllTextAsync(userFile, DataConversionHelpers.ByteArrayToBase64String(encrypted));

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

            Array.Clear(hashedPassword, 0, hashedPassword.Length);
            Array.Clear(password);
            Array.Clear(confirmPassword);
            outputLbl.Text = @"Account created";
            outputLbl.ForeColor = Color.LimeGreen;
            _isAnimating = false;
            MessageBox.Show(
                @"Registration successful! Make sure you do NOT forget your password or you will lose access " +
                @"to all of your files.", @"Registration Complete", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            Hide();
            using Login form = new();
            form.ShowDialog();
            Close();
        }
        catch (ArgumentException ex)
        {
            EnableUi();
            outputLbl.ForeColor = Color.WhiteSmoke;
            outputLbl.Text = @"Idle...";
            _isAnimating = false;
            ErrorLogging.ErrorLog(ex);
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            EnableUi();
            outputLbl.ForeColor = Color.WhiteSmoke;
            outputLbl.Text = @"Idle...";
            _isAnimating = false;
            ErrorLogging.ErrorLog(ex);
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static void ValidateUsernameAndPassword(string userName, char[] password, char[] password2)
    {
        if (!userName.All(c => char.IsLetterOrDigit(c) || c == '_' || c == ' '))
            throw new ArgumentException(
                @"Value contains illegal characters. Valid characters are letters, digits, underscores, and spaces.",
                nameof(userName));

        if (string.IsNullOrEmpty(userName) || userName.Length > 20)
            throw new ArgumentException(@"Invalid username.", nameof(userName));

        if (password == Array.Empty<char>())
            throw new ArgumentException(@"Invalid password.", nameof(password));

        if (!CheckPasswordValidity(password, password2))
            throw new ArgumentException(@"Password must contain between 16 and 64 characters." +
                                        @"It also must include: 1.) At least one uppercase letter." +
                                        @"2.) At least one lowercase letter." +
                                        @"3.) At least one number. " +
                                        @"4.) At least one special character." +
                                        @"5.) Must not contain any spaces.\n" +
                                        @"6.) Both passwords must match.", nameof(passTxt));
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