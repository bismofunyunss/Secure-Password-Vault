using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace Secure_Password_Vault;

public partial class Vault : Form
{
    public Vault()
    {
        InitializeComponent();
    }

    /// <summary>
    ///     Represents static fields and constants used for file processing and UI interactions.
    /// </summary>
    public static class FileProcessingConstants
    {
        public const int MaximumFileSize = 1_000_000_000;
        public static char[] PasswordArray = Array.Empty<char>();
        public static string LoadedFile = string.Empty;
        public static string LoadedFileToHash = string.Empty;
        public static string Result = string.Empty;
        public static bool IsAnimating { get; set; }
        public static bool FileOpened { get; set; }
        public static long FileSize { get; set; }

        public static readonly ToolTip Tooltip = new();
    }

    private void addRowBtn_Click(object sender, EventArgs e)
    {
        PassVault.Rows.Add();
    }

    /// <summary>
    ///     Sets the password array based on the content of the <see cref="SecureString" />.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if <see cref="SecureString" /> is null.</exception>
    private static void SetArray()
    {
        if (Crypto.CryptoConstants.SecurePassword == null || Crypto.CryptoConstants.SecurePassword.Length == 0)
            throw new ArgumentException("Value was empty.", nameof(Crypto.CryptoConstants.SecurePassword));

        var arrayPtr = Crypto.ConversionMethods.CreatePinnedCharArray(Crypto.ConversionMethods.ConvertSecureStringToCharArray(Crypto.CryptoConstants.SecurePassword));
        int len = Crypto.CryptoConstants.SecurePassword.Length;
        var charArray = Crypto.ConversionMethods.ConvertIntPtrToCharArray(arrayPtr, len);

        Marshal.FreeCoTaskMem(arrayPtr);
        arrayPtr = IntPtr.Zero;

        FileProcessingConstants.PasswordArray = charArray;
    }

    private void CreateCustomArray()
    {
        if (Crypto.CryptoConstants.SecurePassword == null || Crypto.CryptoConstants.SecurePassword.Length == 0)
            throw new ArgumentException("Value was empty.", nameof(Crypto.CryptoConstants.SecurePassword));

        var arrayPtr = Crypto.ConversionMethods.CreatePinnedCharArray(CustomPasswordTextBox.Text);
        int len = CustomPasswordTextBox.Text.Length;
        var charArray = Crypto.ConversionMethods.ConvertIntPtrToCharArray(arrayPtr, len);

        Marshal.FreeCoTaskMem(arrayPtr);
        arrayPtr = IntPtr.Zero;

        FileProcessingConstants.PasswordArray = charArray;
    }

    private void deleteRowBtn_Click(object sender, EventArgs e)
    {
        if (PassVault.SelectedRows.Count <= 0)
            return;
        var selectedRow = PassVault.SelectedRows[0].Index;

        PassVault.Rows.RemoveAt(selectedRow);
    }

    /// <summary>
    ///     Handles the click event of the Save Vault button.
    /// </summary>
    /// <param name="sender">The object that triggered the event.</param>
    /// <param name="e">The event arguments.</param>
    /// <exception cref="ArgumentException"></exception>
    private async void saveVaultBtn_Click(object sender, EventArgs e)
    {
        try
        {
            MessageBox.Show(
                "Do NOT close the program while loading. This may cause corrupted data that is NOT recoverable.",
                "Info",
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
                            await sw.WriteAsync("\t");
                    }

                    await sw.WriteLineAsync();
                }
            }
            SetArray();

            var encryptedVault = await Crypto.EncryptFile(Authentication.CurrentLoggedInUser,
                FileProcessingConstants.PasswordArray,
                Authentication.GetUserVault(Authentication.CurrentLoggedInUser));

            if (encryptedVault == Array.Empty<byte>())
                throw new ArgumentException("Value was empty.", nameof(encryptedVault));

            var encryptedVaultString = DataConversionHelpers.ByteArrayToBase64String(encryptedVault);
            await File.WriteAllTextAsync(Authentication.GetUserVault(Authentication.CurrentLoggedInUser),
                encryptedVaultString);

            Crypto.ConversionMethods.CreatePinnedCharArray(FileProcessingConstants.PasswordArray);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

