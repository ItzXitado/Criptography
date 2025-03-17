using System;
using System.Data.SQLite;

namespace Criptography.database
{
    public class SQLite
    {
        private const string connectionString = "Data Source=credentials.db;Version=3;";
        
        
        
        // Initialize the SQLite database and create a table for storing encrypted credentials
        public static void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string createTableQuery = @"CREATE TABLE IF NOT EXISTS Credentials (
                                            Username TEXT PRIMARY KEY,
                                            Salt BLOB NOT NULL,
                                            IV BLOB NOT NULL,
                                            EncryptedPassword BLOB NOT NULL
                                        );";
                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        
        // Save the encrypted credentials into the SQLite database
        public static void SaveEncryptedCredentials(string username, byte[] salt, byte[] iv, byte[] encryptedPassword)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string insertQuery = "INSERT OR REPLACE INTO Credentials (Username, Salt, IV, EncryptedPassword) VALUES (@Username, @Salt, @IV, @EncryptedPassword)";
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Salt", salt);
                    command.Parameters.AddWithValue("@IV", iv);
                    command.Parameters.AddWithValue("@EncryptedPassword", encryptedPassword);
                    command.ExecuteNonQuery();
                }
            }
        }
        
        // Retrieve encrypted credentials from the SQLite database
        public static (byte[] salt, byte[] iv, byte[] encryptedPassword)? GetEncryptedCredentials(string username)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT Salt, IV, EncryptedPassword FROM Credentials WHERE Username = @Username";
                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            byte[] salt = (byte[])reader["Salt"];
                            byte[] iv = (byte[])reader["IV"];
                            byte[] encryptedPassword = (byte[])reader["EncryptedPassword"];
                            return (salt, iv, encryptedPassword);
                        }
                    }
                }
            }
            return null;  // Return null if the username was not found
        }
        
        // Update the encrypted credentials for a specific username
        public static void UpdateEncryptedCredentials(string username, byte[] salt, byte[] iv, byte[] encryptedPassword)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string updateQuery = "UPDATE Credentials SET Salt = @Salt, IV = @IV, EncryptedPassword = @EncryptedPassword WHERE Username = @Username";
                using (var command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Salt", salt);
                    command.Parameters.AddWithValue("@IV", iv);
                    command.Parameters.AddWithValue("@EncryptedPassword", encryptedPassword);
                    command.ExecuteNonQuery();
                }
            }
        }
        
        // Check if credentials exist for a specific username
        public bool Exists(string username)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT COUNT(1) FROM Credentials WHERE Username = @Username";
                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }
        
        // Delete the credentials for a specific username
        public static void DeleteCredentials(string username)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM Credentials WHERE Username = @Username";
                using (var command = new SQLiteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}