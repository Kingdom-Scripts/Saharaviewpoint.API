namespace Saharaviewpoint.Core.Utilities
{
    public static class CodeGenerator
    {
        public static string GenerateCode(int length = 20)
        {
            // generate a random code using the alphabetical characters and 0-9
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var code = new string(Enumerable.Repeat(chars, length)
                             .Select(s => s[random.Next(s.Length)]).ToArray());
            return code;
        }
    }
}
