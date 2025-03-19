using System.Security.Cryptography;
using System.Text;

namespace InternetSentry.Utils
{
    public static class CryptoUtils
    {
        public static string PBKDF2AndHexEncode(string password, string salt, int count = 1000, int length = 128)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            using Rfc2898DeriveBytes pbkdf2 = new(passwordBytes, saltBytes, count, HashAlgorithmName.SHA256);
            int byteLength = (int)Math.Ceiling(length / 8.0);

            byte[] keyBytes = pbkdf2.GetBytes(byteLength);
            return ByteArrayToHexString(keyBytes);
        }

        public static string ByteArrayToHexString(byte[] bytes)
        {
            StringBuilder hex = new(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }
    }
}
