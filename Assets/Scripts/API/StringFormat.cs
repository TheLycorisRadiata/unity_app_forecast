using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

public static class StringFormat
{
    public static string Float(float number)
    {
        NumberFormatInfo nfi = new NumberFormatInfo();
        nfi.NumberDecimalSeparator = ".";
        return number.ToString(nfi);
    }

    // \p{Mn} or \p{Non_Spacing_Mark}: a character intended to be combined with another character without taking up extra space (e.g. accents, umlauts, etc)
    private readonly static Regex nonSpacingMarkRegex = new Regex(@"\p{Mn}", RegexOptions.Compiled);
    public static string RemoveDiacritics(string text)
    {
        if (text == null)
            return string.Empty;

        string normalizedText = text.Normalize(NormalizationForm.FormD);
        return nonSpacingMarkRegex.Replace(normalizedText, string.Empty);
    }
}
