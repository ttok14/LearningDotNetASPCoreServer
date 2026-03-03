using System.Security.Cryptography;
using System.Text;

namespace LearningServer01.Common
{
    public static class Helper
    {
        public static string GetHash(byte[] bytes)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(bytes);
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

                return hashString;
            }
        }

        public static string GetHash(string str)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(str));
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

                return hashString;
            }
        }
    }
}
