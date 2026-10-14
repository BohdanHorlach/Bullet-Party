using System.Security.Cryptography;


namespace General.Utils
{
    public static class CodeGenerator
    {
        private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string GenerateCode(int length = 6)
        {
            var bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);

            var chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = CHARS[bytes[i] % CHARS.Length];
            }

            return new string(chars);
        }
    }
}