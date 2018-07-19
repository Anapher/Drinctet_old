using System;
using System.Collections.Generic;
using Drinctet.Core.Fragments;
using Drinctet.Core.Logging;
using Drinctet.Core.Parsing.Utilities;

namespace Drinctet.Core.Parsing.TextDecoder
{
    public static class TextDecoderConsts
    {
        public const char VarStartChar = '[';
        public const char VarEndChar = ']';
        public const char EscapeChar = '\\';
        public const char SelectionStartChar = '{';
        public const char SelectionModifierStartChar = '!';
        public const char SelectionEndChar = '}';

        public const string PlayerVariable = "Player";
        public const string SipsVariable = "Sips";
        public const char VariableParametersStart = ':';

        public const char SelectionSplitterChar = '/';
        public const char SelectionReferenceChar = '|';
    }

    public class DefaultTextDecoder
    {
        private static readonly ILog Logger = LogProvider.For<DefaultTextDecoder>();

        public IEnumerable<TextFragment> Decode(string s)
        {
            var value = s.AsSpan();
            var index = 0;
            var lastTokenIndex = 0;

            do
            {
                if (value[index] == TextDecoderConsts.VarStartChar)
                {
                    if (lastTokenIndex != index)
                        yield return new RawTextFragment(new string(value.Slice(lastTokenIndex, index - lastTokenIndex)));

                    var content = ReadToken(value, TextDecoderConsts.VarEndChar, ref index);
                    yield return ParseVariableFragment(content);
                }
                else if (value[index] == TextDecoderConsts.SelectionStartChar)
                {
                    if (index > 1 && value[index - 1] == TextDecoderConsts.SelectionModifierStartChar)
                    {
                        index--;
                        if (lastTokenIndex != index)
                            yield return new RawTextFragment(new string(value.Slice(lastTokenIndex, index - lastTokenIndex)));

                        index++;
                        var content = ReadToken(value, TextDecoderConsts.SelectionEndChar, ref index);
                        yield return ParseRandomSelectionFragment(content);
                    }
                    else
                    {
                        if (lastTokenIndex != index)
                            yield return new RawTextFragment(new string(value.Slice(lastTokenIndex, index - lastTokenIndex)));

                        var content = ReadToken(value, TextDecoderConsts.SelectionEndChar, ref index);
                        yield return ParseGenderSelectionFragment(content);
                    }
                }

            } while (++index < s.Length);
        }

        private static TextFragment ParseVariableFragment(ReadOnlySpan<char> content)
        {
            if (content.StartsWith(TextDecoderConsts.PlayerVariable, StringComparison.OrdinalIgnoreCase))
            {
                // Samples:
                // [Player1:f]
                // [Player1]
                // [Player]

                var playerReference = new PlayerReferenceFragment();

                var parameterBegin = content.IndexOf(TextDecoderConsts.VariableParametersStart);

                ReadOnlySpan<char> playerTag;
                if (parameterBegin == -1)
                {
                    playerTag = content;
                }
                else
                {
                    playerTag = content.Slice(0, parameterBegin);

                    var gender = content.Slice(parameterBegin + 1, content.Length - 1 - parameterBegin);
                    if (ParserHelper.ParseRequiredGender(new string(gender), out var requiredGender))
                        playerReference.RequiredGender = requiredGender;
                    else throw new ArgumentException("Gender parameter of player tag could not be parsed: " + new string(content));
                }

                if (playerTag.Length > TextDecoderConsts.PlayerVariable.Length)
                    playerReference.PlayerIndex = int.Parse(content.Slice(TextDecoderConsts.PlayerVariable.Length));

                return playerReference;
            }

            if (content.StartsWith(TextDecoderConsts.SipsVariable, StringComparison.OrdinalIgnoreCase))
            {
                var sipsFragment = new SipsFragment();

                var parameterBegin = content.IndexOf(TextDecoderConsts.VariableParametersStart);

                ReadOnlySpan<char> sipsTag;
                if (parameterBegin == -1)
                {
                    sipsTag = content;
                }
                else
                {
                    sipsTag = content.Slice(0, parameterBegin);
                    sipsFragment.MinSips = int.Parse(content.Slice(parameterBegin + 1));
                }

                if (sipsTag.Length > TextDecoderConsts.SipsVariable.Length)
                    sipsFragment.SipsIndex = int.Parse(content.Slice(TextDecoderConsts.SipsVariable.Length));

                return sipsFragment;
            }
            return null;
        }

        private static TextFragment ParseRandomSelectionFragment(ReadOnlySpan<char> content)
        {
            // Samples:
            // !{12,54,56-90}
            // !{12,hello,not,19}

            var isNumericSelection = true;
            for (int i = 0; i < content.Length; i++)
            {
                if (char.IsLetter(content[i]))
                {
                    isNumericSelection = false;
                    break;
                }
            }

            if (!isNumericSelection)
            {
                return new RandomTextFragment{Texts = new string(content).Split(',', StringSplitOptions.RemoveEmptyEntries)};
            }
        }

        private static TextFragment ParseGenderSelectionFragment(ReadOnlySpan<char> content)
        {
            var fragment = new GenderBasedSelectionFragment();

            var splitterIndex = content.IndexOf(TextDecoderConsts.SelectionSplitterChar);
            var reference = content.IndexOf(TextDecoderConsts.SelectionReferenceChar);

            if (reference > -1)
            {
                fragment.ReferencedPlayerIndex = int.Parse(content.Slice(reference + 1));
                content = content.Slice(0, reference);
            }

            if (splitterIndex == -1)
                fragment.FemaleText = new string(content);
            else
            {
                fragment.FemaleText = new string(content.Slice(0, splitterIndex));
                fragment.MaleText = new string(content.Slice(splitterIndex + 1, content.Length - splitterIndex - 1));
            }

            return fragment;
        }

        private static ReadOnlySpan<char> ReadToken(ReadOnlySpan<char> value, char endChar, ref int i)
        {
            var buf = value;
            var valueLength = value.Length;
            var tokenStartPos = i;

            while (++i < valueLength)
            {
                var valueChar = buf[i];

                if (valueChar == TextDecoderConsts.EscapeChar)
                {
                    i++;
                    continue;
                }

                if (valueChar == endChar)
                {
                    i++;
                    break;
                }
            }

            return value.Slice(tokenStartPos + 1, i - tokenStartPos - 1);
        }

        internal static IEnumerable<string> SplitQuoted(ReadOnlySpan<char> value, char delimiter)
        {
            var tokenStart = 0;

            while (value.Length > tokenStart - 1)
            {
                var withinQuotes = false;

                if (value[tokenStart] == '"')
                {
                    withinQuotes = true;
                    tokenStart++;
                }

                for (int i = tokenStart; i < value.Length; i++)
                {
                    if (value[i] == delimiter)
                    {
                        if (withinQuotes)
                            continue;

                        yield return new string(value.Slice(tokenStart, i - tokenStart));
                        tokenStart = i + 1;
                        break;
                    }
                    else if (value[i] == '"')
                    {
                        if (!withinQuotes)
                            throw new ArgumentException("Invalid quotes in the middle of the field.");

                        var nextChar = value[i + 1];
                        if (nextChar == '"')
                            continue; //escpaed quotes

                        withinQuotes = false;
                        if (nextChar != delimiter)
                            throw new ArgumentException("The delimiter must come after the closing quotes.");
                    }
                }
            }

        }
    }
}