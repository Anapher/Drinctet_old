#if !FEATURE_SPAN
using System;
using System.Collections.Generic;
using Drinctet.Core.Fragments;
using Drinctet.Core.Parsing.Utilities;

namespace Drinctet.Core.Parsing.TextDecoder
{
    public class DefaultTextDecoder : ITextDecoder
    {
        public IReadOnlyList<TextFragment> Decode(string s)
        {
            var result = new List<TextFragment>();

            var value = s;
            var index = 0;
            var lastTokenIndex = 0;

            do
            {
                if (value[index] == TextDecoderConsts.VarStartChar)
                {
                    if (lastTokenIndex != index)
                        result.Add(new RawTextFragment(value.Substring(lastTokenIndex,
                            index - lastTokenIndex)));

                    var content = ReadToken(value, TextDecoderConsts.VarEndChar, ref index);
                    result.Add(ParseVariableFragment(content));
                }
                else if (value[index] == TextDecoderConsts.SelectionStartChar)
                {
                    if (index > 1 && value[index - 1] == TextDecoderConsts.SelectionModifierStartChar)
                    {
                        index--;
                        if (lastTokenIndex != index)
                            result.Add(new RawTextFragment(value.Substring(lastTokenIndex,
                                index - lastTokenIndex)));

                        index++;
                        var content = ReadToken(value, TextDecoderConsts.SelectionEndChar, ref index);
                        result.Add(ParseRandomSelectionFragment(content));
                    }
                    else
                    {
                        if (lastTokenIndex != index)
                            result.Add(new RawTextFragment(value.Substring(lastTokenIndex,
                                index - lastTokenIndex)));

                        var content = ReadToken(value, TextDecoderConsts.SelectionEndChar, ref index);
                        result.Add(ParseGenderSelectionFragment(content));
                    }
                }
                else
                {
                    continue;
                }

                lastTokenIndex = index;
            } while (++index < s.Length);

            if (lastTokenIndex != s.Length)
                result.Add(new RawTextFragment(value.Substring(lastTokenIndex, index - lastTokenIndex)));

            return result;
        }

        internal static TextFragment ParseVariableFragment(string content)
        {
            if (content.StartsWith(TextDecoderConsts.PlayerVariable, StringComparison.OrdinalIgnoreCase))
            {
                // Samples:
                // [Player1:f]
                // [Player1]
                // [Player]

                var playerReference = new PlayerReferenceFragment();

                var parameterBegin = content.IndexOf(TextDecoderConsts.VariableParametersStart);

                string playerTag;
                if (parameterBegin == -1)
                {
                    playerTag = content;
                }
                else
                {
                    playerTag = content.Substring(0, parameterBegin);

                    var gender = content.Substring(parameterBegin + 1, content.Length - 1 - parameterBegin);
                    if (ParserHelper.ParseRequiredGender(gender, out var requiredGender))
                        playerReference.RequiredGender = requiredGender;
                    else
                        throw new ArgumentException("Gender parameter of player tag could not be parsed: " +
                                                    content);
                }

                playerReference.PlayerIndex = ParsePlayerIndex(playerTag);
                return playerReference;
            }

            if (content.StartsWith(TextDecoderConsts.SipsVariable, StringComparison.OrdinalIgnoreCase))
            {
                var sipsFragment = new SipsFragment();

                var parameterBegin = content.IndexOf(TextDecoderConsts.VariableParametersStart);

                string sipsTag;
                if (parameterBegin == -1)
                {
                    sipsTag = content;
                }
                else
                {
                    sipsTag = content.Substring(0, parameterBegin);
                    sipsFragment.MinSips = int.Parse(content.Substring(parameterBegin + 1));
                }

                if (sipsTag.Length > TextDecoderConsts.SipsVariable.Length)
                    sipsFragment.SipsIndex = int.Parse(sipsTag.Substring(TextDecoderConsts.SipsVariable.Length));

                return sipsFragment;
            }

            return new RawTextFragment(content);
        }

        internal static TextFragment ParseRandomSelectionFragment(string content)
        {
            // Samples:
            // 12,54,56-90
            // 12,hello,not,19

            var isNumericSelection = true;
            for (var i = 0; i < content.Length; i++)
                if (char.IsLetter(content[i]) || content[i] == '"')
                {
                    isNumericSelection = false;
                    break;
                }

            if (!isNumericSelection)
                return new RandomTextFragment {Texts = SplitQuoted(content, TextDecoderConsts.ArrayDelimiter)};

            var numbers = ParseNumberArray(content);
            return new RandomNumberFragment {Numbers = numbers};
        }

        internal static IReadOnlyList<INumber> ParseNumberArray(string value)
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
                        ((NumberRange) currentNumber).Min = int.Parse(value.Substring(numberStart, i - numberStart));
                        numberStart = i + 1;
                        continue;
                    }

                    if (c == ',')
                        break;

                    throw new ArgumentException("Invalid character found: " + c);
                }

                if (i == numberStart)
                    throw new ArgumentException(
                        $"A number was expected at position {i} in string '{value}'");

                var num = int.Parse(value.Substring(numberStart, i - numberStart));
                if (isRangeToken)
                    ((NumberRange) currentNumber).Max = num;
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

        internal static TextFragment ParseGenderSelectionFragment(string content)
        {
            var fragment = new GenderBasedSelectionFragment();

            var splitterIndex = content.IndexOf(TextDecoderConsts.SelectionSplitterChar);
            var reference = content.IndexOf(TextDecoderConsts.SelectionReferenceChar);

            if (reference > -1)
            {
                fragment.ReferencedPlayerIndex = ParsePlayerIndex(content.Substring(reference + 1));
                content = content.Substring(0, reference);
            }

            if (splitterIndex == -1)
            {
                fragment.FemaleText = content;
            }
            else
            {
                if (splitterIndex != 0)
                    fragment.FemaleText = content.Substring(0, splitterIndex);
                fragment.MaleText = content.Substring(splitterIndex + 1, content.Length - splitterIndex - 1);
            }

            return fragment;
        }

        private static int ParsePlayerIndex(string value)
        {
            if (value.Length > TextDecoderConsts.PlayerVariable.Length)
                return int.Parse(value.Substring(TextDecoderConsts.PlayerVariable.Length));

            return 1;
        }

        private static string ReadToken(string value, char endChar, ref int i)
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

            return value.Substring(tokenStartPos + 1, i - tokenStartPos - 2);
        }

        internal static IReadOnlyList<string> SplitQuoted(string value, char delimiter)
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

                        result.Add(value.Substring(tokenStart, i - tokenStart));
                        tokenStart = i + 1;
                        break;
                    }

                    if (value[i] == '"')
                    {
                        if (!withinQuotes)
                            continue; //allow quotes in the middle

                        if (i == value.Length - 1) //if its the last char
                        {
                            result.Add(value.Substring(tokenStart, i - tokenStart).Replace("\"\"", "\""));
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

                        result.Add(value.Substring(tokenStart, i - tokenStart).Replace("\"\"", "\""));
                        tokenStart = i + 2;
                        break;
                    }

                    if (i == value.Length - 1)
                    {
                        if (withinQuotes)
                            throw new ArgumentException("The text must end with a quote");

                        result.Add(value.Substring(tokenStart, i - tokenStart + 1));
                        return result;
                    }
                } while (++i < value.Length);
            }

            return result;
        }
    }
}
#endif
