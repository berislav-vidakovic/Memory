using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Shared
{
    public class HashUtil
    {
        public static string HashClient(string password)
        {
            using var sha1 = SHA1.Create();
            var bytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public static string GenerateSalt(int size = 16)
        {
            var bytes = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static string HashPasswordServer(string clientHash)
        {
            const int saltSize = 16;     // 128-bit salt
            const int keySize = 32;      // 256-bit derived key
            const int iterations = 100_000;

            // Generate salt
            byte[] salt = RandomNumberGenerator.GetBytes(saltSize);

            // PBKDF2 derive key
            byte[] key = Rfc2898DeriveBytes.Pbkdf2(
                password: Encoding.UTF8.GetBytes(clientHash),
                salt: salt,
                iterations: iterations,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: keySize
            );

            // Combine salt + key into one byte array
            byte[] result = new byte[saltSize + keySize];
            Buffer.BlockCopy(salt, 0, result, 0, saltSize);
            Buffer.BlockCopy(key, 0, result, saltSize, keySize);

            return Convert.ToBase64String(result);
        }

        public static bool VerifyPasswordServer(string clientHash, string storedHash)
        {
            const int saltSize = 16;
            const int keySize = 32;
            const int iterations = 100_000;

            byte[] storedBytes = Convert.FromBase64String(storedHash);

            byte[] salt = new byte[saltSize];
            byte[] storedKey = new byte[keySize];

            Buffer.BlockCopy(storedBytes, 0, salt, 0, saltSize);
            Buffer.BlockCopy(storedBytes, saltSize, storedKey, 0, keySize);

            byte[] computedKey = Rfc2898DeriveBytes.Pbkdf2(
                password: Encoding.UTF8.GetBytes(clientHash),
                salt: salt,
                iterations: iterations,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: keySize
            );

            return CryptographicOperations.FixedTimeEquals(storedKey, computedKey);
        }


    }
}
