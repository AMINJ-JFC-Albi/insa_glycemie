using System.Text.RegularExpressions;

public class TextTool
{
    public static bool CompareTexts(string text1, string text2)
    {
        return ExtractText(text1) == ExtractText(text2);
    }

    public static string ExtractText(string input)
    {
        string pattern = @"^(.*?)\s*\(";
        Match match = Regex.Match(input, pattern);
        return match.Success ? match.Groups[1].Value.Trim().ToLower() : input.Trim().ToLower();
    }
}
