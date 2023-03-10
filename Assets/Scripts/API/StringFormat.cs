using System.Globalization;

public static class StringFormat
{
    public static string Float(float number)
    {
        NumberFormatInfo nfi = new NumberFormatInfo();
        nfi.NumberDecimalSeparator = ".";
        return number.ToString(nfi);
    }
}
