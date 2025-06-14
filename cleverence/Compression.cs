using System.Text;

public class Compression
{
    public static string Compress(string s)
    {
        if (string.IsNullOrEmpty(s))
            return string.Empty;

        StringBuilder result = new StringBuilder();
        char currentChar = s[0];
        int count = 1;

        for (int i = 1; i < s.Length; i++)
        {
            if (s[i] == currentChar)
            {
                count++;
            }
            else
            {
                AppendChar(result, currentChar, count);
                currentChar = s[i];
                count = 1;
            }
        }
        AppendChar(result, currentChar, count);
        return result.ToString();
    }

    private static void AppendChar(StringBuilder sb, char c, int count)
    {
        sb.Append(c);
        if (count > 1)
            sb.Append(count);
    }

    
}