            FileProcessingConstants.IsAnimating = false;
            outputLbl.Text = "Vault saved successfully";
            outputLbl.ForeColor = Color.LimeGreen;
            MessageBox.Show("Vault saved successfully.", "Save vault", MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            EnableUi();
            outputLbl.Text = "Idle...";
            outputLbl.ForeColor = Color.WhiteSmoke;
        }
        catch (Exception ex)
        {
            EnableUi();
            FileProcessingConstants.IsAnimating = false;
            Crypto.CryptoUtilities.ClearMemory(FileProcessingConstants.PasswordArray);

            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            ErrorLogging.ErrorLog(ex);
        }
    }

    /// <summary>
    ///     Asynchronously loads the user's vault data into the PassVault DataGridView.
    /// </summary>
    public async void LoadVault()
    {
        try
        {
            var filePath = Authentication.GetUserVault(Authentication.CurrentLoggedInUser);

            if (File.Exists(filePath))
            {
                using var sr = new StreamReader(filePath);
                PassVault.Rows.Clear();

                while (!sr.EndOfStream)
                {
                    var line = await sr.ReadLineAsync();
                    var values = line?.Split('\t');

                    if (IsBase64(line))
                        throw new ArgumentException("Invalid input text", nameof(line));
                    if (values is { Length: <= 0 })
                        continue;

                    var rowIndex = PassVault.Rows.Add();

                    if (values != null)
                        for (var i = 0; i < values.Length; i++)
                            PassVault.Rows[rowIndex].Cells[i].Value = values[i];
                }
            }
            else
            {
                EnableUi();
                MessageBox.Show("Vault file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            EnableUi();
            Crypto.CryptoUtilities.ClearMemory(FileProcessingConstants.PasswordArray);
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ErrorLogging.ErrorLog(ex);
        }
    }


    private static bool IsBase64(string? str)
    {
        return str != null && MyRegex().IsMatch(str) && str.Length % 4 == 0;
    }

    private void EnableUi()
    {
        SaveVaultBtn.Enabled = true;
        DeleteRowBtn.Enabled = true;
        AddRowBtn.Enabled = true;
        PassVault.Enabled = true;
        ImportFileBtn.Enabled = true;
        ExportFileBtn.Enabled = true;
        EncryptBtn.Enabled = true;
        DecryptBtn.Enabled = true;
        Hashimportfile.Enabled = true;
        CalculateHashBtn.Enabled = true;
    }

    private void DisableUi()
    {
        SaveVaultBtn.Enabled = false;
        DeleteRowBtn.Enabled = false;
        AddRowBtn.Enabled = false;
        PassVault.Enabled = false;
        ImportFileBtn.Enabled = false;
        ExportFileBtn.Enabled = false;
        EncryptBtn.Enabled = false;
        DecryptBtn.Enabled = false;
        Hashimportfile.Enabled = false;
        CalculateHashBtn.Enabled = false;
    }

    private async void StartAnimation()
    {
        FileProcessingConstants.IsAnimating = true;
        await AnimateLabel();
    }

    private async Task AnimateLabel()
    {
        while (FileProcessingConstants.IsAnimating)
        {
            outputLbl.Text = "Saving vault";
            for (var i = 0; i < 4; i++)
            {
                outputLbl.Text += ".";
                await Task.Delay(400); // Delay between each period
            }
        }
    }

    /// <summary>
    ///     Handles the click event of the Import File button.
    /// </summary>
    /// <param name="sender">The object that triggered the event.</param>
    /// <param name="e">The event arguments.</param>
    private async void ImportFileBtn_Click(object? sender, EventArgs e)
    {
        try
        {
            using var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Txt files(*.txt) | *.txt";
            openFileDialog.FilterIndex = 1;
            openFileDialog.ShowHiddenFiles = true;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.InitialDirectory =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            var selectedFileName = openFileDialog.FileName;
            var selectedExtension = Path.GetExtension(selectedFileName).ToLower();
            var fileInfo = new FileInfo(selectedFileName);

            FileProcessingConstants.FileOpened = true;
            FileProcessingConstants.LoadedFile = selectedFileName;

            if (!string.IsNullOrEmpty(FileProcessingConstants.Result))
            {
                await using var fs = new FileStream(selectedFileName, FileMode.OpenOrCreate, FileAccess.Read);
                using var sr = new StreamReader(fs, Encoding.UTF8);
                var result = await sr.ReadToEndAsync();

                if (string.IsNullOrEmpty(result))
                    throw new IOException("Result was empty.");
            }

            if (string.IsNullOrEmpty(selectedFileName))
                throw new ArgumentException("Value was empty.", nameof(selectedFileName));

            if (selectedExtension != ".txt")
                throw new ArgumentException("Invalid file extension. Please select a text file.",
                    nameof(selectedExtension));

            if (fileInfo.Length > FileProcessingConstants.MaximumFileSize)
                throw new ArgumentException("File size is too large.", nameof(FileProcessingConstants.FileSize));

            FileProcessingConstants.FileSize = (int)fileInfo.Length;

            var fileSize = fileInfo.Length.ToString("#,0");
            FileSizeNumLbl.Text = $"{fileSize} bytes";

            FileOutputLbl.Text = "File opened.";
            FileOutputLbl.ForeColor = Color.LimeGreen;
            MessageBox.Show("File opened successfully.", "Opened successfully", MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            FileOutputLbl.Text = "Idle...";
            FileOutputLbl.ForeColor = Color.WhiteSmoke;
            FileProcessingConstants.Result = string.Empty;
        }
        catch (Exception ex)
        {
            FileSizeNumLbl.Text = "0";
            FileOutputLbl.Text = "Error loading file.";
            FileOutputLbl.ForeColor = Color.Red;
            FileOutputLbl.Text = "Idle...";
            FileOutputLbl.ForeColor = Color.WhiteSmoke;
            ErrorLogging.ErrorLog(ex);
        }
    }


    /// <summary>
    ///     Handles the click event of the Export File button.
    /// </summary>
    /// <param name="sender">The object that triggered the event.</param>
    /// <param name="e">The event arguments.</param>
    private async void ExportFileBtn_Click(object sender, EventArgs e)
    {
        try
        {
            if (!FileProcessingConstants.FileOpened)
                throw new Exception("No file is opened.");

            using var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Txt files(*.txt) | *.txt";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.ShowHiddenFiles = true;
            saveFileDialog.CheckFileExists = false;
            saveFileDialog.CheckPathExists = false;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.InitialDirectory =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            var selectedFileName = saveFileDialog.FileName;

            if (string.IsNullOrEmpty(FileProcessingConstants.Result))
                return;
            await using (var fs = new FileStream(selectedFileName, FileMode.OpenOrCreate, FileAccess.Write))
            await using (var sw = new StreamWriter(fs, Encoding.UTF8))
            {
                await sw.WriteAsync(FileProcessingConstants.Result);
            }

            FileOutputLbl.Text = "File saved successfully.";
            FileOutputLbl.ForeColor = Color.LimeGreen;
            MessageBox.Show("File saved successfully.", "Saved successfully", MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            FileProcessingConstants.Result = string.Empty;
        }
        catch (Exception ex)
        {
            FileOutputLbl.Text = "Error saving file.";
            FileOutputLbl.ForeColor = Color.Red;
            ErrorLogging.ErrorLog(ex);
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            FileOutputLbl.Text = "Idle...";
            FileOutputLbl.ForeColor = Color.WhiteSmoke;
        }
    }


    /// <summary>
    ///     Event handler for the Encrypt button click.
    ///     Encrypts the currently opened file using the specified password.
    /// </summary>
    /// <param name="sender">The object that triggered the event.</param>
    /// <param name="e">The event arguments.</param>
    private async void EncryptBtn_Click(object sender, EventArgs e)
    {
        try
        {
            MessageBox.Show(
                @"Do NOT close the program while loading. This may cause corrupted data that is NOT recoverable.
                If using a custom password to encrypt with, MAKE SURE YOU REMEMBER THE PASSWORD! You will NOT be able to
                decrypt the file without the password. It is separate than the password you use to login with.",
                "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            if (!FileProcessingConstants.FileOpened)
                throw new ArgumentException("No file is opened.", nameof(FileProcessingConstants.FileOpened));

            if (CustomPasswordCheckBox.Checked)
                CreateCustomArray();
            else
                SetArray();
            StartAnimationEncryption();
            DisableUi();

            if (FileProcessingConstants.LoadedFile == string.Empty)
                return;

            var encryptedFile =
               await Crypto.EncryptFile(Authentication.CurrentLoggedInUser, FileProcessingConstants.PasswordArray,
                    FileProcessingConstants.LoadedFile);

            if (encryptedFile == Array.Empty<byte>())
                throw new ArgumentException("Value was empty.", nameof(encryptedFile));

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

            var str = DataConversionHelpers.ByteArrayToBase64String(encryptedFile);

            if (!string.IsNullOrEmpty(str))
                FileProcessingConstants.Result = str;

            EnableUi();
            FileProcessingConstants.IsAnimating = false;

            FileOutputLbl.Text = "File encrypted.";
            FileOutputLbl.ForeColor = Color.LimeGreen;
            Crypto.CryptoUtilities.ClearMemory(FileProcessingConstants.PasswordArray);

            MessageBox.Show("File was encrypted successfully.", "Success", MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

            var size = (long)FileProcessingConstants.Result.Length;
            var fileSize = size.ToString("#,0");
            FileSizeNumLbl.Text = $"{fileSize} bytes";

            FileOutputLbl.Text = "Idle...";
            FileOutputLbl.ForeColor = Color.WhiteSmoke;
        }
        catch (Exception ex)
        {
            EnableUi();
            FileProcessingConstants.IsAnimating = false;
            FileOutputLbl.Text = "Error encrypting file.";
            FileOutputLbl.ForeColor = Color.Red;
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            FileOutputLbl.Text = "Idle...";
            FileOutputLbl.ForeColor = Color.WhiteSmoke;
            Crypto.CryptoUtilities.ClearMemory(FileProcessingConstants.PasswordArray);
            ErrorLogging.ErrorLog(ex);
        }
    }


    /// <summary>
    ///     Event handler for the "Decrypt" button click.
    /// </summary>
    /// <param name="sender">The object that raised the event (the "Decrypt" button).</param>
    /// <param name="e">Event arguments.</param>
    private async void DecryptBtn_Click(object sender, EventArgs e)
    {
        try
        {
            MessageBox.Show(
                "Do NOT close the program while loading. This may cause corrupted data that is NOT recoverable.",
                "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            if (!FileProcessingConstants.FileOpened)
                throw new ArgumentException("No file is opened.", nameof(FileProcessingConstants.FileOpened));

            if (FileProcessingConstants.FileSize > FileProcessingConstants.MaximumFileSize)
                throw new ArgumentException("File size is too large.", nameof(FileProcessingConstants.FileSize));

            if (CustomPasswordCheckBox.Checked)
                CreateCustomArray();
            else
                SetArray();
            DisableUi();
            StartAnimationDecryption();

            if (FileProcessingConstants.LoadedFile == string.Empty)
                return;

            var decryptedFile = await Crypto.DecryptFile(Authentication.CurrentLoggedInUser,
                FileProcessingConstants.PasswordArray, FileProcessingConstants.LoadedFile);

            if (decryptedFile == Array.Empty<byte>())
                throw new Exception("The decrypted file value returned empty.");

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

            var str = DataConversionHelpers.ByteArrayToString(decryptedFile);

            if (!string.IsNullOrEmpty(str))
                FileProcessingConstants.Result = str;

            EnableUi();
            FileProcessingConstants.IsAnimating = false;

            FileOutputLbl.Text = @"File decrypted.";
            FileOutputLbl.ForeColor = Color.LimeGreen;

            Crypto.CryptoUtilities.ClearMemory(FileProcessingConstants.PasswordArray);

            MessageBox.Show(@"File was decrypted successfully.", @"Success", MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            var size = (long)FileProcessingConstants.Result.Length;
            var fileSize = size.ToString("#,0");
            FileSizeNumLbl.Text = $"{fileSize} bytes";

            FileOutputLbl.Text = @"Idle...";
            FileOutputLbl.ForeColor = Color.WhiteSmoke;
        }
        catch (Exception ex)
        {
            EnableUi();
            FileProcessingConstants.IsAnimating = false;
            FileOutputLbl.Text = @"Error decrypting file.";
            FileOutputLbl.ForeColor = Color.Red;
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            FileOutputLbl.Text = @"Idle...";
            FileOutputLbl.ForeColor = Color.WhiteSmoke;
            Crypto.CryptoUtilities.ClearMemory(FileProcessingConstants.PasswordArray);
            ErrorLogging.ErrorLog(ex);
        }
    }

    /// <summary>
    /// Initiates the animation for encryption.
    /// </summary>
    private async void StartAnimationEncryption()
    {
        FileProcessingConstants.IsAnimating = true;
        await AnimateLabelEncrypt();
    }

    /// <summary>
    /// Initiates the animation for decryption.
    /// </summary>
    private async void StartAnimationDecryption()
    {
        FileProcessingConstants.IsAnimating = true;
        await AnimateLabelDecrypt();
    }

    /// <summary>
    /// Asynchronously animates the label to indicate file encryption progress.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task AnimateLabelEncrypt()
    {
        while (FileProcessingConstants.IsAnimating)
        {
            FileOutputLbl.Text = @"Encrypting file";
            for (var i = 0; i < 4; i++)
            {
                FileOutputLbl.Text += @".";
                await Task.Delay(400);
            }
        }
    }

    /// <summary>
    /// Asynchronously animates the label to indicate file decryption progress.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task AnimateLabelDecrypt()
    {
        while (FileProcessingConstants.IsAnimating)
        {
            FileOutputLbl.Text = @"Decrypting file";
            for (var i = 0; i < 4; i++)
            {
                FileOutputLbl.Text += @".";
                await Task.Delay(400);
            }
        }
    }

    [GeneratedRegex(@"^[a-zA-Z0-9\+/]*={0,3}$")]
    private static partial Regex MyRegex();

    private async void Vault_Load(object sender, EventArgs e)
    {
        CustomPasswordTextBox.Enabled = false;
        ConfirmPassword.Enabled = false;

        UserWelcomeLbl.Text = @$"Welcome, {Authentication.CurrentLoggedInUser}!";
        await Task.Run(() => RainbowLabel(UserWelcomeLbl));
    }

    /// <summary>
    /// Asynchronously animates the text color of a label to create a rainbow effect.
    /// </summary>
    /// <param name="label">The label to animate.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static async Task RainbowLabel(Control label)
    {
        while (true)
        {
            label.ForeColor =
                Color.FromArgb(Crypto.CryptoUtilities.BoundedInt(0, 255), Crypto.CryptoUtilities.BoundedInt(0, 255), Crypto.CryptoUtilities.BoundedInt(0, 255));

            await Task.Delay(125);
        }
    }

    private void DecryptBtn_MouseHover(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.AutomaticDelay = 500;
        FileProcessingConstants.Tooltip.IsBalloon = false;
        FileProcessingConstants.Tooltip.ToolTipIcon = ToolTipIcon.Info;
        FileProcessingConstants.Tooltip.Show("Decrypts the opened file.", DecryptBtn, int.MaxValue);
    }

    private void EncryptBtn_MouseHover(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.AutomaticDelay = 500;
        FileProcessingConstants.Tooltip.IsBalloon = false;
        FileProcessingConstants.Tooltip.ToolTipIcon = ToolTipIcon.Info;
        FileProcessingConstants.Tooltip.Show("Encrypts the opened file within a certain file size value of 1GB.", EncryptBtn, int.MaxValue);
    }

    private void ExportFileBtn_MouseHover(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.AutomaticDelay = 500;
        FileProcessingConstants.Tooltip.IsBalloon = false;
        FileProcessingConstants.Tooltip.ToolTipIcon = ToolTipIcon.Info;
        FileProcessingConstants.Tooltip.Show("Saves the output of the encrypted or decrypted file to a text file.",
            ExportFileBtn, int.MaxValue);
    }

    private void ImportFileBtn_MouseHover(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.AutomaticDelay = 500;
        FileProcessingConstants.Tooltip.IsBalloon = false;
        FileProcessingConstants.Tooltip.ToolTipIcon = ToolTipIcon.Info;
        FileProcessingConstants.Tooltip.Show(
            "Loads a text file to either encrypt or decrypt. The maximum file size is 1GB.", ImportFileBtn,
            int.MaxValue);
    }

    private void saveVaultBtn_MouseHover(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.AutomaticDelay = 500;
        FileProcessingConstants.Tooltip.IsBalloon = false;
        FileProcessingConstants.Tooltip.ToolTipIcon = ToolTipIcon.Info;
        FileProcessingConstants.Tooltip.Show("Saves the output of the encrypted or decrypted file to a text file.",
            SaveVaultBtn, int.MaxValue);
    }

    private void deleteRowBtn_MouseHover(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.AutomaticDelay = 500;
        FileProcessingConstants.Tooltip.IsBalloon = false;
        FileProcessingConstants.Tooltip.ToolTipIcon = ToolTipIcon.Info;
        FileProcessingConstants.Tooltip.Show("Deletes the selected row.", DeleteRowBtn, int.MaxValue);
    }

    private void addRowBtn_MouseHover(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.AutomaticDelay = 500;
        FileProcessingConstants.Tooltip.IsBalloon = false;
        FileProcessingConstants.Tooltip.ToolTipIcon = ToolTipIcon.Info;
        FileProcessingConstants.Tooltip.Show("Adds a new row that accepts values such as usernames, passwords, etc.",
            AddRowBtn, int.MaxValue);
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

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            var selectedFileName = openFileDialog.FileName;

            FileProcessingConstants.LoadedFileToHash = selectedFileName;

            if (!string.IsNullOrEmpty(selectedFileName))
                filenamelbl.Text = $@"File Name: {FileProcessingConstants.LoadedFileToHash}";
        }
        catch (Exception ex)
        {
            ErrorLogging.ErrorLog(ex);
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Asynchronously calculates the SHA-3 hash of a file.
    /// </summary>
    /// <param name="file">The file path or name.</param>
    /// <returns>A task representing the asynchronous operation that returns the hash value as a hexadecimal string.</returns>
    private static async Task<string> CalculateHash(string file)
    {
        using var ms = new MemoryStream();
        await using (var fs = new FileStream(FileProcessingConstants.LoadedFileToHash, FileMode.Open,
                         FileAccess.Read))
        {
            await fs.CopyToAsync(ms);
        }

        var hashBytes = Crypto.HashingMethods.Sha3Hash(ms.ToArray());
        var hashHexString = DataConversionHelpers.ByteArrayToHexString(hashBytes).ToLower();

        return hashHexString;
    }

    private async void calculatehashbtn_Click(object sender, EventArgs e)
    {
        try
        {
           var result = await CalculateHash(FileProcessingConstants.LoadedFileToHash);
           hashoutputtxt.Text = result;
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
        FileProcessingConstants.Tooltip.AutomaticDelay = 500;
        FileProcessingConstants.Tooltip.IsBalloon = false;
        FileProcessingConstants.Tooltip.ToolTipIcon = ToolTipIcon.Info;
        FileProcessingConstants.Tooltip.Show(
            "Calculates the hash of the opened file using SHA3-512. Beware, larger files take longer to hash so please be patient.",
            CalculateHashBtn, int.MaxValue);
    }

    private void Hashimportfile_MouseHover(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.AutomaticDelay = 500;
        FileProcessingConstants.Tooltip.IsBalloon = false;
        FileProcessingConstants.Tooltip.ToolTipIcon = ToolTipIcon.Info;
        FileProcessingConstants.Tooltip.Show("Opens a file to calculate the hash.", CalculateHashBtn, int.MaxValue);
    }

    private void Calculatehashbtn_MouseLeave(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.Hide(CalculateHashBtn);
    }

    private void Hashimportfile_MouseLeave(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.Hide(Hashimportfile);
    }

    private void DecryptBtn_MouseLeave(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.Hide(DecryptBtn);
    }

    private void EncryptBtn_MouseLeave(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.Hide(EncryptBtn);
    }

    private void ImportFileBtn_MouseLeave(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.Hide(ImportFileBtn);
    }

    private void ExportFileBtn_MouseLeave(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.Hide(ExportFileBtn);
    }

    private void SaveVaultBtn_MouseLeave(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.Hide(SaveVaultBtn);
    }

    private void DeleteRowBtn_MouseLeave(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.Hide(DeleteRowBtn);
    }

    private void AddRowBtn_MouseLeave(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.Hide(AddRowBtn);
    }

    private void ViewPasswordsCheckbox_CheckedChanged(object sender, EventArgs e)
    {
        CustomPasswordTextBox.UseSystemPasswordChar = !ViewPasswordsCheckbox.Checked;
        ConfirmPassword.UseSystemPasswordChar = !ViewPasswordsCheckbox.Checked;
    }

    private void CustomPasswordCheckBox_MouseHover(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.AutomaticDelay = 500;
        FileProcessingConstants.Tooltip.IsBalloon = false;
        FileProcessingConstants.Tooltip.ToolTipIcon = ToolTipIcon.Info;
        FileProcessingConstants.Tooltip.Show("Allows you to use a different password than the default login password" +
                                             " to derive a key from and encrypt / decrypt a file. Make sure you do NOT forget this" +
                                             " password as your file will be unable to encrypt or decrypt without it.",
            CustomPasswordCheckBox, int.MaxValue);
    }

    private void CustomPasswordCheckBox_MouseLeave(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.Hide(CustomPasswordTextBox);
    }

    private void CustomPasswordCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        if (CustomPasswordCheckBox.Checked)
        {
            CustomPasswordTextBox.Enabled = true;
            ConfirmPassword.Enabled = true;
        }
        else
        {
            CustomPasswordTextBox.Enabled = false;
            ConfirmPassword.Enabled = false;
        }
    }
}
