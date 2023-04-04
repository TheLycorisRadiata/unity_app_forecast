using System;
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

    public static string RemoveExtraSpaces(string str)
    {
        /* Remove leading and trailing spaces */
        str = str.Trim();
        /* Remove multiple spaces to leave only single ones */
        str = Regex.Replace(str, @"\s+", " ");
        return str;
    }

    public static bool WordContainsWord(string str, string subStr)
    {
        return RemoveDiacritics(str.ToLower()).Contains(RemoveDiacritics(subStr.ToLower()));
    }

    public static bool WordComparison(string str1, string str2)
    {
        return string.Equals(RemoveDiacritics(str1), RemoveDiacritics(str2), StringComparison.OrdinalIgnoreCase);
    }
}
