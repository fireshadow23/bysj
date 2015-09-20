using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Yyx.BS.Utils
{
    public static class CryptographyUtils
    {
        // Fields
        private const string key = "YyxKey";

        // Methods
        public static string DecryptString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            input = EncryptKey(Encoding.Default.GetString(Convert.FromBase64String(input)));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                builder.Append(((char) (input[i] ^ input[++i])).ToString());
            }
            return builder.ToString();
        }

        private static string EncryptKey(string input)
        {
            string str = Md5Encrypt(key);
            int num = 0;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                num = (num == str.Length) ? 0 : num;
                builder.Append(((char) (input[i] ^ str[num++])).ToString());
            }
            return builder.ToString();
        }

        public static string EncryptString(object inputValue)
        {
            string str = inputValue.ToString();
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            Random random = new Random((int) DateTime.Now.Ticks);
            //string str2 = Md5Encrypt(random.Next(0, 0x7d00).ToString());
            string str2 = Md5Encrypt(key);
            int num2 = 0;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                num2 = (num2 == str2.Length) ? 0 : num2;
                builder.Append(str2[num2].ToString() + ((char) (str[i] ^ str2[num2++])).ToString());
            }
            return Convert.ToBase64String(Encoding.Default.GetBytes(EncryptKey(builder.ToString())));
        }

        public static byte[] GetSHA256(string input)
        {
            byte[] buffer;
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }
            SHA256Managed managed = new SHA256Managed();
            try
            {
                buffer = managed.ComputeHash(Encoding.Default.GetBytes(input));
            }
            finally
            {
                managed.Clear();
            }
            return buffer;
        }

        private static string Md5Encrypt(string input)
        {
            StringBuilder builder = new StringBuilder();
            using (MD5 md = new MD5CryptoServiceProvider())
            {
                byte[] bytes = Encoding.Default.GetBytes(input);
                foreach (byte num in md.ComputeHash(bytes))
                {
                    string str = num.ToString("x", CultureInfo.CurrentCulture);
                    if (str.Length == 1)
                    {
                        str = str.PadLeft(2, '0');
                    }
                    builder.Append(str);
                }
                md.Clear();
            }
            return builder.ToString();
        }

        public static bool VerfiySHA256Hash(string input, byte[] hash)
        {
            byte[] buffer = GetSHA256(input);
            if ((hash == null) || (buffer == null))
            {
                return false;
            }
            if (hash.Length != buffer.Length)
            {
                return false;
            }
            for (int i = 0; i < hash.Length; i++)
            {
                if (hash[i] != buffer[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static string Id2No(string id)
        {
            id = Regex.Replace(id, "[a-zA-Z]+", "");//去除字母

            uint a = uint.Parse(id) ^ 0x8fffffff;
            uint b = uint.Parse(id) & 0x7777777f;

            uint x = a * 51788888 + b;
            return x.ToString("0000000000");
        }
    }

}