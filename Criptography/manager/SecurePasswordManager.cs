using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace Criptography.encryption
{
    public class SecurePasswordManager
    {
        
        // Generate a random salt
        public static byte[] GenerateSalt(int size = 16)
        {
            byte[] salt = new byte[size];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }


        // Derive the key using Argon2id
        public static byte[] DeriveKeyFromPassword(string password, byte[] salt, int keySize = 32, int iterations = 10, int memorySize = 524288, int degreeOfParallelism = 8)
        {
            var argon2 = new Konscious.Security.Cryptography.Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                Iterations = iterations,                 // t = 3
                MemorySize = memorySize,                 // 64 MB = 65536 KB
                DegreeOfParallelism = degreeOfParallelism // p = 4
            };                                            //My default values, for now im using 524MB bcuz im the best :)

            return argon2.GetBytes(keySize);  // Key size in bytes (e.g., 32 bytes for AES-256)
        }

        // Encrypt data using AES-256-CBC
        public static byte[] EncryptData(string plainText, byte[] key, out byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV(); // Generate a new IV for each encryption
                aes.Mode = CipherMode.CBC;

                iv = aes.IV; // Pass out the IV

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                    }

                    return ms.ToArray(); // Return encrypted data as byte array
                }
            }
        }

        // Decrypt data using AES-256-CBC
        public static string DecryptData(byte[] cipherText, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream(cipherText))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd(); // Return the decrypted plain text
                        }
                    }
                }
            }
        }
    }
}