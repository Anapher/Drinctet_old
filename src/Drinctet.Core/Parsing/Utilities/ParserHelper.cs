#if FEATURE_SPAN
using System;
#endif
using System.Collections.Generic;
using System.Linq;
using Drinctet.Core.Cards.Base;

namespace Drinctet.Core.Parsing.Utilities
{
    internal static class ParserHelper
    {
#if FEATURE_SPAN
        public static bool ParsePlayerTag(ReadOnlySpan<char> s, out int index)
        {
            const string tagName = "Player";

            if (!s.StartsWith(tagName))
            {
                index = -1;
                return false;
            }

            if (s.Length > tagName.Length)
                return int.TryParse(s.Slice(tagName.Length), out index);

            index = 1;
            return true;
        }
#else
        public static bool ParsePlayerTag(string s, out int index)
        {
            const string tagName = "Player";

            if (!s.StartsWith(tagName))
            {
                index = -1;
                return false;
            }

            if (s.Length > tagName.Length)
                return int.TryParse(s.Substring(tagName.Length), out index);

            index = 1;
            return true;
        }
#endif

        private static readonly IReadOnlyDictionary<string, RequiredGender> RequiredGenderMap =
            new Dictionary<string, RequiredGender>
            {
                {"m", RequiredGender.Male},
                {"male", RequiredGender.Male},
                {"f", RequiredGender.Female},
                {"female", RequiredGender.Female},
                {"o", RequiredGender.Opposite},
                {"opposite", RequiredGender.Opposite},
                {"s", RequiredGender.Same},
                {"same", RequiredGender.Same},
            };

        public static bool ParseRequiredGender(string s, out RequiredGender gender)
        {
            if (RequiredGenderMap.TryGetValue(s, out gender))
                return true;

            return false;
        }

        public static string GetRequiredGenderShorcut(RequiredGender requiredGender)
        {
            return RequiredGenderMap.Where(x => x.Value == requiredGender).OrderBy(x => x.Key.Length).First().Key;
        }

        public static IReadOnlyList<TEnum> ParseEnum<TEnum>(string value) where TEnum : struct
        {
            var result = new List<TEnum>();

            var values = value.Split(',');
            foreach (var v in values)
                result.Add(Enum.Parse<TEnum>(v, true));

            return result;
        }
    }
}