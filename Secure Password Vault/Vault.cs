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
        // Check if SecurePassword is null.
        if (Login.SecurePassword.Length == 0)
            throw new ArgumentException("Value was empty.", nameof(Login.SecurePassword));

        FileProcessingConstants.PasswordArray = ConvertSecureStringToCharArray(Login.SecurePassword);
    }

    private void CreateCustomArray()
    {
        var buffer = CustomPasswordTextBox.Text.Length;
        var passwordArray = new char[buffer];
        var confirmPasswordBuffer = ConfirmPassword.Text.Length;
        var confirmPasswordbufferArray = new char[confirmPasswordBuffer];
        CustomPasswordTextBox.Text.CopyTo(0, passwordArray, 0, buffer);
        RegisterAccount.CheckPasswordValidity(passwordArray, confirmPasswordbufferArray);

        // Validate that the password is not empty.
        if (passwordArray == Array.Empty<char>())
            throw new ArgumentException("Invalid password.", nameof(passwordArray));

        // Validate the password using the CheckPasswordValidity method.
        if (!RegisterAccount.CheckPasswordValidity(passwordArray))
            throw new ArgumentException(
                "Password must contain between 24 and 120 characters. It also must include:" +
                " 1.) At least one uppercase letter." +
                " 2.) At least one lowercase letter." +
                " 3.) At least one number." +
                " 4.) At least one special character." +
                " 5.) Must not contain any spaces." +
                " 6.) Both passwords must match.",
                nameof(passwordArray));

        FileProcessingConstants.PasswordArray = passwordArray;
        Array.Clear(passwordArray, 0, passwordArray.Length);
    }

    private void deleteRowBtn_Click(object sender, EventArgs e)
    {
        if (PassVault.SelectedRows.Count <= 0)
            return;
        var selectedRow = PassVault.SelectedRows[0].Index;

        PassVault.Rows.RemoveAt(selectedRow);
    }

    /// <summary>
    ///     Converts a <see cref="SecureString" /> to a character array.
    /// </summary>
    /// <param name="secureString">The SecureString to convert.</param>
    /// <returns>A character array containing the characters from the SecureString.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="secureString" /> is null.</exception>
    public static char[] ConvertSecureStringToCharArray(SecureString secureString)
    {
        // Check if secureString is null.
        if (secureString.Length == 0)
            throw new ArgumentException("SecureString was empty.", nameof(secureString));

        var charArray = new char[secureString.Length];
        var unmanagedString = IntPtr.Zero;

        try
        {
            // Convert SecureString to an unmanaged Unicode string.
            unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);

            // Copy characters from the unmanaged string to the charArray.
            for (var i = 0; i < secureString.Length; i++)
                charArray[i] = (char)Marshal.ReadInt16(unmanagedString, i * 2);
        }
        finally
        {
            // Zero-free the allocated unmanaged memory.
            if (unmanagedString != IntPtr.Zero) Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
        }

        return charArray;
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
            // Display a warning message to avoid closing the program during the operation.
            MessageBox.Show(
                @"Do NOT close the program while loading. This may cause corrupted data that is NOT recoverable.",
                @"Info",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            // Start the animation and disable the UI.
            StartAnimation();
            DisableUi();

            // Set the file path for the user's vault.
            var filePath = Path.Combine("Password Vault", "Users",
                Authentication.GetUserVault(Authentication.CurrentLoggedInUser));

            // Write the vault content to the file.
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

            // Convert the secure password to a character array.
            FileProcessingConstants.PasswordArray = ConvertSecureStringToCharArray(Login.SecurePassword);

            // Encrypt the vault and save it to the user's file.
            var encryptedVault = await Crypto.EncryptFile(Authentication.CurrentLoggedInUser,
                FileProcessingConstants.PasswordArray,
                Authentication.GetUserVault(Authentication.CurrentLoggedInUser));
            if (encryptedVault == Array.Empty<byte>())
                throw new ArgumentException("Value was empty.", nameof(encryptedVault));

            var encryptedVaultString = DataConversionHelpers.ByteArrayToBase64String(encryptedVault);
            await File.WriteAllTextAsync(Authentication.GetUserVault(Authentication.CurrentLoggedInUser),
                encryptedVaultString);
            Array.Clear(FileProcessingConstants.PasswordArray, 0, FileProcessingConstants.PasswordArray.Length);

            // Perform garbage collection to release resources.
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

            // Stop the animation, display success messages, and enable the UI.
            FileProcessingConstants.IsAnimating = false;
            outputLbl.Text = @"Vault saved successfully";
            outputLbl.ForeColor = Color.LimeGreen;
            MessageBox.Show(@"Vault saved successfully.", @"Save vault", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            EnableUi();
            outputLbl.Text = @"Idle...";
            outputLbl.ForeColor = Color.WhiteSmoke;
        }
        catch (Exception ex)
        {
            // Enable the UI and stop the animation in case of an error.
            EnableUi();
            FileProcessingConstants.IsAnimating = false;

            // Display an error message and log the exception.
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK,
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
            // Get the file path for the user's vault.
            var filePath = Authentication.GetUserVault(Authentication.CurrentLoggedInUser);

            // Check if the vault file exists.
            if (File.Exists(filePath))
            {
                // Open the vault file for reading.
                using var sr = new StreamReader(filePath);

                // Clear existing data in the DataGridView.
                PassVault.Rows.Clear();

                // Read each line from the vault file and populate the DataGridView.
                while (!sr.EndOfStream)
                {
                    // Read a line from the file.
                    var line = await sr.ReadLineAsync();

                    // Split the line by tabs.
                    var values = line?.Split('\t');

                    // Check for invalid input text (base64 encoded).
                    if (IsBase64(line))
                        throw new ArgumentException("Invalid input text", nameof(line));

                    // Skip processing if the values array is empty.
                    if (values is { Length: <= 0 }) continue;

                    // Add a new row to the DataGridView and populate it with values.
                    var rowIndex = PassVault.Rows.Add();
                    if (values != null)
                        for (var i = 0; i < values.Length; i++)
                            PassVault.Rows[rowIndex].Cells[i].Value = values[i];
                }
            }
            else
            {
                // If the vault file does not exist, enable UI and show an error message.
                EnableUi();
                MessageBox.Show(@"Vault file does not exist.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions by enabling UI, displaying an error message, and logging the exception.
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
            outputLbl.Text = @"Saving vault";
            // Add animated periods
            for (var i = 0; i < 4; i++)
            {
                outputLbl.Text += @".";
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
            // Create an OpenFileDialog.
            using var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = @"Txt files(*.txt) | *.txt";
            openFileDialog.FilterIndex = 1;
            openFileDialog.ShowHiddenFiles = true;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.InitialDirectory =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));


            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            // Get information about the selected file.
            var selectedFileName = openFileDialog.FileName;
            var selectedExtension = Path.GetExtension(selectedFileName).ToLower();
            var fileInfo = new FileInfo(selectedFileName);

            // Update FileProcessingConstants with file information.
            FileProcessingConstants.FileOpened = true;
            FileProcessingConstants.LoadedFile = selectedFileName;

            // Read the content of the file if it is not empty.
            if (!string.IsNullOrEmpty(FileProcessingConstants.Result))
            {
                await using var fs = new FileStream(selectedFileName, FileMode.OpenOrCreate, FileAccess.Read);
                using var sr = new StreamReader(fs, Encoding.UTF8);
                var result = await sr.ReadToEndAsync();

                if (string.IsNullOrEmpty(result))
                    throw new IOException("Result was empty.");
            }

            // Perform additional validations on the selected file.
            if (string.IsNullOrEmpty(selectedFileName))
                throw new ArgumentException("Value was empty.", nameof(selectedFileName));

            if (selectedExtension != ".txt")
                throw new ArgumentException("Invalid file extension. Please select a text file.",
                    nameof(selectedExtension));

            if (fileInfo.Length > FileProcessingConstants.MaximumFileSize)
                throw new ArgumentException("File size is too large.", nameof(FileProcessingConstants.FileSize));

            // Update FileProcessingConstants with file size.
            FileProcessingConstants.FileSize = (int)fileInfo.Length;

            // Display the file size.
            var fileSize = fileInfo.Length.ToString("#,0");
            FileSizeNumLbl.Text = $@"{fileSize} bytes";

            // Display success messages and reset UI.
            FileOutputLbl.Text = "File opened.";
            FileOutputLbl.ForeColor = Color.LimeGreen;
            MessageBox.Show("File opened successfully.", "Opened successfully", MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            // Reset UI and clear sensitive data.
            FileOutputLbl.Text = "Idle...";
            FileOutputLbl.ForeColor = Color.WhiteSmoke;
            Array.Clear(FileProcessingConstants.PasswordArray, 0, FileProcessingConstants.PasswordArray.Length);
            FileProcessingConstants.Result = string.Empty;
        }
        catch (Exception ex)
        {
            // Handle errors and display messages.
            FileSizeNumLbl.Text = "0";
            FileOutputLbl.Text = "Error loading file.";
            FileOutputLbl.ForeColor = Color.Red;

            // Reset UI and log the error.
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
            // Check if a file is opened.
            if (!FileProcessingConstants.FileOpened)
                throw new Exception(@"No file is opened.");

            // Create a SaveFileDialog.
            using var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = @"Txt files(*.txt) | *.txt";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.ShowHiddenFiles = true;
            saveFileDialog.CheckFileExists = false;
            saveFileDialog.CheckPathExists = false;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.InitialDirectory =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;
            // Get the selected file name.
            var selectedFileName = saveFileDialog.FileName;

            // Save the file content if it is not empty.
            if (string.IsNullOrEmpty(FileProcessingConstants.Result))
                return;
            await using (var fs = new FileStream(selectedFileName, FileMode.OpenOrCreate, FileAccess.Write))
            await using (var sw = new StreamWriter(fs, Encoding.UTF8))
            {
                await sw.WriteAsync(FileProcessingConstants.Result);
            }

            // Display success messages and reset UI.
            FileOutputLbl.Text = "File saved successfully.";
            FileOutputLbl.ForeColor = Color.LimeGreen;
            MessageBox.Show("File saved successfully.", "Saved successfully", MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            // Clear sensitive data.
            Array.Clear(FileProcessingConstants.PasswordArray, 0, FileProcessingConstants.PasswordArray.Length);
            FileProcessingConstants.Result = string.Empty;
        }
        catch (Exception ex)
        {
            // Handle errors and display messages.
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
            // Display a warning message about not closing the program during encryption.
            MessageBox.Show(
                @"Do NOT close the program while loading. This may cause corrupted data that is NOT recoverable.
                If using a custom password to encrypt with, MAKE SURE YOU REMEMBER THE PASSWORD! You will NOT be able to
                decrypt the file without the password. It is separate than the password you use to login with.",
                @"Info",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            // Check if a file is opened.
            if (!FileProcessingConstants.FileOpened)
                throw new ArgumentException(@"No file is opened.", nameof(FileProcessingConstants.FileOpened));

            // Set array and start the encryption animation.
            if (CustomPasswordCheckBox.Checked)
                CreateCustomArray();
            else
                SetArray();
            StartAnimationEncryption();
            DisableUi();

            // Check if a loaded file exists.
            if (FileProcessingConstants.LoadedFile == string.Empty)
                return;

            // Encrypt the file.
            var encryptedFile =
                await Crypto.EncryptFile(Authentication.CurrentLoggedInUser, FileProcessingConstants.PasswordArray,
                    FileProcessingConstants.LoadedFile);

            // Check if the encrypted file is empty.
            if (encryptedFile == Array.Empty<byte>())
                throw new ArgumentException(@"Value was empty.", nameof(encryptedFile));

            // Perform garbage collection aggressively.
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

            // Convert the encrypted file to Base64 string.
            var str = DataConversionHelpers.ByteArrayToBase64String(encryptedFile);


            // Update the result and enable UI.
            if (!string.IsNullOrEmpty(str))
                FileProcessingConstants.Result = str;

            EnableUi();
            FileProcessingConstants.IsAnimating = false;

            // Display the encrypted file size.
            var size = (long)FileProcessingConstants.Result.Length;
            var fileSize = size.ToString("#,0");
            FileSizeNumLbl.Text = $@"{fileSize} bytes";

            // Display success message.
            FileOutputLbl.Text = @"File encrypted.";
            FileOutputLbl.ForeColor = Color.LimeGreen;

            MessageBox.Show(@"File was encrypted successfully.", @"Success", MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            // Reset UI state and clear sensitive information.
            FileOutputLbl.Text = @"Idle...";
            FileOutputLbl.ForeColor = Color.WhiteSmoke;
            Array.Clear(FileProcessingConstants.PasswordArray, 0, FileProcessingConstants.PasswordArray.Length);
        }
        catch (Exception ex)
        {
            // Handle exceptions, enable UI, and log errors.
            EnableUi();
            FileProcessingConstants.IsAnimating = false;
            FileOutputLbl.Text = @"Error encrypting file.";
            FileOutputLbl.ForeColor = Color.Red;
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            FileOutputLbl.Text = @"Idle...";
            FileOutputLbl.ForeColor = Color.WhiteSmoke;
            Array.Clear(FileProcessingConstants.PasswordArray, 0, FileProcessingConstants.PasswordArray.Length);
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
            // Display an informational message to the user.
            MessageBox.Show(
                @"Do NOT close the program while loading. This may cause corrupted data that is NOT recoverable.",
                @"Info",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            // Check if a file is opened.
            if (!FileProcessingConstants.FileOpened)
                throw new ArgumentException(@"No file is opened.", nameof(FileProcessingConstants.FileOpened));

            // Check if the file size exceeds the maximum allowed size.
            if (FileProcessingConstants.FileSize > FileProcessingConstants.MaximumFileSize)
                throw new ArgumentException(@"File size is too large.", nameof(FileProcessingConstants.FileSize));

            // Initialize an array and disable the UI.
            if (CustomPasswordCheckBox.Checked)
                CreateCustomArray();
            else
                SetArray();
            DisableUi();
            StartAnimationDecryption();

            // If no loaded file is specified, return.
            if (FileProcessingConstants.LoadedFile == string.Empty)
                return;

            // Decrypt the file using the Crypto class.
            var decryptedFile = await Crypto.DecryptFile(Authentication.CurrentLoggedInUser,
                FileProcessingConstants.PasswordArray, FileProcessingConstants.LoadedFile);

            // Check if the decrypted file is empty.
            if (decryptedFile == Array.Empty<byte>())
                throw new Exception(@"The decrypted file value returned empty.");

            // Perform garbage collection.
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

            // Convert the decrypted file to a string.
            var str = DataConversionHelpers.ByteArrayToString(decryptedFile);

            // Update result and enable the UI.
            if (!string.IsNullOrEmpty(str))
                FileProcessingConstants.Result = str;

            EnableUi();
            FileProcessingConstants.IsAnimating = false;

            // Display the decrypted file size.
            var size = (long)FileProcessingConstants.Result.Length;
            var fileSize = size.ToString("#,0");
            FileSizeNumLbl.Text = $@"{fileSize} bytes";

            // Update UI labels and show success message.
            FileOutputLbl.Text = @"File decrypted.";
            FileOutputLbl.ForeColor = Color.LimeGreen;
            MessageBox.Show(@"File was decrypted successfully.", @"Success", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            FileOutputLbl.Text = @"Idle...";
            FileOutputLbl.ForeColor = Color.WhiteSmoke;

            // Clear the password array for security reasons.
            Array.Clear(FileProcessingConstants.PasswordArray, 0, FileProcessingConstants.PasswordArray.Length);
        }
        catch (Exception ex)
        {
            // Handle exceptions, enable UI, and log errors.
            EnableUi();
            FileProcessingConstants.IsAnimating = false;
            FileOutputLbl.Text = @"Error decrypting file.";
            FileOutputLbl.ForeColor = Color.Red;
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            FileOutputLbl.Text = @"Idle...";
            FileOutputLbl.ForeColor = Color.WhiteSmoke;
            Array.Clear(FileProcessingConstants.PasswordArray, 0, FileProcessingConstants.PasswordArray.Length);
            ErrorLogging.ErrorLog(ex);
        }
    }


    private async void StartAnimationEncryption()
    {
        FileProcessingConstants.IsAnimating = true;
        await AnimateLabelEncrypt();
    }

    private async void StartAnimationDecryption()
    {
        FileProcessingConstants.IsAnimating = true;
        await AnimateLabelDecrypt();
    }

    private async Task AnimateLabelEncrypt()
    {
        while (FileProcessingConstants.IsAnimating)
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
        while (FileProcessingConstants.IsAnimating)
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
        CustomPasswordTextBox.Enabled = false;
        ConfirmPassword.Enabled = false;

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
        FileProcessingConstants.Tooltip.Show("Encrypts the opened file.", EncryptBtn, int.MaxValue);
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

            FileProcessingConstants.LoadedFileHash = selectedFileName;

            if (!string.IsNullOrEmpty(selectedFileName))
                filenamelbl.Text = $@"File Name: {FileProcessingConstants.LoadedFileHash}";
        }
        catch (Exception ex)
        {
            ErrorLogging.ErrorLog(ex);
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void calculatehashbtn_Click(object sender, EventArgs e)
    {
        try
        {
            using var ms = new MemoryStream();
            await using (var fs = new FileStream(FileProcessingConstants.LoadedFileHash, FileMode.Open,
                             FileAccess.Read))
            {
                await fs.CopyToAsync(ms);
            }

            var hashBytes = Crypto.Sha3Hash(ms.ToArray());
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
        FileProcessingConstants.Tooltip.AutomaticDelay = 500;
        FileProcessingConstants.Tooltip.IsBalloon = false;
        FileProcessingConstants.Tooltip.ToolTipIcon = ToolTipIcon.Info;
        FileProcessingConstants.Tooltip.Show(
            "Calculates the hash of the opened file using SHA3-512. Beware, larger files take longer to hash so please be patient.",
            CalculateHashBtn, int.MaxValue);
    }

    // ReSharper disable once IdentifierTypo
    private void Hashimportfile_MouseHover(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.AutomaticDelay = 500;
        FileProcessingConstants.Tooltip.IsBalloon = false;
        FileProcessingConstants.Tooltip.ToolTipIcon = ToolTipIcon.Info;
        FileProcessingConstants.Tooltip.Show("Opens a file to calculate the hash.", CalculateHashBtn, int.MaxValue);
    }

    // ReSharper disable once IdentifierTypo
    private void Calculatehashbtn_MouseLeave(object sender, EventArgs e)
    {
        FileProcessingConstants.Tooltip.Hide(CalculateHashBtn);
    }

    // ReSharper disable once IdentifierTypo
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

    /// <summary>
    ///     Represents static fields and constants used for file processing and UI interactions.
    /// </summary>
    private static class FileProcessingConstants
    {
        /// <summary>
        ///     Gets the maximum allowed file size in bytes.
        /// </summary>
        public const int MaximumFileSize = 1_000_000_000;

        /// <summary>
        ///     Gets or sets an array to store the characters of the user's password.
        /// </summary>
        public static char[] PasswordArray = Array.Empty<char>();

        /// <summary>
        ///     Gets or sets the path of the currently loaded file.
        /// </summary>
        public static string LoadedFile = string.Empty;

        /// <summary>
        ///     Gets or sets the hash value of the currently loaded file.
        /// </summary>
        public static string LoadedFileHash = string.Empty;

        /// <summary>
        ///     Gets or sets the result of a file processing operation.
        /// </summary>
        public static string Result = string.Empty;

        /// <summary>
        ///     Gets the tooltip used for providing information to the user.
        /// </summary>
        public static readonly ToolTip Tooltip = new();

        /// <summary>
        ///     Gets or sets a value indicating whether an animation is currently in progress.
        /// </summary>
        public static bool IsAnimating { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether a file is currently opened.
        /// </summary>
        public static bool FileOpened { get; set; }

        /// <summary>
        ///     Gets or sets the size of the currently loaded file.
        /// </summary>
        public static long FileSize { get; set; }
    }
}