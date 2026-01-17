using Xunit;

namespace TPLTask4.Tests;

public class StringUtilityTests
{
    [Fact]
    public void SplitToSubstrings_ShouldSplitCorrectly()
    {
        const string input = "HelloWorld!";
        const int substringLength = 3;

        var result = StringUtility.SplitToSubstrings(input, substringLength);

        Assert.Equal(["Hel", "loW", "orl", "d!"], result);
    }

    [Fact]
    public void SplitToSubstrings_StringShorterThanChunk_ReturnsWholeString()
    {
        const string input = "Hi";
        const int substringLength = 5;

        var result = StringUtility.SplitToSubstrings(input, substringLength);

        Assert.Single(result);
        Assert.Equal("Hi", result.First());
    }

    [Fact]
    public void SplitToSubstrings_StringEqualToChunk_ReturnsWholeString()
    {
        const string input = "Hello";
        const int substringLength = 5;

        var result = StringUtility.SplitToSubstrings(input, substringLength);

        Assert.Single(result);
        Assert.Equal("Hello", result.First());
    }

    [Fact]
    public void SplitToSubstrings_LastChunkShorter_ShouldReturnCorrectly()
    {
        const string input = "ABCDEFG";
        const int substringLength = 3;

        var result = StringUtility.SplitToSubstrings(input, substringLength);

        Assert.Equal(["ABC", "DEF", "G"], result);
    }

    [Fact]
    public void SplitToSubstrings_EmptyString_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => StringUtility.SplitToSubstrings(string.Empty, 3).ToList());
    }

    [Fact]
    public void SplitToSubstrings_NullString_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => StringUtility.SplitToSubstrings(null!, 3).ToList());
    }
}