using Drinctet.Harvester.Picolo;
using Xunit;

namespace Drinctet.Harvester.Tests.Picolo
{
    public class PicoloTests
    {
        [Theory]
        [InlineData("Hello World!", "Hello World!")]
        [InlineData("%s, du hast 5 Minuten", "[Player], du hast 5 Minuten")]
        [InlineData("%s, trink $ Schlucke", "[Player], trink [sips]")]
        [InlineData("%s, wenn du mehr als $ Gläser getrunken hast, darfst du $ Schlucke verteilen", "[Player], wenn du mehr als $ Gläser getrunken hast, darfst du [sips] verteilen")]
        [InlineData("%s, gib $ Schlucke an %s oder $ Schlucke an %s ab",
            "[Player1], gib [sips1] an [Player2] oder [sips2] an [Player3] ab")]
        public void TestTransformText(string source, string expectedResult)
        {
            var result = PicoloHarvester.TransformText(source, 0);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void TestReplaceWithArray()
        {
            string[] values = {"one", "two", "three"};
            var text = "Hello $, is that really $ or $?";
            var result = PicoloHarvester.ReplaceWithArray(text, "$", values);
            Assert.Equal("Hello one, is that really two or three?", result);
        }
    }
}