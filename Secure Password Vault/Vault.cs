using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;

namespace Secure_Password_Vault;

public partial class Vault : Form
{
    private static bool _isAnimating;
    private static char[] _passwordArray = Array.Empty<char>();
    public Vault()
    {
        InitializeComponent();
    }

    private static bool enteredPassword;

    private void addRowBtn_Click(object sender, EventArgs e)
    {
        PassVault.Rows.Add();
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
        {
            throw new ArgumentNullException(nameof(secureString));
        }

        var charArray = new char[secureString.Length];
        var unmanagedString = IntPtr.Zero;

        try
        {
            unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
            for (var i = 0; i < secureString.Length; i++)
            {
                charArray[i] = (char)Marshal.ReadInt16(unmanagedString, i * 2);
            }
        }
        finally
        {
            if (unmanagedString != IntPtr.Zero)
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        return charArray;
    }

    private async void saveVaultBtn_Click(object sender, EventArgs e)
    {
        try
        {
            StartAnimation();

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
            outputLbl.Text = @"Saved successfully";
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
                        return;

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
        }
    }

    private static bool IsBase64(string? str)
    {
        return str != null && Regex.IsMatch(str, @"^[a-zA-Z0-9\+/]*={0,3}$") && str.Length % 4 == 0;
    }

    private void EnableUi()
    {
        saveVaultBtn.Enabled = true;
        deleteRowBtn.Enabled = true;
        addRowBtn.Enabled = true;
        PassVault.Enabled = true;
    }

    private void DisableUi()
    {
        saveVaultBtn.Enabled = false;
        deleteRowBtn.Enabled = false;
        addRowBtn.Enabled = false;
        PassVault.Enabled = false;
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
}