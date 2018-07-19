using Drinctet.Core.Parsing.Utilities;
using Xunit;

namespace Drinctet.Core.Tests.Parsing.Utilities
{
    public class ParserHelperTests
    {
        [Theory]
        [InlineData("Player", 0)]
        [InlineData("Player1", 1)]
        [InlineData("Player100", 100)]
        [InlineData("Player84", 84)]
        public void TestParseValidPlayerTag(string s, int index)
        {
            Assert.True(ParserHelper.ParsePlayerTag(s, out var resultIndex));
            Assert.Equal(index, resultIndex);
        }
    }
}