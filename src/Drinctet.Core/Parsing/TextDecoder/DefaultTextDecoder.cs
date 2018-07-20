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
        public const char ArrayDelimiter = ',';
    }

    public class DefaultTextDecoder
    {
        public IReadOnlyList<TextFragment> Decode(string s)
        {
            var result = new List<TextFragment>();

            var value = s.AsSpan();
            var index = 0;
            var lastTokenIndex = 0;

            do
            {
                if (value[index] == TextDecoderConsts.VarStartChar)
                {
                    if (lastTokenIndex != index)
                        result.Add(new RawTextFragment(new string(value.Slice(lastTokenIndex, index - lastTokenIndex))));

                    var content = ReadToken(value, TextDecoderConsts.VarEndChar, ref index);
                    result.Add(ParseVariableFragment(content));
                }
                else if (value[index] == TextDecoderConsts.SelectionStartChar)
                {
                    if (index > 1 && value[index - 1] == TextDecoderConsts.SelectionModifierStartChar)
                    {
                        index--;
                        if (lastTokenIndex != index)
                            result.Add(new RawTextFragment(new string(value.Slice(lastTokenIndex,
                                index - lastTokenIndex))));

                        index++;
                        var content = ReadToken(value, TextDecoderConsts.SelectionEndChar, ref index);
                        result.Add(ParseRandomSelectionFragment(content));
                    }
                    else
                    {
                        if (lastTokenIndex != index)
                            result.Add(new RawTextFragment(new string(value.Slice(lastTokenIndex,
                                index - lastTokenIndex))));

                        var content = ReadToken(value, TextDecoderConsts.SelectionEndChar, ref index);
                        result.Add(ParseGenderSelectionFragment(content));
                    }
                }
                else continue;

                lastTokenIndex = index;
            } while (++index < s.Length);

            if (lastTokenIndex != s.Length)
                result.Add(new RawTextFragment(new string(value.Slice(lastTokenIndex,
                    index - lastTokenIndex))));

            return result;
        }

        internal static TextFragment ParseVariableFragment(ReadOnlySpan<char> content)
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
                    playerReference.PlayerIndex = int.Parse(playerTag.Slice(TextDecoderConsts.PlayerVariable.Length));

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
                    sipsFragment.SipsIndex = int.Parse(sipsTag.Slice(TextDecoderConsts.SipsVariable.Length));

                return sipsFragment;
            }
            return null;
        }

        internal static TextFragment ParseRandomSelectionFragment(ReadOnlySpan<char> content)
        {
            // Samples:
            // 12,54,56-90
            // 12,hello,not,19

            var isNumericSelection = true;
            for (int i = 0; i < content.Length; i++)
            {
                if (char.IsLetter(content[i]) || content[i] == '"')
                {
                    isNumericSelection = false;
                    break;
                }
            }

            if (!isNumericSelection)
            {
                return new RandomTextFragment
                {
                    Texts = SplitQuoted(content, TextDecoderConsts.ArrayDelimiter)
                };
            }

            var numbers = ParseNumberArray(content);
            return new RandomNumberFragment{Numbers = numbers};
        }

        internal static IReadOnlyList<INumber> ParseNumberArray(ReadOnlySpan<char> value)
        {
            var result = new List<INumber>();
            INumber currentNumber = null;

            var numberStart = 0;
            var isRangeToken = false;

            var i = 0;
            while (true)
            {
                for (; i < value.Length; i++)
                {
                    var c = value[i];

                    if (char.IsDigit(c))
                        continue;

                    if (c == '-')
                    {
                        if (isRangeToken)
                            throw new ArgumentException("Can only have one range identifier per field");
                        isRangeToken = true;

                        currentNumber = new NumberRange();
                        ((NumberRange)currentNumber).Min = int.Parse(value.Slice(numberStart, i - numberStart));
                        numberStart = i + 1;
                        continue;
                    }

                    if (c == ',')
                        break;

                    throw new ArgumentException("Invalid character found: " + c);
                }

                if (i == numberStart)
                    throw new ArgumentException($"A number was expected at position {i} in string '{value.ToString()}'");

                    var num = int.Parse(value.Slice(numberStart, i - numberStart));
                if (isRangeToken)
                {
                    ((NumberRange)currentNumber).Max = num;
                }
                else currentNumber = new StaticNumber(num);

                result.Add(currentNumber);

                if (i == value.Length)
                    break;

                currentNumber = null;
                isRangeToken = false;
                numberStart = ++i;
            }

            return result;
        }

        internal static TextFragment ParseGenderSelectionFragment(ReadOnlySpan<char> content)
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
                if (splitterIndex != 0)
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

            return value.Slice(tokenStartPos + 1, i - tokenStartPos - 2);
        }

        internal static IReadOnlyList<string> SplitQuoted(ReadOnlySpan<char> value, char delimiter)
        {
            var tokenStart = 0;
            var result = new List<string>();

            while (value.Length > tokenStart - 1)
            {
                var withinQuotes = false;

                if (value[tokenStart] == '"')
                {
                    withinQuotes = true;
                    tokenStart++;
                }

                var i = tokenStart;
                do
                {
                    if (value[i] == delimiter)
                    {
                        if (withinQuotes)
                            continue;

                        result.Add(new string(value.Slice(tokenStart, i - tokenStart)));
                        tokenStart = i + 1;
                        break;
                    }
                    else if (value[i] == '"')
                    {
                        if (!withinQuotes)
                            continue; //allow quotes in the middle
                        
                        if (i == value.Length - 1) //if its the last char
                        {
                            result.Add(new string(value.Slice(tokenStart, i - tokenStart)).Replace("\"\"", "\""));
                            return result;
                        }

                        var nextChar = value[i + 1];
                        if (nextChar == '"')
                        {
                            i++;
                            continue; //escaped quotes
                        }

                        if (nextChar != delimiter)
                            throw new ArgumentException("The delimiter must come after the closing quotes.");

                        result.Add(new string(value.Slice(tokenStart, i - tokenStart)).Replace("\"\"", "\""));
                        tokenStart = i + 2;
                        break;
                    }

                    if (i == value.Length - 1)
                    {
                        if (withinQuotes)
                            throw new ArgumentException("The text must end with a quote");

                        result.Add(new string(value.Slice(tokenStart, i - tokenStart + 1)));
                        return result;
                    }
                } while (++i < value.Length);
            }

            return result;
        }
    }
}