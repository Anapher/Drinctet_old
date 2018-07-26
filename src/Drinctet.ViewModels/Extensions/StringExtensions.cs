using System.Linq;

namespace Drinctet.ViewModels.Extensions
{
    public static class StringExtensions
    {
        private static readonly char[] TerminalChars = {'.', '!', '?'};

        public static string StartsWithLowerCase(this string s) => char.ToLower(s[0]) + s.Remove(0, 1);
        public static string StartsWithUpperCase(this string s) => char.ToUpper(s[0]) + s.Remove(0, 1);

        public static string HasTerminalCharacter(this string s)
        {
            if (TerminalChars.Contains(s.Last()))
                return s;

            return s + ".";
        }

        public static string NoTerminalPoint(this string s) => s.TrimEnd(TerminalChars);
    }
}