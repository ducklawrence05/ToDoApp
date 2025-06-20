using System.Security.Cryptography;
using System.Text;

namespace ToDoApp.Application.Helpers
{
    public class HashHelper
    {
        public static string BCryptHash(string input)
        {
            return BCrypt.Net.BCrypt.HashPassword(input);
        }

        public static bool BCryptVerify(string input, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(input, hash);
        }

        public static  string Hash256(string input)
        {
            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            string hash = Convert.ToHexString(hashBytes);
            return hash;
        }

        public static string GenerateRandomString(int length)
        {
            var str = new StringBuilder();
            var random = new Random();

            for (int i = 0; i < length; i++)
            {
                str.Append((char)random.Next('a', 'z'));
            }

            return str.ToString();
        }
    }
}
