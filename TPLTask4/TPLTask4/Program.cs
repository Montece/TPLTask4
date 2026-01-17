using TPLTask4;

using var catalog = new Catalog(Console.WriteLine);

var isWorking = true;
var isWorkingLock = new object();

Console.CancelKeyPress += (_, _) =>
{
    lock (isWorkingLock)
    {
        isWorking = false;
    }
};

catalog.BeginSortProcess();

while (true)
{
    lock (isWorkingLock)
    {
        if (!isWorking)
        {
            break;
        }
    }

    Console.Write("Enter text: ");
    var rawInput = Console.ReadLine();

    if (rawInput is null)
    {
        break;
    }

    if (string.IsNullOrEmpty(rawInput))
    {
        catalog.Show();
    }
    else
    if (rawInput == "1")
    {
        catalog.Sort();
    }
    else
    {
        const int MAX_SUBSTRING_LENGTH = 80;
        var inputs = StringUtility.SplitToSubstrings(rawInput, MAX_SUBSTRING_LENGTH);

        foreach (var input in inputs)
        {
            catalog.Add(input);
        }
    }
}

catalog.EndSortProcess();