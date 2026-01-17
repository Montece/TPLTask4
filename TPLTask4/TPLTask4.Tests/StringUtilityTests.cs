using Xunit;

namespace TPLTask4.Tests;

public class StringUtilityTests
{
    [Fact]
    public void SplitToSubstrings_ShouldSplitCorrectly()
    {
        const string INPUT = "HelloWorld!";
        const int SUBSTRING_LENGTH = 3;

        var result = StringUtility.SplitToSubstrings(INPUT, SUBSTRING_LENGTH);

        Assert.Equal(["Hel", "loW", "orl", "d!"], result);
    }

    [Fact]
    public void SplitToSubstrings_StringShorterThanChunk_ReturnsWholeString()
    {
        const string INPUT = "Hi";
        const int SUBSTRING_LENGTH = 5;

        var result = StringUtility.SplitToSubstrings(INPUT, SUBSTRING_LENGTH);

        var collection = result as string[] ?? result.ToArray();

        Assert.Single(collection);
        Assert.Equal("Hi", collection.First());
    }

    [Fact]
    public void SplitToSubstrings_StringEqualToChunk_ReturnsWholeString()
    {
        const string INPUT = "Hello";
        const int SUBSTRING_LENGTH = 5;

        var result = StringUtility.SplitToSubstrings(INPUT, SUBSTRING_LENGTH);

        var collection = result as string[] ?? result.ToArray();
        
        Assert.Single(collection);
        Assert.Equal("Hello", collection.First());
    }

    [Fact]
    public void SplitToSubstrings_LastChunkShorter_ShouldReturnCorrectly()
    {
        const string INPUT = "ABCDEFG";
        const int SUBSTRING_LENGTH = 3;

        var result = StringUtility.SplitToSubstrings(INPUT, SUBSTRING_LENGTH);

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