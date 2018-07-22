using System;
using System.Collections.Generic;
using System.Linq;
using Drinctet.Core.Cards.Base;
using Drinctet.Core.Selection;
using Xunit;

namespace Drinctet.Core.Tests.Selection
{
    public class BenokolaAlgorithmTests
    {
        private BenokolaAlgorithm _algorithm;

        public BenokolaAlgorithmTests()
        {
            _algorithm = new BenokolaAlgorithm();
        }

        public static readonly TheoryData<RequiredGender[], PlayerInfo[], int[]> SelectPlayerData =
            new TheoryData<RequiredGender[], PlayerInfo[], int[]>
            {
                {
                    new[] {RequiredGender.Male},
                    new[] {new PlayerInfo(1, Gender.Female), new PlayerInfo(2, Gender.Male)},
                    new[] {2}
                },
                {
                    new[] {RequiredGender.Female, RequiredGender.Opposite},
                    new[] {new PlayerInfo(1, Gender.Female), new PlayerInfo(2, Gender.Male)},
                    new[] {1, 2}
                }
            };

        [Theory]
        [MemberData(nameof(SelectPlayerData))]
        public void TestSelectPlayers(RequiredGender[] requiredGenders, PlayerInfo[] players, int[] expectedPlayers)
        {
            _algorithm.Initialize(
                new DrinctetStatus
                {
                    Players = players,
                    PlayerScores = players.ToDictionary(x => x.Id, x => 10),
                    PlayerArrangements = new Dictionary<int, int>()
                }, new Random());

            var result = _algorithm.SelectPlayers(requiredGenders, new CardTag[0]);
            Assert.Equal(expectedPlayers.Length, result.Count);

            for (int i = 0; i < expectedPlayers.Length; i++)
            {
                var expectedId = expectedPlayers[i];
                Assert.Equal(expectedId, result[i].Id);
            }
        }
    }
}