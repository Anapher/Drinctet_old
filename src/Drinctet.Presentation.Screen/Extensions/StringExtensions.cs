namespace Drinctet.Presentation.Screen.Extensions
{
    public static class StringExtensions
    {
        public static string StartsWithLowerCase(this string s) => char.ToLower(s[0]) + s.Remove(0, 1);
        public static string StartsWithUpperCase(this string s) => char.ToUpper(s[0]) + s.Remove(0, 1);
    }
}