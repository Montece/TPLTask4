namespace TPLTask4;

internal static class StringUtility
{
    internal static IEnumerable<string> SplitToSubstrings(string input, int substringLength)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentNullException(nameof(input));
        }

        if (substringLength <= 0)
        {
            throw new ArgumentException("substringLength must be greater than zero", nameof(substringLength));
        }

        if (input.Length <= substringLength)
        {
            yield return input;
        }
        else
        {
            for (var i = 0; i < input.Length; i += substringLength)
            {
                var length = Math.Min(substringLength, input.Length - i);
                var substring = input.Substring(i, length);

                yield return substring;
            }
        }
    }
}