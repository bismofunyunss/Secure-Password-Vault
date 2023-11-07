namespace Secure_Password_Vault;

public static class ErrorLogging
{
    public static void ErrorLog(Exception ex)
    {
        string logFileName = "ErrorLog.txt";

        // Create the log file if it doesn't exist
        if (!File.Exists(logFileName))
        {
            using (File.Create(logFileName))
            {
            }
        }

        try
        {
            using var writer = File.AppendText(logFileName);
            writer.AutoFlush = true;
            writer.WriteLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            writer.WriteLine($"Exception Type: {ex.GetType().FullName}");
            writer.WriteLine($"Message: {ex.Message}");
            writer.WriteLine($"Stack Trace: {ex.StackTrace}");
            writer.WriteLine();

            // If there's an inner exception, log it
            if (ex.InnerException != null)
            {
                writer.WriteLine("Inner Exception:");
                writer.WriteLine($"Exception Type: {ex.InnerException.GetType().FullName}");
                writer.WriteLine($"Message: {ex.InnerException.Message}");
                writer.WriteLine($"Stack Trace: {ex.InnerException.StackTrace}");
                writer.WriteLine();
            }
        }
        catch (IOException ioException)
        {
            // Handle file I/O errors
            MessageBox.Show($@"Error logging failed due to I/O exception: {ioException.Message}", @"Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception logException)
        {
            // Handle other unexpected exceptions during logging
            MessageBox.Show($@"Error logging failed with an unexpected exception: {logException.Message}", @"Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}