using Utility.Other.Enums;

namespace Utility.Other.Extensions;

public static class StringExtension
{
    private static string TransformString(string s, Action<List<char>, int> action)
    {
        var chars = s.ToList();
        
        for (var i = 0; i < s.Length; i++)
        {
            action(chars, i);
        }

        return new string(chars.ToArray());
    }
    
    public static string ToCase(this string s, NamingConvention convention)
    {
        switch (convention)
        {
            case NamingConvention.LowerCase:
                s = s.ToLower().Replace("_", "");
                break;
            case NamingConvention.UpperCase:
                s = s.ToUpper().Replace("_", "");
                break;
            case NamingConvention.LowerCaseCamelCase:
                s = char.ToLower(s[0]) + s[1..];
                s = TransformString(s, (chars, i) =>
                {
                    if (chars[i] != '_') return;
                    chars.RemoveAt(i);
                    chars[i] = char.ToUpper(chars[i]);
                });
                break;
            case NamingConvention.SnakeCase:
                s = TransformString(s, (chars, i) =>
                {
                    if (char.IsLower(chars[i]) 
                        || chars[i - 1] == '_' 
                        || i == 0) return;
                    chars.Insert(i, '_'); 
                });
                s = s.ToLower();
                break;
            case NamingConvention.SnakeCaseUpperCase:
                s = TransformString(s, (chars, i) =>
                {
                    if (char.IsLower(chars[i]) 
                        || i == 0) return;
                    if (chars[i] == '_') chars[i + 1] = char.ToUpper(chars[i + 1]);
                    if (char.IsUpper(chars[i])
                        && chars[i - 1] != '_') chars.Insert(i, '_');
                });
                break;
            case NamingConvention.UpperCaseCamelCase:
                s = char.ToUpper(s[0]) + s[1..];
                s = TransformString(s, (chars, i) =>
                {
                    if (chars[i] != '_') return;
                    
                    chars[i + 1] = char.ToUpper(chars[i + 1]);
                    chars.RemoveAt(i);
                });
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(convention), convention, null);
        }

        return s;
    }
}