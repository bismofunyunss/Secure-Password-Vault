using System.Text.RegularExpressions;
using Windows.Devices.PointOfService;
using static System.Net.Mime.MediaTypeNames;

namespace Secure_Password_Vault;

public partial class Vault : Form
{
    public Vault()
    {
        InitializeComponent();
        PassVault.EnableHeadersVisualStyles = false;
    }

    private void addRowBtn_Click(object sender, EventArgs e)
    {
        PassVault.Rows.Add();
    }

    private void deleteRowBtn_Click(object sender, EventArgs e)
    {
        if (PassVault.SelectedRows.Count > 0)
        {
            var selectedRow = PassVault.SelectedRows[0].Index; 

            PassVault.Rows.RemoveAt(selectedRow);
        }
    }

    private void saveVaultBtn_Click(object sender, EventArgs e)
    {
        try
        {
            var filePath = Path.Combine("Password Vault", "Users",
                Authentication.GetUserVault(Authentication.CurrentLoggedInUser));

            using (var sw = new StreamWriter(filePath))
            {
                sw.NewLine = null;
                sw.AutoFlush = true;
                foreach (DataGridViewRow row in PassVault.Rows)
                {
                    for (var i = 0; i < PassVault.Columns.Count; i++)
                    {
                        row.Cells[i].ValueType = typeof(char[]);
                        sw.Write(row.Cells[i].Value);
                        if (i < PassVault.Columns.Count - 1) sw.Write("\t"); // Use a tab character to separate columns
                    }

                    sw.WriteLine(); // Start a new line for each row
                }
            }

            PopupPassword.Save = true;
            using var form = new PopupPassword();
            form.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void loadVaultBtn_Click(object sender, EventArgs e)
    {
        try
        {
            PopupPassword.Load = true;
            using PopupPassword form = new();
            form.ShowDialog();

            var filePath = Path.Combine("Password Vault", "Users",
                Authentication.GetUserVault(Authentication.CurrentLoggedInUser));

            if (File.Exists(filePath))
            {
                using (var sr = new StreamReader(filePath))
                {
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

                GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
                var encryptedVault = await Crypto.EncryptUserFiles(Authentication.CurrentLoggedInUser,
                    PopupPassword.PasswordArray, Authentication.GetUserVault(Authentication.CurrentLoggedInUser));
                if (encryptedVault != null)
                    await File.WriteAllTextAsync(Authentication.GetUserVault(Authentication.CurrentLoggedInUser),
                        DataConversionHelpers.ByteArrayToBase64String(encryptedVault));
                Array.Clear(PopupPassword.PasswordArray, 0, PopupPassword.PasswordArray.Length);
            }
            else
            {
                MessageBox.Show(@"Vault file does not exist.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private static bool IsBase64(string? str)
    {
        return str != null && Regex.IsMatch(str, @"^[a-zA-Z0-9\+/]*={0,3}$") && (str.Length % 4 == 0);
    }
}