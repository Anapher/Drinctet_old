using System;
using System.Collections.Generic;
using System.Linq;
using Drinctet.Core.Cards.Base;
using Drinctet.Core.Fragments;
using Drinctet.Core.Parsing.TextDecoder;
using Xunit;

namespace Drinctet.Core.Tests.Parsing.TextDecoder
{
    public class DefaultTextDecoderTests
    {
        public static readonly TheoryData<string, string[]> QuotedData = new TheoryData<string, string[]>
        {
            {"hello,world,test", new[] {"hello", "world", "test"}},
            {"hello,wtf is that,test", new[] {"hello", "wtf is that", "test"}},
            {"hello,\"oranges, pinapple\",test", new[] {"hello", "oranges, pinapple", "test"}},
            {"hello,\"oranges,\"\" pinapple\",test", new[] {"hello", "oranges,\" pinapple", "test"}},
            {"hello,\"oranges,\"\" pinapple\"", new[] {"hello", "oranges,\" pinapple"}},
            {"hello,\"oranges, pinapple\"", new[] {"hello", "oranges, pinapple"}},
            {"hello,\"oranges,\"\", pinapple\"", new[] {"hello", "oranges,\", pinapple"}},
            {"\"wtf\"\"\"", new[] {"wtf\""}},
            {"\"oranges,\"\", pinapple\"", new[] {"oranges,\", pinapple"}},
            {
                "hello \"quoted\",wtf is\" that,\"wtf, is, that\"",
                new[] {"hello \"quoted\"", "wtf is\" that", "wtf, is, that"}
            }
        };

        [Theory]
        [MemberData(nameof(QuotedData))]
        public void TestSplitQuoted(string source, string[] expectedResult)
        {
            var result = DefaultTextDecoder.SplitQuoted(source.AsSpan(), ',').ToArray();

            Assert.Equal(expectedResult.Length, result.Length);
            for (var i = 0; i < expectedResult.Length; i++) Assert.Equal(expectedResult[i], result[i]);
        }

        [Theory]
        [InlineData("\"hello,world")]
        [InlineData("\"hello\"world,test")]
        [InlineData("\"hello\"\",test")]
        public void TestSplitQuotedWithInvalidData(string source)
        {
            Assert.ThrowsAny<ArgumentException>(() => DefaultTextDecoder.SplitQuoted(source, ','));
        }

        public static readonly TheoryData<string, INumber[]> TestNumberData = new TheoryData<string, INumber[]>
        {
            {"5", new INumber[] {new StaticNumber(5)}},
            {"1-10", new INumber[] {new NumberRange(1, 10)}},
            {"5,1-10", new INumber[] {new StaticNumber(5), new NumberRange(1, 10)}},
            {
                "1,3-5,7,8-9,11",
                new INumber[]
                {
                    new StaticNumber(1), new NumberRange(3, 5), new StaticNumber(7), new NumberRange(8, 9),
                    new StaticNumber(11)
                }
            }
        };

