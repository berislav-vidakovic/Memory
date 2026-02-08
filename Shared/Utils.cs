using System;
using System.Security.Cryptography;
using System.Text;

namespace Shared
{
    public static class HashUtil
    {
        // Client-side hash (kept)
        public static string HashClient(string password)
        {
            using var sha1 = SHA1.Create();
            var bytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        // Generate random salt
        public static string GenerateSalt(int size = 16)
        {
            var bytes = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        // Server-side hash using HMAC-SHA256 with salt
        public static string HashPasswordServer(string clientHash)
        {
            string salt = GenerateSalt(); // 16-byte random salt
            using var hmac = new HMACSHA256(Convert.FromBase64String(salt));
            byte[] key = hmac.ComputeHash(Encoding.UTF8.GetBytes(clientHash));

            // Store as Base64(salt + hash)
            byte[] result = new byte[16 + key.Length];
            Array.Copy(Convert.FromBase64String(salt), 0, result, 0, 16);
            Array.Copy(key, 0, result, 16, key.Length);

            return Convert.ToBase64String(result);
        }

        // Verify password
        public static bool VerifyPasswordServer(string clientHash, string storedHash)
        {
            if (string.IsNullOrEmpty(storedHash))
                return false;

            byte[] storedBytes;
            try
            {
                storedBytes = Convert.FromBase64String(storedHash);
            }
            catch
            {
                return false;
            }

            if (storedBytes.Length < 16)
                return false;

            // Extract salt
            byte[] salt = new byte[16];
            Array.Copy(storedBytes, 0, salt, 0, 16);

            // Extract stored key
            int keyLength = storedBytes.Length - 16;
            byte[] storedKey = new byte[keyLength];
            Array.Copy(storedBytes, 16, storedKey, 0, keyLength);

            // Recompute key with same salt
            using var hmac = new HMACSHA256(salt);
            byte[] computedKey = hmac.ComputeHash(Encoding.UTF8.GetBytes(clientHash));

            return CryptographicOperations.FixedTimeEquals(storedKey, computedKey);
        }
    }
}
