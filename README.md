# 🔐 Secure Password Manager - Research Initiative
This project is a **research initiative** for a **future password manager**, implementing **secure password encryption, storage, and session management**.
The project development began as a way to deepen understanding of secure cryptography for application in a school project.

## 🚀 Features  

✅ **Password Generation** - Generates strong passwords  
✅ **Secure Encryption** - Uses **AES-256-CBC** for password encryption  
✅ **Key Derivation** - Implements **Argon2id** to derive secure encryption keys  
✅ **Session Management** - Caches encryption keys securely and enforces **auto-expiration**  

## 🛠 Technologies Used  

- **C# (.NET Framework/Core)**
- **Argon2id** (via `Konscious.Security.Cryptography`)
- **AES-256-CBC** (for encryption)
- **SQLite** (for secure credential storage)

## 🔑 How It Works  

1. **Password Generation**  
   - Uses `PasswordGenerator` to create strong passwords.  
   - Example:  
     ```csharp
     string password = PasswordGenerator.GetInstance().GeneratePassword(34, 12, 10);
     ```

2. **Encryption & Storage**  
   - Derives a **secure encryption key** from the master password using `Argon2id`.  
   - Encrypts passwords using **AES-256-CBC** and stores them securely in SQLite.  
   - Example:  
     ```csharp
     byte[] key = SecurePasswordManager.DeriveKeyFromPassword(masterPassword, salt);
     byte[] encryptedPassword = SecurePasswordManager.EncryptData(passwordToEncrypt, key, out iv);
     SQLite.SaveEncryptedCredentials(username, salt, iv, encryptedPassword);
     ```

3. **Decryption**  
   - Retrieves stored credentials, derives the key again, and decrypts the password.  
   - Example:  
     ```csharp
     string decryptedPassword = SecurePasswordManager.DecryptData(storedEncryptedPassword, derivedKey, storedIv);
     ```

4. **Session Management**  
   - Encrypts and caches session keys securely using `SecureString`.  
   - Implements a **timeout system** (default: **10 min**) to automatically **clear keys** from memory.  
   - Example:  
     ```csharp
     SessionManager.StartSession(derivedKey, masterPassword);
     ```

## Secure Password Encryption Example

This example demonstrates how to securely encrypt a password using a master password and a derived key.

```csharp
string masterPassword = "SuperSecretMasterPassword";
byte[] salt = SecurePasswordManager.GenerateSalt();  
byte[] key = SecurePasswordManager.DeriveKeyFromPassword(masterPassword, salt);

string passwordToEncrypt = "MySecurePassword123";
byte[] iv;
byte[] encryptedPassword = SecurePasswordManager.EncryptData(passwordToEncrypt, key, out iv);

Console.WriteLine("Encrypted Password: " + Convert.ToBase64String(encryptedPassword));
```

## 🔒 Security Considerations  

✔ Uses **Argon2id** for key derivation (resistant to brute-force attacks).  
✔ Implements **AES-256-CBC** for encryption (secure against most attacks).  
✔ **SecureString & session timeout** to prevent key exposure in memory.  
✔ Random **salt & IV generation** to ensure unique encryption per password.  

## 📌 Future Improvements  

- [ ] **Mandatory 2FA/MFA** with support for TOTP (Google Authenticator)
- [ ] Secure UI for managing credentials  
- [ ] Encrypted backup & restore options  
- [ ] Implement **protection against screen capture** and **key injection** on Windows.
- [ ] Use **signed checksums** to detect executable tampering.

## 📜 License  

This is a **study** project and is provided **AS-IS**. It is not ready for real world usage.
