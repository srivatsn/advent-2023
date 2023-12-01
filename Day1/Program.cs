var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);

var words = new Dictionary<string, int>(){
    {"one", 1},
    {"two", 2},
    {"three", 3},
    {"four", 4},
    {"five", 5},
    {"six", 6},
    {"seven", 7},
    {"eight", 8},
    {"nine", 9},
    {"0", 0},
    {"1", 1},
    {"2", 2},
    {"3", 3},
    {"4", 4},
    {"5", 5},
    {"6", 6},
    {"7", 7},
    {"8", 8},
    {"9", 9}
};


int FindFirstNumber(string line)
{
for (int i = 0; i < line.Length; i++)
{
var subStr = line.Substring(i);
var first = words.Select(w => subStr.StartsWith(w.Key, StringComparison.OrdinalIgnoreCase) ? w.Value : -1)
    .SingleOrDefault(w => w > 0);
if (first > 0)
return first;
}
return -1;
}

int FindLastNumber(string line)
{
    for (int i = line.Length - 1; i >= 0; i--)
    {
        var subStr = line.Substring(i);
        var first = words.Select(w => subStr.StartsWith(w.Key, StringComparison.OrdinalIgnoreCase) ? w.Value : -1)
            .SingleOrDefault(w => w > 0);
        if (first > 0)
            return first;
    }
    return -1;
}

int sum = 0;
foreach (var line in lines)
{
    var valueString = FindFirstNumber(line).ToString() + FindLastNumber(line).ToString();
    if (!int.TryParse(valueString, out int value))
    {
        Console.WriteLine($"Failed to parse value from line: {line}");
        return;
    }
    Console.WriteLine($"Value: {value}");
    sum += value;
}

Console.WriteLine($"Sum of all values: {sum}");

