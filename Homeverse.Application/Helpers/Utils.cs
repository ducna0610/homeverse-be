using System.Text;

namespace Homeverse.Application.Helpers;

public static class Utils
{
    public static string GenerateRandomString(int size)
    {
        var chars = "0123456789abcdefghijklmnopqrstuvwxyz!@#$%";
        var output = new StringBuilder();
        var random = new Random();
        for (int i = 0; i < size; i++)
        {
            output.Append(chars[random.Next(chars.Length)]);
        }

        return output.ToString();
    }
}
