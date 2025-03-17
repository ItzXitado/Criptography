using System;
using System.Security;
using System.Timers;

public class SessionManager
{
    private static byte[] cachedMasterKey = null;  // Holds the derived master key
    private static SecureString cachedMasterPassword = null;  // Securely holds the master password
    private static Timer sessionTimer;  // Timer to track session timeout
    private static readonly int sessionTimeoutMinutes = 10;  // Session timeout duration (10 minutes)

    // Initialize the session with a key and start the session timer
    public static void StartSession(byte[] masterKey, string masterPassword)
    {
        // Secure the master password in memory
        cachedMasterPassword = new SecureString();
        foreach (char c in masterPassword)
        {
            cachedMasterPassword.AppendChar(c);
        }
        cachedMasterPassword.MakeReadOnly();  // Make the SecureString immutable

        // Cache the derived master key
        cachedMasterKey = masterKey;

        // Start the session timer
        StartSessionTimer();
    }

    // Returns the cached master key if the session is active
    public static byte[] GetCachedMasterKey()
    {
        if (cachedMasterKey == null)
        {
            throw new InvalidOperationException("Session has expired. Please re-enter the master password.");
        }

        // Reset the session timer on each access
        ResetSessionTimer();

        return cachedMasterKey;
    }

    // Securely clear the cached key and password
    public static void EndSession()
    {
        if (cachedMasterPassword != null)
        {
            cachedMasterPassword.Dispose();  // Clears the SecureString from memory
            cachedMasterPassword = null;
        }

        if (cachedMasterKey != null)
        {
            Array.Clear(cachedMasterKey, 0, cachedMasterKey.Length);  // Clear the key from memory
            cachedMasterKey = null;
        }

        // Stop the session timer
        StopSessionTimer();
    }

    // Starts the session timer that will expire the session after inactivity
    private static void StartSessionTimer()
    {
        sessionTimer = new Timer(sessionTimeoutMinutes * 60 * 1000);  // Convert minutes to milliseconds
        sessionTimer.Elapsed += OnSessionTimeout;
        sessionTimer.AutoReset = false;  // Timer should only fire once after the timeout
        sessionTimer.Start();
    }

    // Stops the session timer
    private static void StopSessionTimer()
    {
        if (sessionTimer != null)
        {
            sessionTimer.Stop();
            sessionTimer.Dispose();
            sessionTimer = null;
        }
    }

    // Resets the session timer whenever the session is accessed
    private static void ResetSessionTimer()
    {
        if (sessionTimer != null)
        {
            sessionTimer.Stop();
            sessionTimer.Start();
        }
    }

    // This method is called when the session times out
    private static void OnSessionTimeout(object source, ElapsedEventArgs e)
    {
        EndSession();
        Console.WriteLine("Session has expired due to inactivity.");
    }
}
