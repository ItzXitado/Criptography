using System;
using System.Text;

public class PasswordGenerator
{
    
    private static PasswordGenerator instance;
    public static PasswordGenerator GetInstance() { return instance ?? (instance = new PasswordGenerator()); }
        
    private static readonly Random random = new Random();
    private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private const string Numbers = "0123456789";
    private const string Symbols = "!@#$%^&*()_+-=[]{}|;:,.<>?";

    public string GeneratePassword(int length, int symbolLength = 0, int numberLength = 0)
    {
        if (symbolLength + numberLength > length)
            throw new ArgumentException("Sum of symbol and number lengths cannot exceed the total length");
        
        int letterLength = length - symbolLength - numberLength;
        StringBuilder passwordBuilder = new StringBuilder();
        
        passwordBuilder.Append(GenerateRandomCharacters(Letters, letterLength));
        
        passwordBuilder.Append(GenerateRandomCharacters(Numbers, numberLength));
        
        passwordBuilder.Append(GenerateRandomCharacters(Symbols, symbolLength));
        
        return ShuffleString(passwordBuilder.ToString());
    }

    private string GenerateRandomCharacters(string characterSet, int length)
    {
        StringBuilder result = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            int index = random.Next(characterSet.Length);
            result.Append(characterSet[index]);
        }
        return result.ToString();
    }

    private string ShuffleString(string input)
    {
        char[] array = input.ToCharArray();
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
        return new string(array);
    }
}