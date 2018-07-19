using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Drinctet.Core.Cards;
using Drinctet.Core.Cards.Base;
using Drinctet.Core.Parsing;
using Xunit;

namespace Drinctet.Core.Tests.Parsing
{
    public class BaseCardParserPlayerTests : BaseCardParser<WyrCard>
    {
        public static readonly TheoryData<string, IReadOnlyList<PlayerSettings>> TestData =
            new TheoryData<string, IReadOnlyList<PlayerSettings>>
            {
                {"<Player />", new List<PlayerSettings> {new PlayerSettings {Gender = RequiredGender.None}}},
                {"<Player gender=\"m\"/>", new List<PlayerSettings> {new PlayerSettings {Gender = RequiredGender.Male}}},
                {"<Player gender=\"f\"/>", new List<PlayerSettings> {new PlayerSettings {Gender = RequiredGender.Female}}},
                {"<Player1 gender=\"f\"/>", new List<PlayerSettings> {new PlayerSettings {Gender = RequiredGender.Female, PlayerIndex = 1}}},
                {"<Player56 gender=\"o\"/>", new List<PlayerSettings> {new PlayerSettings {Gender = RequiredGender.Opposite, PlayerIndex = 56}}},
                {"<Player56 gender=\"s\"/>", new List<PlayerSettings> {new PlayerSettings {Gender = RequiredGender.Same, PlayerIndex = 56}}},
                {"<Player100 gender=\"male\"/>", new List<PlayerSettings> {new PlayerSettings {Gender = RequiredGender.Male, PlayerIndex = 100}}},
                {"<Player102 gender=\"female\"/>", new List<PlayerSettings> {new PlayerSettings {Gender = RequiredGender.Female, PlayerIndex = 102}}},
                {"<Player1 gender=\"m\"/> <Player2 gender=\"f\"/>", new List<PlayerSettings> {new PlayerSettings {Gender = RequiredGender.Male, PlayerIndex = 1}, new PlayerSettings {Gender = RequiredGender.Female, PlayerIndex = 2}}},
            };

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestParsePlayers(string s, IReadOnlyList<PlayerSettings> expected)
        {
            var result = ParsePlayers(XmlUtils.CreateReader(s)).ToList();

            Assert.Equal(expected.Count, result.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                var expectedPlayer = expected[i];
                var actual = result[i];

                Assert.Equal(expectedPlayer.PlayerIndex, actual.PlayerIndex);
                Assert.Equal(expectedPlayer.Gender, actual.Gender);
            }
        }

        protected override bool ParseElement(XmlReader xmlReader, WyrCard card) => throw new NotImplementedException();
        protected override void ParseAttributes(WyrCard card) => throw new NotImplementedException();
    }
}