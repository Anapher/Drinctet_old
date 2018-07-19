using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Drinctet.Core.Cards.Base;
using Drinctet.Core.Parsing;
using Xunit;

namespace Drinctet.Core.Tests.Parsing
{
    public class TextContentParserTests
    {
        public static TheoryData<string, List<TextElement>> TestData = new TheoryData<string, List<TextElement>>
        {
            {
                "<Text lang=\"de\">Hallo Welt!</Text>",
                new List<TextElement>
                {
                    new TextElement
                    {
                        Weight = 1,
                        Translations =
                            new Dictionary<CultureInfo, string> {{CultureInfo.GetCultureInfo("de"), "Hallo Welt!"}}
                    }
                }
            },
            {
                "<Text lang=\"de\">Hallo Welt!</Text>" + "\r\n<Text lang=\"en\">Hello World!</Text>",
                new List<TextElement>
                {
                    new TextElement
                    {
                        Weight = 1,
                        Translations = new Dictionary<CultureInfo, string>
                        {
                            {CultureInfo.GetCultureInfo("de"), "Hallo Welt!"},
                            {CultureInfo.GetCultureInfo("en"), "Hello World!"}
                        }
                    }
                }
            },
            {
                @"  <Case weight=""60"">
                        <Text lang=""de"">Schwarz. Alle, die weiß gewählt haben, trinken [sips]</Text>
                        <Text lang=""en"">Black. Everybody who said white drinks [sips]</Text>
                    </Case>
                    <Case weight=""40"">
                        <Text lang=""de"">Weiß. Alle, die schwarz gewählt haben, trinken [sips]</Text>
                    </Case>",
                new List<TextElement>
                {
                    new TextElement
                    {
                        Weight = 60,
                        Translations = new Dictionary<CultureInfo, string>
                        {
                            {CultureInfo.GetCultureInfo("de"), "Schwarz. Alle, die weiß gewählt haben, trinken [sips]"},
                            {CultureInfo.GetCultureInfo("en"), "Black. Everybody who said white drinks [sips]"}
                        }
                    },
                    new TextElement
                    {
                        Weight = 40,
                        Translations = new Dictionary<CultureInfo, string>
                        {
                            {CultureInfo.GetCultureInfo("de"), "Weiß. Alle, die schwarz gewählt haben, trinken [sips]"}
                        }
                    }
                }
            },
            {
                @"  <Case weight=""60"">
                        <Text lang=""de"">Schwarz. Alle, die weiß gewählt haben, trinken [sips]</Text>
                        <Text lang=""en"">Black. Everybody who said white drinks [sips]</Text>
                    </Case>",
                new List<TextElement>
                {
                    new TextElement
                    {
                        Weight = 60,
                        Translations = new Dictionary<CultureInfo, string>
                        {
                            {CultureInfo.GetCultureInfo("de"), "Schwarz. Alle, die weiß gewählt haben, trinken [sips]"},
                            {CultureInfo.GetCultureInfo("en"), "Black. Everybody who said white drinks [sips]"}
                        }
                    }
                }
            }
        };

        public static TheoryData<string, List<TextElement>> InvalidDataTest =
            new TheoryData<string, List<TextElement>>
            {
                {"<Text>Hello!</Text>", null},
                {
                    @"  <Case weight=""40"">
                            <Text lang=""de"">Weiß. Alle, die schwarz gewählt haben, trinken [sips]</Text>
                        </Case>
                        <Text lang=""de"">Iss eine Banane mit Schale.</Text>",
                    new List<TextElement>
                    {
                        new TextElement
                        {
                            Weight = 40,
                            Translations = new Dictionary<CultureInfo, string>
                            {
                                {
                                    CultureInfo.GetCultureInfo("de"),
                                    "Weiß. Alle, die schwarz gewählt haben, trinken [sips]"
                                }
                            }
                        }
                    }
                },
            };

        [Theory]
        [MemberData(nameof(InvalidDataTest))]
        public void TestInvalidXml(string s, List<TextElement> expectedResult)
        {
            var reader = XmlUtils.CreateReader(s);
            var parser = new TextContentParser();

            if (expectedResult == null)
            {
                Assert.Throws<ArgumentException>(() =>
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                            parser.AddElement(reader);
                    }
                });
            }
            else
            {
                var anyWrong = false;

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                        if (!parser.AddElement(reader))
                            anyWrong = true;
                }

                Assert.True(anyWrong);

                CompareResult(parser.Result, expectedResult);
            }
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestParseXml(string s, List<TextElement> expectedResult)
        {
            var reader = XmlUtils.CreateReader(s);
            var parser = new TextContentParser();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                    Assert.True(parser.AddElement(reader));
            }

            CompareResult(parser.Result, expectedResult);
        }

        private void CompareResult(List<TextElement> result, List<TextElement> expectedResult)
        {
            Assert.Equal(expectedResult.Count, result.Count);
            for (int i = 0; i < expectedResult.Count; i++)
            {
                var expected = expectedResult[i];
                var actual = result[i];

                Assert.Equal(expected.Weight, actual.Weight);
                Assert.Equal(expected.Translations.Count, actual.Translations.Count);

                foreach (var (lang, text) in expected.Translations)
                {
                    Assert.True(actual.Translations.TryGetValue(lang, out var actualText));
                    Assert.Equal(text, actualText);
                }
            }
        }
    }
}