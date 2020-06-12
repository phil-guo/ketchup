using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Ketchup.Profession.Utilis
{
    public static class SecurityEncryptExtension
    {
        public static string Get32MD5One(this string source)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                string hash = sBuilder.ToString();
                return hash.ToUpper();
            }
        }
    }
}
