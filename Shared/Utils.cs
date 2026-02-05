using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Shared
{
    public class Utils
    {
        public static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
