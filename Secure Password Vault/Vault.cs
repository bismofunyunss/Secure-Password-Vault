using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Secure_Password_Vault;

public partial class Vault : Form
{
    private static bool _isAnimating;
    private static char[] _passwordArray = Array.Empty<char>();
    private static string _loadedFile = string.Empty;
    private static string _loadedFileHash = string.Empty;
    private static bool _fileOpened;
    private static string _result = string.Empty;
    private static readonly ToolTip Tip = new();
    private const int MaxFileSize = 950_000_000;
    private static int _fileSize = 0;

    public Vault()
    {
        InitializeComponent();
    }

    private void addRowBtn_Click(object sender, EventArgs e)
    {
        PassVault.Rows.Add();
    }

    private static void SetArray()
    {
        _passwordArray = ConvertSecureStringToCharArray(Login.SecurePassword);
    }

    private void deleteRowBtn_Click(object sender, EventArgs e)
    {
        if (PassVault.SelectedRows.Count <= 0)
            return;
        var selectedRow = PassVault.SelectedRows[0].Index;

        PassVault.Rows.RemoveAt(selectedRow);
    }

    public static char[] ConvertSecureStringToCharArray(SecureString secureString)
    {
        if (secureString == null)
            throw new ArgumentNullException(nameof(secureString));

        var charArray = new char[secureString.Length];
        var unmanagedString = IntPtr.Zero;

        try
        {
            unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
            for (var i = 0; i < secureString.Length; i++)
                charArray[i] = (char)Marshal.ReadInt16(unmanagedString, i * 2);
        }
        finally
        {
            if (unmanagedString != IntPtr.Zero) Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
        }

        return charArray;
    }

