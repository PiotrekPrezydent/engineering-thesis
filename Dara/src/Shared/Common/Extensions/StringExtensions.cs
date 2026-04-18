namespace Dara.Shared.Common.Extensions;

public static class StringExtensions
{
    public static string EnsureText(this string? value)
    {
        if (value == null)
            return "NULL-STRING";
        
        if(value.Length == 0)
            return "EMPTY-STRING";
        
        if(string.IsNullOrWhiteSpace(value))
            return "WHITESPACE-WHITESPACE";
        
        return value;
    }
}