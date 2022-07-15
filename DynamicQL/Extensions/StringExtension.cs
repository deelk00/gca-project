namespace DynamicQL.Extensions;

public static class StringExtension
{
    public static string ToLowerCaseCamelCase(this string s)
    {
        return s.ToLower()[0] + s[1..];
    }
    
    public static string ToUpperCaseCamelCase(this string s)
    {
        return s.ToUpper()[0] + s[1..];
    }
}