        [Theory]
        [MemberData(nameof(TestNumberData))]
        public void TestParseNumberArray(string source, INumber[] numbers)
        {
            var result = DefaultTextDecoder.ParseNumberArray(source);
            CompareNumberArrays(numbers, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(",")]
        [InlineData("5-")]
        [InlineData("1,5-,56")]
        [InlineData("1, 5-7,56")]
        [InlineData("1,$5-7,56")]
        public void TestParseNumberArrayWithInvalidData(string source)
        {
            Assert.ThrowsAny<ArgumentException>(() => DefaultTextDecoder.ParseNumberArray(source));
        }

        public static readonly TheoryData<string, GenderBasedSelectionFragment> GenderBasedSelectionFragmentData =
            new TheoryData<string, GenderBasedSelectionFragment>
            {
                {"{she}", new GenderBasedSelectionFragment{FemaleText = "she"}},
                {"{she|5}", new GenderBasedSelectionFragment{FemaleText = "she", ReferencedPlayerIndex = 5}},
                {"{she/he}", new GenderBasedSelectionFragment{FemaleText = "she", MaleText = "he"}},
                {"{she/he|5}", new GenderBasedSelectionFragment{FemaleText = "she", MaleText = "he", ReferencedPlayerIndex = 5}},
                {"{/he|5}", new GenderBasedSelectionFragment{MaleText = "he", ReferencedPlayerIndex = 5}}
            };

        [Theory]
        [MemberData(nameof(GenderBasedSelectionFragmentData))]
        public void TestParseGenderSelectionFragment(string source, GenderBasedSelectionFragment expected)
        {
            var result = DefaultTextDecoder.ParseGenderSelectionFragment(source.AsSpan(1, source.Length - 2));
            CompareFragments(expected, result);
        }

        public static readonly TheoryData<string, TextFragment> ParseRandomSelectionFragmentData =
            new TheoryData<string, TextFragment>
            {
                {"hello,world", new RandomTextFragment {Texts = new[] {"hello", "world"}}},
                {"12,world", new RandomTextFragment {Texts = new[] {"12", "world"}}},
                {"12,50", new RandomNumberFragment{Numbers = new INumber[]{new StaticNumber(12), new StaticNumber(50)  }}},
                {"12,50-60", new RandomNumberFragment{Numbers = new INumber[]{new StaticNumber(12), new NumberRange(50, 60),   }}},
                {"\"12\"", new RandomTextFragment {Texts = new[] {"12"}}},
                {"hello,\" world, hey \",this \" is,\"great!\"\"\"", new RandomTextFragment {Texts = new[] {"hello", " world, hey ", "this \" is", "great!\""}}},
            };

        [Theory]
        [MemberData(nameof(ParseRandomSelectionFragmentData))]
        public void TestParseRandomSelectionFragment(string source, TextFragment expected)
        {
            var result = DefaultTextDecoder.ParseRandomSelectionFragment(source);
            CompareFragments(expected, result);
        }

        public static readonly TheoryData<string, TextFragment> ParseVariableFragmentData =
            new TheoryData<string, TextFragment>
            {
                {"Player", new PlayerReferenceFragment {PlayerIndex = 1, RequiredGender = RequiredGender.None}},
                {"Player1", new PlayerReferenceFragment {PlayerIndex = 1, RequiredGender = RequiredGender.None}},
                {"Player2", new PlayerReferenceFragment {PlayerIndex = 2, RequiredGender = RequiredGender.None}},
                {"Player2:f", new PlayerReferenceFragment {PlayerIndex = 2, RequiredGender = RequiredGender.Female}},
                {"Player:f", new PlayerReferenceFragment {PlayerIndex = 1, RequiredGender = RequiredGender.Female}},
                {"sips", new SipsFragment {SipsIndex = 1, MinSips = 1}},
                {"sips1", new SipsFragment {SipsIndex = 1, MinSips = 1}},
                {"sips2", new SipsFragment {SipsIndex = 2, MinSips = 1}},
                {"sips2:5", new SipsFragment {SipsIndex = 2, MinSips = 5}},
                {"sips:5", new SipsFragment {SipsIndex = 1, MinSips = 5}},
            };

        [Theory]
        [MemberData(nameof(ParseVariableFragmentData))]
        public void TestParseVariableFragmentData(string source, TextFragment expected)
        {
            var result = DefaultTextDecoder.ParseVariableFragment(source);
            CompareFragments(expected, result);
        }

        public static readonly TheoryData<string, TextFragment[]> DecodeData = new TheoryData<string, TextFragment[]>
        {
            {"Hello World!", new TextFragment[] {new RawTextFragment("Hello World!")}},
            {
                "Schwarz. Alle, die weiß gewählt haben, trinken [sips]",
                new TextFragment[]
                    {new RawTextFragment("Schwarz. Alle, die weiß gewählt haben, trinken "), new SipsFragment()}
            },
            {
                "[Player1], kiss [Player2:o]. Drink [sips:6] if you don't want that.",
                new TextFragment[]
                {
                    new PlayerReferenceFragment {PlayerIndex = 1}, new RawTextFragment(", kiss "),
                    new PlayerReferenceFragment {PlayerIndex = 2, RequiredGender = RequiredGender.Opposite},
                    new RawTextFragment(". Drink "), new SipsFragment {MinSips = 6},
                    new RawTextFragment(" if you don't want that."),
                }
            },
            {
                "Trinke einen Shot aus [Player]s Bauchnabel.",
                new TextFragment[]
                {
                    new RawTextFragment("Trinke einen Shot aus "), new PlayerReferenceFragment(),
                    new RawTextFragment("s Bauchnabel."),
                }
            },
            {
                "[Player1], wenn du [Player2] ein bisschen kennst, dann gib {ihr/ihm} einen Kuss oder trink [sips:8]",
                new TextFragment[]
                {
                    new PlayerReferenceFragment(), new RawTextFragment(", wenn du "),
                    new PlayerReferenceFragment {PlayerIndex = 2},
                    new RawTextFragment(" ein bisschen kennst, dann gib "),
                    new GenderBasedSelectionFragment {FemaleText = "ihr", MaleText = "ihm"},
                    new RawTextFragment(" einen Kuss oder trink "), new SipsFragment {MinSips = 8},
                }
            },
            {
                "Alle Spieler, in deren Vorname ein '!{e,i,a,u}' vorkommt, müssen [sips] trinken",
                new TextFragment[]
                {
                    new RawTextFragment("Alle Spieler, in deren Vorname ein '"),
                    new RandomTextFragment {Texts = new[] {"e", "i", "a", "u"}},
                    new RawTextFragment("' vorkommt, müssen "), new SipsFragment(), new RawTextFragment(" trinken"),
                }
            }
        };

        [Theory]
        [MemberData(nameof(DecodeData))]
        public void TestDecode(string source, TextFragment[] expectedFragments)
        {
            var decoder = new DefaultTextDecoder();
            var result = decoder.Decode(source);
            Assert.Equal(expectedFragments.Length, result.Count);

            for (int i = 0; i < expectedFragments.Length; i++)
                CompareFragments(expectedFragments[i], result[i]);
        }

        private void CompareFragments(TextFragment expectedFragment, TextFragment actualFragment)
        {
            bool CheckForType<T>(Action<T, T> checkAction) where T : TextFragment
            {
                if (expectedFragment is T expected)
                {
                    var actual = Assert.IsType<T>(actualFragment);
                    checkAction(expected, actual);
                    return true;
                }

                return false;
            }

            var typeHandlers = new List<Func<bool>>();
            typeHandlers.Add(() => CheckForType<PlayerReferenceFragment>((expected, actual) =>
            {
                Assert.Equal(expected.PlayerIndex, actual.PlayerIndex);
                Assert.Equal(expected.RequiredGender, actual.RequiredGender);
            }));
            typeHandlers.Add(() => CheckForType<SipsFragment>((expected, actual) =>
            {
                Assert.Equal(expected.MinSips, actual.MinSips);
                Assert.Equal(expected.SipsIndex, actual.SipsIndex);
            }));
            typeHandlers.Add(() => CheckForType<RandomTextFragment>((expected, actual) =>
            {
                Assert.Equal(expected.Texts.Count, actual.Texts.Count);
                for (var i = 0; i < expected.Texts.Count; i++)
                {
                    var expectedText = expected.Texts[i];
                    Assert.Equal(actual.Texts[i], expectedText);
                }
            }));
            typeHandlers.Add(() => CheckForType<RandomNumberFragment>((expected, actual) =>
            {
                CompareNumberArrays(expected.Numbers, actual.Numbers);
            }));
            typeHandlers.Add(() => CheckForType<RawTextFragment>((expected, actual) =>
            {
                Assert.Equal(expected.Text, actual.Text);
            }));
            typeHandlers.Add(() => CheckForType<RawTextFragment>((expected, actual) =>
            {
                Assert.Equal(expected.Text, actual.Text);
            }));
            typeHandlers.Add(() => CheckForType<GenderBasedSelectionFragment>((expected, actual) =>
            {
                Assert.Equal(expected.MaleText, actual.MaleText);
                Assert.Equal(expected.FemaleText, actual.FemaleText);
                Assert.Equal(expected.ReferencedPlayerIndex, actual.ReferencedPlayerIndex);
            }));

            foreach (var typeHandler in typeHandlers)
            {
                if (typeHandler())
                    return;
            }

            throw new ArgumentException($"Invalid fragment type: {actualFragment.GetType().Name}");
        }

        private static void CompareNumberArrays(IReadOnlyList<INumber> expected, IReadOnlyList<INumber> actual)
        {
            Assert.Equal(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                var number = expected[i];
                var actualNumber = actual[i];

                if (number is StaticNumber staticNumber)
                {
                    var actualStaticNumber = Assert.IsType<StaticNumber>(actualNumber);
                    Assert.Equal(staticNumber.Number, actualStaticNumber.Number);
                }
                else if (number is NumberRange numberRange)
                {
                    var actualNumberRange = Assert.IsType<NumberRange>(actualNumber);
                    Assert.Equal(numberRange.Min, actualNumberRange.Min);
                    Assert.Equal(numberRange.Max, actualNumberRange.Max);
                }
            }
        }
    }
}