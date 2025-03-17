using System;
using System.Security.Cryptography;
using System.Windows.Forms;
using Criptography.database;
using Criptography.encryption;
using Criptography.utils;

namespace Criptography
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string password = PasswordGenerator.GetInstance().GeneratePassword(34, 12, 10);
            Console.WriteLine("Your new password: " + password);
            Console.WriteLine("Your new password's security score: " + PassStrength.CheckStrength(password));
            
            SQLite.InitializeDatabase();
            
            string masterPassword = "SuperSecretMasterPassword";
            byte[] salt = SecurePasswordManager.GenerateSalt();  // Generate a random salt
            
            byte[] key = SecurePasswordManager.DeriveKeyFromPassword(masterPassword, salt);
            
            string passwordToEncrypt = "MyPassword1sfsdfsdfsfzx234";
            byte[] iv;
            byte[] encryptedPassword = SecurePasswordManager.EncryptData(passwordToEncrypt, key, out iv);
            
            Console.WriteLine("IV-> " + iv);
            string username = "user1";
            SQLite.SaveEncryptedCredentials(username, salt, iv, encryptedPassword);
            Console.WriteLine("Password encrypted and saved to database.");
            
            var storedData = SQLite.GetEncryptedCredentials("user1");
            if (storedData.HasValue)
            {
                byte[] storedSalt = storedData.Value.salt;
                byte[] storedIv = storedData.Value.iv;
                byte[] storedEncryptedPassword = storedData.Value.encryptedPassword;

                // Step 6: Derive the key again using the retrieved salt
                byte[] derivedKey = SecurePasswordManager.DeriveKeyFromPassword(masterPassword, storedSalt);

                try
                {
                    // Step 7: Decrypt the password
                    string decryptedPassword = SecurePasswordManager.DecryptData(storedEncryptedPassword, derivedKey, storedIv);
                    Console.WriteLine($"Decrypted Password: {decryptedPassword}");
                }
                catch (CryptographicException)
                {
                    // This will catch any issues with decryption (likely due to a wrong master password)
                    Console.WriteLine("Failed to decrypt. Incorrect master password.");
                }
            }
            else
            {
                Console.WriteLine("Credentials not found.");
            }

        }
    }
}