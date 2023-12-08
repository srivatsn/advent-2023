using System.Text.RegularExpressions;

var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);

bool CheckLine(int lineNumber, int startIndex, int endIndex)
{
    var line = lines[lineNumber];
    var lineStart = startIndex > 0 ? startIndex - 1 : startIndex;
    var lineEnd = endIndex < line.Length - 1 ? endIndex + 1 : endIndex;
    return line.Substring(lineStart, lineEnd - lineStart + 1).Any(c => !(char.IsDigit(c) || c == '.'));
}

bool IsNextToASymbol(int startIndex, int endIndex, int lineNumber)
{
    // Check the line above, below and the current line
    if ((lineNumber > 0 && CheckLine(lineNumber - 1, startIndex, endIndex)) ||
        (lineNumber < lines.Length - 1 && CheckLine(lineNumber + 1, startIndex, endIndex)) ||
        CheckLine(lineNumber, startIndex, endIndex))
    {
        return true;
    }

    return false;
}

// Part 1
int sum = 0;
for (int i = 0; i < lines.Length; i++)
{
    // Find all the numbers and their positions in this line
    var matches = Regex.Matches(lines[i], @"\d+");

    foreach (Match match in matches)
    {
        var number = int.Parse(match.ToString());
        var startIndex = match.Index;
        var endIndex = startIndex + match.Length - 1;
        if (IsNextToASymbol(startIndex, endIndex, i))
        {
            sum += number;
        }
    }
}

Console.WriteLine(sum);

List<int> GetNumbers(int lineNumber, int startIndex, int endIndex)
{
    if (lineNumber < 0 || lineNumber >= lines.Length)
    {
        return [];
    }

    var line = lines[lineNumber];

    while (startIndex > 0 && char.IsDigit(line[startIndex]))
    {
        startIndex--;
    }

    while (endIndex < line.Length - 1 && char.IsDigit(line[endIndex]))
    {
        endIndex++;
    }

    var substring = line.Substring(startIndex, endIndex - startIndex + 1);
    return Regex.Matches(substring, @"\d+").Select(m => int.Parse(m.ToString())).ToList();
}

// Part 2
var gearRatioSum = 0;
for (int i = 0; i < lines.Length; i++)
{
    int startIndex = 0;
    int starIndex = 0;
    while (starIndex != -1)
    {
        starIndex = lines[i].IndexOf('*', startIndex);
        if (starIndex == -1)
        {
            break;
        }

        var numbers = new List<int>();
        numbers.AddRange(GetNumbers(i - 1, starIndex - 1, starIndex + 1));
        numbers.AddRange(GetNumbers(i, starIndex - 1, starIndex + 1));
        numbers.AddRange(GetNumbers(i + 1, starIndex - 1, starIndex + 1));

        if (numbers.Count == 2)
        {
            var gearRatio = numbers[0] * numbers[1];
            gearRatioSum += gearRatio;
        }
        startIndex = starIndex + 1;
    }
}

Console.WriteLine($"Gear Ratio sum is {gearRatioSum}");