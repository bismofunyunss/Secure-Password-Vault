using System.Text;

namespace Secure_Password_Vault;

public partial class RegisterAccount : Form
{
    private static bool _isAnimating;

    public RegisterAccount()
    {
        InitializeComponent();
    }

    private static bool CheckPasswordValidity(char[] password, char[] password2)
    {
        if (password.Length is < 8 or > 64)
            return false;

        if (!password.Any(char.IsUpper) || !password.Any(char.IsLower) || !password.Any(char.IsDigit))
            return false;

        if (password.Any(char.IsWhiteSpace) || password2.Any(char.IsWhiteSpace) || !password.SequenceEqual(password2))
            return false;

        return password.Any(char.IsSymbol) || password.Any(char.IsPunctuation);
    }

    private async void CreateAccount()
    {
        var userName = userTxt.Text;
        var passArray = new char[passTxt.Text.Length];
        passTxt.Text.CopyTo(0, passArray, 0, passTxt.Text.Length);

        var confirmPassArray = new char[confirmPassTxt.Text.Length];
        confirmPassTxt.Text.CopyTo(0, confirmPassArray, 0, confirmPassArray.Length);

        var userDirectory = CreateDirectoryIfNotExists(Path.Combine("Password Vault", "Users", userName));
        var userFile = Path.Combine(userDirectory, $"{userName}.user");
        var userInfo = Path.Combine(userDirectory, $"{userName}.info");

        var userExists = Authentication.UserExists(userName);

        try
        {
            if (!userExists)
            {
                StartAnimation();
                ValidateUsernameAndPassword(userName, passArray, confirmPassArray);

                var userId = Guid.NewGuid().ToString();
                Crypto.Salt = Crypto.RndByteSized(Crypto.SaltSize);
                Crypto.Iv = Crypto.RndByteSized(Crypto.IvBit / 8);
                var hashedPassword = await Crypto.HashAsync(passArray, Crypto.Salt);
                if (hashedPassword == null)
                    throw new ArgumentException(@"Value was null or empty.", nameof(hashedPassword));

                Crypto.Hash = hashedPassword;
                var saltString = DataConversionHelpers.ByteArrayToBase64String(Crypto.Salt);

                await File.WriteAllTextAsync(userFile,
                    $"User:\n{userName}\nUserID:\n{userId}\nSalt:\n{saltString}\nHash:\n{DataConversionHelpers.ByteArrayToHexString(hashedPassword)}\n");

                var textString = await File.ReadAllTextAsync(userFile);
                var textBytes = DataConversionHelpers.StringToByteArray(textString);


                var derivedKey = await Crypto.DeriveAsync(passArray, Crypto.Salt);
                if (derivedKey == null)
                    throw new ArgumentException(@"Value returned null or empty.", nameof(derivedKey));
                var keyBytes = Encoding.UTF8.GetBytes(derivedKey);

                var encrypted = Crypto.Encrypt(textBytes, keyBytes);
                if (encrypted == null)
                    throw new ArgumentException(@"Value returned null or empty.", nameof(encrypted));

                await File.WriteAllTextAsync(userFile, DataConversionHelpers.ByteArrayToBase64String(encrypted));
                await File.AppendAllTextAsync(userInfo,
                    DataConversionHelpers.ByteArrayToBase64String(Crypto.Salt) +
                    DataConversionHelpers.ByteArrayToBase64String(Crypto.Iv));

                Array.Clear(keyBytes, 0, keyBytes.Length);
                Array.Clear(derivedKey, 0, derivedKey.Length);
                Array.Clear(hashedPassword, 0, hashedPassword.Length);

                var dialogResult = MessageBox.Show(
                    @"Registration successful! Make sure you do NOT forget your password or you will lose access " +
                    @"to all of your files.", @"Registration Complete", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                if (dialogResult != DialogResult.OK)
                    return;
                Hide();
                using Login form = new();
                form.ShowDialog();
                Close();
            }
            else
            {
                throw new ArgumentException(@"Username already exists", userTxt.Text);
            }
        }
        catch (ArgumentException ex)
        {
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
            throw new ArgumentException("Password must contain between 8 and 64 characters. " +
                                        "It also must include:\n1.) At least one uppercase letter.\n2.) At least one lowercase letter.\n" +
                                        "3.) At least one number.\n4.) At least one special character.\n5.) Must not contain any spaces.\n" +
                                        "6.) Both passwords must match.\n", nameof(passTxt));
    }

    private static string CreateDirectoryIfNotExists(string directoryPath)
    {
        var fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            directoryPath);
        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);

        return fullPath;
    }

    private void createAccountBtn_Click(object sender, EventArgs e)
    {
        CreateAccount();
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
}