using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Secure_Password_Vault
{
    public static class DataConversionHelpers
    {
        public static string? ByteArrayToHexString(byte[] Buffer)
        {
            return Buffer != null ? Convert.ToHexString(Buffer, 0, Buffer.Length).ToLower(CultureInfo.CurrentCulture) : null;
        }
        public static byte[]? HexStringToByteArray(string Input)
        {
            return !string.IsNullOrWhiteSpace(Input) ? Encoding.UTF8.GetBytes(Input) : null;
        }
        public static string? StringToHex(byte[] Buffer)
        {
            return Buffer != null ? Convert.ToHexString(Buffer, 0, Buffer.Length).ToLower(CultureInfo.CurrentCulture) : null;
        }
        public static string? ByteArrayToString(byte[] buffer)
        {
            return buffer != null ? Encoding.UTF8.GetString(buffer) : null;
        }

        public static byte[]? StringToByteArray(string input)
        {
            return !string.IsNullOrWhiteSpace(input) ? Encoding.UTF8.GetBytes(input) : null;
        }

        public static string? ByteArrayToBase64String(byte[] buffer)
        {
            return buffer != null ? Convert.ToBase64String(buffer) : null;
        }

        public static byte[]? Base64StringToByteArray(string input)
        {
            return !string.IsNullOrWhiteSpace(input) ? Convert.FromBase64String(input) : null;
        }
    }
}
