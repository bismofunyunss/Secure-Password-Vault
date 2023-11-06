﻿using System.Globalization;
using System.Text;

namespace Secure_Password_Vault;

public static class DataConversionHelpers
{
    public static string? ByteArrayToHexString(byte[]? buffer)
    {
        return buffer != null
            ? Convert.ToHexString(buffer, 0, buffer.Length).ToUpper(CultureInfo.CurrentCulture)
            : null;
    }

    public static byte[]? HexStringToByteArray(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Convert.FromHexString(input) : null;
    }

    public static string? StringToHex(byte[]? buffer)
    {
        return buffer != null
            ? Convert.ToHexString(buffer, 0, buffer.Length).ToUpper(CultureInfo.CurrentCulture)
            : null;
    }

    public static string? ByteArrayToString(byte[]? buffer)
    {
        return buffer != null ? Encoding.UTF8.GetString(buffer) : null;
    }

    public static byte[]? StringToByteArray(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Encoding.UTF8.GetBytes(input) : null;
    }

    public static string? ByteArrayToBase64String(byte[]? buffer)
    {
        return buffer != null ? Convert.ToBase64String(buffer) : null;
    }

    public static byte[]? Base64StringToByteArray(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Convert.FromBase64String(input) : null;
    }
}