    private async void saveVaultBtn_Click(object sender, EventArgs e)
    {
        try
        {
            MessageBox.Show(
                @"Do NOT close the program while loading. This may cause corrupted data that is NOT recoverable.",
                @"Info",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            StartAnimation();
            DisableUi();
            var filePath = Path.Combine("Password Vault", "Users",
                Authentication.GetUserVault(Authentication.CurrentLoggedInUser));

            await using (var sw = new StreamWriter(filePath))
            {
                sw.NewLine = null;
                sw.AutoFlush = true;
                foreach (DataGridViewRow row in PassVault.Rows)
                {
                    for (var i = 0; i < PassVault.Columns.Count; i++)
                    {
                        row.Cells[i].ValueType = typeof(char[]);
                        sw.Write(row.Cells[i].Value);
                        if (i < PassVault.Columns.Count - 1)
                            await sw.WriteAsync("\t"); // Use a tab character to separate columns
                    }

                    await sw.WriteLineAsync(); // Start a new line for each row
                }
            }

            _passwordArray = ConvertSecureStringToCharArray(Login.SecurePassword);

            var encryptedVault = await Crypto.EncryptFile(Authentication.CurrentLoggedInUser, _passwordArray,
                Authentication.GetUserVault(Authentication.CurrentLoggedInUser));
            var encryptedVaultString = DataConversionHelpers.ByteArrayToBase64String(encryptedVault);
            await File.WriteAllTextAsync(Authentication.GetUserVault(Authentication.CurrentLoggedInUser),
                encryptedVaultString);
            Array.Clear(_passwordArray, 0, _passwordArray.Length);


            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

            _isAnimating = false;
            outputLbl.Text = @"Vault saved successfully";
            outputLbl.ForeColor = Color.LimeGreen;
            MessageBox.Show(@"Vault saved successfully.", @"Save vault", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            outputLbl.Text = @"Idle...";
            outputLbl.ForeColor = Color.WhiteSmoke;
        }
        catch (Exception ex)
        {
            EnableUi();
            _isAnimating = false;
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ErrorLogging.ErrorLog(ex);
        }
    }

    public async void LoadVault()
    {
        try
        {
            var filePath = Authentication.GetUserVault(Authentication.CurrentLoggedInUser);
            if (File.Exists(filePath))
            {
                using var sr = new StreamReader(filePath);
                PassVault.Rows.Clear(); // Clear existing data in the DataGridView

                while (!sr.EndOfStream)
                {
                    var line = await sr.ReadLineAsync();
                    var values = line?.Split('\t'); // Split the line by tabs

                    if (IsBase64(line))
                        throw new InvalidOperationException("Invalid input text");

                    if (values is { Length: <= 0 }) continue;
                    // Add a new row to the DataGridView and populate it with values
                    var rowIndex = PassVault.Rows.Add();
                    if (values == null) continue;
                    for (var i = 0; i < values.Length; i++)
                        PassVault.Rows[rowIndex].Cells[i].Value = values[i];
                }
            }
            else
            {
                EnableUi();
                MessageBox.Show(@"Vault file does not exist.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            EnableUi();
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ErrorLogging.ErrorLog(ex);
        }
    }

    private static bool IsBase64(string? str)
    {
        return str != null && MyRegex().IsMatch(str) && str.Length % 4 == 0;
    }

    private void EnableUi()
    {
        saveVaultBtn.Enabled = true;
        deleteRowBtn.Enabled = true;
        addRowBtn.Enabled = true;
        PassVault.Enabled = true;
        ImportFileBtn.Enabled = true;
        ExportFileBtn.Enabled = true;
        EncryptBtn.Enabled = true;
        DecryptBtn.Enabled = true;
        hashimportfile.Enabled = true;
        calculatehashbtn.Enabled = true;
    }

    private void DisableUi()
    {
        saveVaultBtn.Enabled = false;
        deleteRowBtn.Enabled = false;
        addRowBtn.Enabled = false;
        PassVault.Enabled = false;
        ImportFileBtn.Enabled = false;
        ExportFileBtn.Enabled = false;
        EncryptBtn.Enabled = false;
        DecryptBtn.Enabled = false;
        hashimportfile.Enabled = false;
        calculatehashbtn.Enabled = false;
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
            outputLbl.Text = @"Saving vault";
            // Add animated periods
            for (var i = 0; i < 4; i++)
            {
                outputLbl.Text += @".";
                await Task.Delay(400); // Delay between each period
            }
        }
    }

    private async void ImportFileBtn_Click(object? sender, EventArgs e)
    {
        try
        {
            using var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = @"Txt files(*.txt) | *.txt";
            openFileDialog.FilterIndex = 1;
            openFileDialog.ShowHiddenFiles = true;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.InitialDirectory =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var selectedFileName = openFileDialog.FileName;
                var selectedExtension = Path.GetExtension(selectedFileName).ToLower();
                var fileInfo = new FileInfo(selectedFileName);
                _fileOpened = true;
                _loadedFile = selectedFileName;

                if (!string.IsNullOrEmpty(_result))
                {
                    await using var fs = new FileStream(selectedFileName, FileMode.OpenOrCreate, FileAccess.Read);
                    using var sr = new StreamReader(fs, Encoding.UTF8);
                    var result = await sr.ReadToEndAsync();


                    if (string.IsNullOrEmpty(result))
                        throw new IOException("Result was empty.");
                }


                if (string.IsNullOrEmpty(selectedFileName))
                    throw new IOException("Invalid path.");

                if (selectedExtension != ".txt")
                    throw new ArgumentException(@"Invalid file extension. Please select a text file.",
                        nameof(selectedFileName));

                if (fileInfo.Length > MaxFileSize)
                    throw new ArgumentException(@"File size is too large.", nameof(selectedFileName));

                _fileSize = (int)fileInfo.Length;

                var fileSize = fileInfo.Length.ToString("#,0");
                FileSizeNumLbl.Text = $@"{fileSize} bytes";
            }

            FileOutputLbl.Text = @"File opened.";
            FileOutputLbl.ForeColor = Color.LimeGreen;
            MessageBox.Show(@"File opened successfully.", @"Opened successfully", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            FileOutputLbl.Text = @"Idle...";
            FileOutputLbl.ForeColor = Color.WhiteSmoke;
            Array.Clear(_passwordArray, 0, _passwordArray.Length);
            _result = string.Empty;
        }
        catch (Exception ex)
        {
            FileSizeNumLbl.Text = @"0";
            FileOutputLbl.Text = @"Error loading file.";
            FileOutputLbl.ForeColor = Color.Red;
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            FileOutputLbl.Text = @"Idle...";
            FileOutputLbl.ForeColor = Color.WhiteSmoke;
            ErrorLogging.ErrorLog(ex);
        }
    }

    private async void ExportFileBtn_Click(object sender, EventArgs e)
    {
        try
        {
            if (!_fileOpened)
                throw new Exception(@"No file is opened.");

            using var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = @"Txt files(*.txt) | *.txt";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.ShowHiddenFiles = true;
            saveFileDialog.CheckFileExists = false;
            saveFileDialog.CheckPathExists = false;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.InitialDirectory =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var selectedFileName = saveFileDialog.FileName;

                if (!string.IsNullOrEmpty(_result))
                {
                    await using var fs = new FileStream(selectedFileName, FileMode.OpenOrCreate, FileAccess.Write);
                    await using var sw = new StreamWriter(fs, Encoding.UTF8);
                    await sw.WriteAsync(_result);
                }
            }

            FileOutputLbl.Text = @"File saved successfully.";
            FileOutputLbl.ForeColor = Color.LimeGreen;
            MessageBox.Show(@"File saved successfully.", @"Saved successfully", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            FileOutputLbl.Text = @"Idle...";
            FileOutputLbl.ForeColor = Color.WhiteSmoke;
            Array.Clear(_passwordArray, 0, _passwordArray.Length);
            _result = string.Empty;
        }
        catch (Exception ex)
        {
            FileOutputLbl.Text = @"Error saving file.";
            FileOutputLbl.ForeColor = Color.Red;
            ErrorLogging.ErrorLog(ex);
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            FileOutputLbl.Text = @"Idle...";
            FileOutputLbl.ForeColor = Color.WhiteSmoke;
        }
    }

    private async void EncryptBtn_Click(object sender, EventArgs e)
    {
        try
        {
            MessageBox.Show(
                @"Do NOT close the program while loading. This may cause corrupted data that is NOT recoverable.",
                @"Info",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            if (!_fileOpened)
                throw new ArgumentException(@"No file is opened.", nameof(_fileOpened));

            SetArray();
            StartAnimationEncryption();
            DisableUi();
            if (_loadedFile == string.Empty)
                return;

            var encryptedFile =
                await Crypto.EncryptFile(Authentication.CurrentLoggedInUser, _passwordArray, _loadedFile);

            if (encryptedFile == Array.Empty<byte>())
                throw new ArgumentException(@"Value returned empty.", nameof(encryptedFile));

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

            var str = DataConversionHelpers.ByteArrayToBase64String(encryptedFile);

            if (!string.IsNullOrEmpty(str))
                _result = str;

            EnableUi();
            _isAnimating = false;

            var size = (long)_result.Length;

            var fileSize = size.ToString("#,0");
            FileSizeNumLbl.Text = $@"{fileSize} bytes";

            FileOutputLbl.Text = @"File encrypted.";
            FileOutputLbl.ForeColor = Color.LimeGreen;

            MessageBox.Show(@"File was encrypted successfully.", @"Success", MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            FileOutputLbl.Text = @"Idle...";
            FileOutputLbl.ForeColor = Color.WhiteSmoke;
            Array.Clear(_passwordArray, 0, _passwordArray.Length);
        }
        catch (Exception ex)
        {
            EnableUi();
            _isAnimating = false;
            FileOutputLbl.Text = @"Error encrypting file.";
            FileOutputLbl.ForeColor = Color.Red;
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            FileOutputLbl.Text = @"Idle...";
            FileOutputLbl.ForeColor = Color.WhiteSmoke;
            Array.Clear(_passwordArray, 0, _passwordArray.Length);
            ErrorLogging.ErrorLog(ex);
        }
    }

    private async void DecryptBtn_Click(object sender, EventArgs e)
    {
        try
        {
            MessageBox.Show(
                @"Do NOT close the program while loading. This may cause corrupted data that is NOT recoverable.",
                @"Info",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            if (!_fileOpened)
                throw new ArgumentException(@"No file is opened.", nameof(_fileOpened));

            if (_fileSize > MaxFileSize)
                throw new ArgumentException(@"File size is too large.");

            SetArray();
            DisableUi();
            StartAnimationDecryption();
            if (_loadedFile == string.Empty)
                return;

            var decryptedFile =
                await Crypto.DecryptFile(Authentication.CurrentLoggedInUser, _passwordArray, _loadedFile);

            if (decryptedFile == Array.Empty<byte>())
                throw new ArgumentException(@"Value returned empty.", nameof(decryptedFile));

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

            var str = DataConversionHelpers.ByteArrayToString(decryptedFile);
            if (!string.IsNullOrEmpty(str))
                _result = str;

            EnableUi();
            _isAnimating = false;

            var size = (long)_result.Length;

            var fileSize = size.ToString("#,0");
            FileSizeNumLbl.Text = $@"{fileSize} bytes";

            FileOutputLbl.Text = @"File decrypted.";
            FileOutputLbl.ForeColor = Color.LimeGreen;
            MessageBox.Show(@"File was decrypted successfully.", @"Success", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            FileOutputLbl.Text = @"Idle...";
            FileOutputLbl.ForeColor = Color.WhiteSmoke;
            Array.Clear(_passwordArray, 0, _passwordArray.Length);
        }
        catch (Exception ex)
        {
            EnableUi();
            _isAnimating = false;
            FileOutputLbl.Text = @"Error decrypting file.";
            FileOutputLbl.ForeColor = Color.Red;
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            FileOutputLbl.Text = @"Idle...";
            FileOutputLbl.ForeColor = Color.WhiteSmoke;
            Array.Clear(_passwordArray, 0, _passwordArray.Length);
            ErrorLogging.ErrorLog(ex);
        }
    }

    private async void StartAnimationEncryption()
    {
        _isAnimating = true;
        await AnimateLabelEncrypt();
    }

    private async void StartAnimationDecryption()
    {
        _isAnimating = true;
        await AnimateLabelDecrypt();
    }

    private async Task AnimateLabelEncrypt()
    {
        while (_isAnimating)
        {
            FileOutputLbl.Text = @"Encrypting file";
            // Add animated periods
            for (var i = 0; i < 4; i++)
            {
                FileOutputLbl.Text += @".";
                await Task.Delay(400); // Delay between each period
            }
        }
    }

    private async Task AnimateLabelDecrypt()
    {
        while (_isAnimating)
        {
            FileOutputLbl.Text = @"Decrypting file";
            // Add animated periods
            for (var i = 0; i < 4; i++)
            {
                FileOutputLbl.Text += @".";
                await Task.Delay(400); // Delay between each period
            }
        }
    }

    [GeneratedRegex(@"^[a-zA-Z0-9\+/]*={0,3}$")]
    private static partial Regex MyRegex();

    private async void Vault_Load(object sender, EventArgs e)
    {
        UserWelcomeLbl.Text = @$"Welcome, {Authentication.CurrentLoggedInUser}!";
        await Task.Run(() => RainbowLabel(UserWelcomeLbl));
    }

    private static async Task RainbowLabel(Control label)
    {
        while (true)
        {
            label.ForeColor =
                Color.FromArgb(Crypto.BoundedInt(0, 255), Crypto.BoundedInt(0, 255), Crypto.BoundedInt(0, 255));

            await Task.Delay(125);
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private void DecryptBtn_MouseHover(object sender, EventArgs e)
    {
        Tip.AutomaticDelay = 500;
        Tip.IsBalloon = false;
        Tip.ToolTipIcon = ToolTipIcon.Info;
        Tip.Show("Decrypts the opened file.", DecryptBtn, int.MaxValue);
    }

    private void EncryptBtn_MouseHover(object sender, EventArgs e)
    {
        Tip.AutomaticDelay = 500;
        Tip.IsBalloon = false;
        Tip.ToolTipIcon = ToolTipIcon.Info;
        Tip.Show("Encrypts the opened file.", EncryptBtn, int.MaxValue);
    }

    private void ExportFileBtn_MouseHover(object sender, EventArgs e)
    {
        Tip.AutomaticDelay = 500;
        Tip.IsBalloon = false;
        Tip.ToolTipIcon = ToolTipIcon.Info;
        Tip.Show("Saves the output of the encrypted or decrypted file to a text file.", ExportFileBtn, int.MaxValue);
    }

    private void ImportFileBtn_MouseHover(object sender, EventArgs e)
    {
        Tip.AutomaticDelay = 500;
        Tip.IsBalloon = false;
        Tip.ToolTipIcon = ToolTipIcon.Info;
        Tip.Show(
            "Loads a text file to either encrypt or decrypt. The maximum file size is 950,000,000 bytes which is 0.95GB." +
            "\n" +
            "This is regardless of loading the file for encryption or decryption. Beware, the decrypted output size will always" +
            "\n" +
            "be higher than the original file size when unencrypted.", ImportFileBtn, int.MaxValue);
    }

    private void saveVaultBtn_MouseHover(object sender, EventArgs e)
    {
        Tip.AutomaticDelay = 500;
        Tip.IsBalloon = false;
        Tip.ToolTipIcon = ToolTipIcon.Info;
        Tip.Show("Saves the output of the encrypted or decrypted file to a text file.", saveVaultBtn, int.MaxValue);
    }

    private void deleteRowBtn_MouseHover(object sender, EventArgs e)
    {
        Tip.AutomaticDelay = 500;
        Tip.IsBalloon = false;
        Tip.ToolTipIcon = ToolTipIcon.Info;
        Tip.Show("Deletes the selected row.", deleteRowBtn, int.MaxValue);
    }

    private void addRowBtn_MouseHover(object sender, EventArgs e)
    {
        Tip.AutomaticDelay = 500;
        Tip.IsBalloon = false;
        Tip.ToolTipIcon = ToolTipIcon.Info;
        Tip.Show("Adds a new row that accepts values such as usernames, passwords, etc.", addRowBtn, int.MaxValue);
    }

    private void hashimportfile_Click(object sender, EventArgs e)
    {
        try
        {
            using var openFileDialog = new OpenFileDialog();
            openFileDialog.FilterIndex = 1;
            openFileDialog.ShowHiddenFiles = true;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.InitialDirectory =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var selectedFileName = openFileDialog.FileName;

                _loadedFileHash = selectedFileName;

                if (!string.IsNullOrEmpty(selectedFileName))
                {
                    filenamelbl.Text = $@"File Name: {_loadedFileHash}";
                }
            }
        }
        catch (Exception ex)
        {
            ErrorLogging.ErrorLog(ex);
            MessageBox.Show(ex.Message);
        }
    }

    private async void calculatehashbtn_Click(object sender, EventArgs e)
    {
        try
        {
            await using var fs = new FileStream(_loadedFileHash, FileMode.Open, FileAccess.Read);
            using var hashAlg = SHA512.Create();

            var hashBytes = await hashAlg.ComputeHashAsync(fs);
            var hashHexString = DataConversionHelpers.ByteArrayToHexString(hashBytes).ToLower();

            hashoutputtxt.Text = hashHexString;
        }
        catch (Exception ex)
        {
            ErrorLogging.ErrorLog(ex);
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            hashoutputtxt.Text = string.Empty;
        }
    }

    private void calculatehashbtn_MouseHover(object sender, EventArgs e)
    {
        Tip.AutomaticDelay = 500;
        Tip.IsBalloon = false;
        Tip.ToolTipIcon = ToolTipIcon.Info;
        Tip.Show("Calculates the hash of the opened file using SHA-512. Beware, larger files take longer to hash so please be patient.",
                  calculatehashbtn, int.MaxValue);
    }

    private void hashimportfile_MouseHover(object sender, EventArgs e)
    {
        Tip.AutomaticDelay = 500;
        Tip.IsBalloon = false;
        Tip.ToolTipIcon = ToolTipIcon.Info;
        Tip.Show("Opens a file to calculate the hash.", calculatehashbtn, int.MaxValue);
    }
}