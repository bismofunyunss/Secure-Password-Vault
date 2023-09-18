namespace Secure_Password_Vault;

public partial class Login : Form
{
    private static bool _isAnimating;

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
        var buffer = passTxt.Text.Length;
        var passwordArray = new char[buffer];
        passTxt.Text.CopyTo(0, passwordArray, 0, buffer);

        var userExists = Authentication.UserExists(userNameTxt.Text);

        try
        {
            if (userNameTxt.Text == string.Empty)
                throw new ArgumentException(@"Value was null or empty.", nameof(userNameTxt));
            if (passwordArray.Length == 0)
                throw new ArgumentException(@"Value was null or empty.", nameof(passwordArray));
            switch (userExists)
            {
                case true:
                {
                    StartAnimation();
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
                    var decryptedBytes = await Crypto.DecryptUserFiles(userNameTxt.Text, passwordArray, 
                        Authentication.GetUserFilePath(userNameTxt.Text));

                    if (decryptedBytes != null)
                    {
                        var decryptedText = DataConversionHelpers.ByteArrayToString(decryptedBytes);
                        await File.WriteAllTextAsync(Authentication.GetUserFilePath(userNameTxt.Text), decryptedText);
                        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
                        var hashedInput = await Task.Run(() => Crypto.HashAsync(passwordArray, Crypto.Salt));
                        if (hashedInput == null)
                            throw new ArgumentException(@"Hash value returned null.", nameof(hashedInput));

                        Authentication.GetUserInfo(userNameTxt.Text);

                        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

                        var loginSuccessful = await Task.Run(() => Crypto.ComparePassword(hashedInput));
                        var encryptedBytes = await Task.Run(() => Crypto.EncryptUserFiles(userNameTxt.Text, passwordArray,
                            Authentication.GetUserFilePath(userNameTxt.Text)));
                        if (encryptedBytes == null)
                            throw new ArgumentException(@"Value returned empty or null.", nameof(encryptedBytes));

                        Array.Clear(passwordArray, 0, passwordArray.Length);
                        Array.Clear(hashedInput, 0, hashedInput.Length);
                        if (Crypto.Hash != null)
                            Array.Clear(Crypto.Hash, 0, Crypto.Hash.Length);

                        await File.WriteAllTextAsync(Authentication.GetUserFilePath(userNameTxt.Text),
                            DataConversionHelpers.ByteArrayToBase64String(encryptedBytes));

                        switch (loginSuccessful)
                        {
                            case true:
                            {
                                outputLbl.ForeColor = Color.LimeGreen;
                                outputLbl.Text = @"Logged in.";
                                _isAnimating = false;
                                Authentication.CurrentLoggedInUser = userNameTxt.Text;
                                MessageBox.Show(@"Login successful. Loading vault...", @"Login success.", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                                Hide();
                                using Vault form = new();
                                form.ShowDialog();
                                Close();
                                break;
                            }
                            case false:
                                Array.Clear(passwordArray, 0, passwordArray.Length);
                                _isAnimating = false;
                                outputLbl.ForeColor = Color.DarkRed;
                                outputLbl.Text = @"Idle...";
                                MessageBox.Show(@"Log in failed! Please recheck your login credentials and try again.", @"Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                        }
                    }

                    break;
                }
                case false:
                    Array.Clear(passwordArray, 0, passwordArray.Length);
                    _isAnimating = false;
                    outputLbl.ForeColor = Color.DarkRed;
                    outputLbl.Text = @"Idle...";
                    MessageBox.Show(@"Username does not exist.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }
        catch (Exception ex)
        {
            Array.Clear(passwordArray, 0, passwordArray.Length);
            _isAnimating = false;
            outputLbl.ForeColor = Color.DarkRed;
            outputLbl.Text = @"Idle...";
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ErrorLogging.ErrorLog(ex);
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
            outputLbl.Text = @"Logging in";

            // Add animated periods
            for (var i = 0; i < 4; i++)
            {
                outputLbl.Text += @".";
                await Task.Delay(400); // Delay between each period
            }
        }
    }
}