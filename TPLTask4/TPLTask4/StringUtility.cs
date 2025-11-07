namespace TPLTask4;

internal static class StringUtility
{
    public static List<string> SplitToSubstrings(string input, int substringLength)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentNullException(nameof(input));
        }

        if (input.Length <= substringLength)
        {
            return [ input ];
        }

        var substrings = new List<string>();

        for (var i = 0; i < input.Length; i += substringLength)
        {
            var length = Math.Min(substringLength, input.Length - i);
            var substring = input.Substring(i, length);
            substrings.Add(substring);
        }

        return substrings;
    }
}