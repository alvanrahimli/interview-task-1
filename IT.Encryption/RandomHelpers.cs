namespace IT.Encryption;

public class RandomHelpers
{
    private static readonly Random Random = new Random();
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const string Digits = "0123456789";

    public static string RandomString(int length)
    {
        return new string(Enumerable.Repeat(Chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    public static int RandomCode(int length)
    {
        var codeStr = new string(Enumerable.Repeat(Digits, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
        return int.Parse(codeStr);
    }
}