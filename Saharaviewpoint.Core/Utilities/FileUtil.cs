namespace Saharaviewpoint.Core.Utilities;

public static class FileUtil
{
    public static string ToFolderName(this string value)
    {
            string result = value.ToLower().Trim().Replace(" ", "-");

            // remove any duplicate hyphen in the string
            while (result.Contains("--"))
            {
                result = result.Replace("--", "-");
            }

            // reduce the result if it's larger than 50 characters, result should not end with hypen
            if (result.Length > 50)
            {
                result = result[..50];
                if (result.EndsWith("-"))
                {
                    // remove the last character
                    result = result[..^1];
                }
            }

            return result;
        }
}