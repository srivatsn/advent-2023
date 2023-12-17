
var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n").ToList();

static List<char[][]> ParsePatterns(List<string> lines)
{
    var patterns = new List<char[][]>();
    var patternLines = new List<char[]>();
    foreach (var line in lines)
    {
        if (string.IsNullOrEmpty(line))
        {
            patterns.Add([.. patternLines]);
            patternLines.Clear();
            continue;
        }
        patternLines.Add([.. line]);
    }
    patterns.Add([.. patternLines]);

    return patterns;
}

int FindVerticalMirror(char[][] pattern, int skipIndex = -1)
{
    for (int i = 1; i < pattern[0].Length; i++)
    {
        if (i == skipIndex)
        {
            continue;
        }

        int j;
        for (j = 0; j < pattern.Length; j++)
        {
            var maxLen = Math.Min(i, pattern[j].Length - i);
            var lhs = pattern[j][(i - maxLen)..i];
            var rhs = pattern[j][i..(i + maxLen)];

            if (!lhs.SequenceEqual(rhs.Reverse().ToArray()))
            {
                break;
            }
        }
        if (j == pattern.Length)
        {
            return i;
        }
    }

    return 0;
}

int FindHorizontalMirror(char[][] pattern, int skipIndex = -1)
{
    for (int i = 1; i < pattern.Length; i++)
    {
        if (i == skipIndex)
        {
            continue;
        }

        int j;
        for (j = 0; j < pattern[0].Length; j++)
        {
            var maxLen = Math.Min(i, pattern.Length - i);
            var lhs = pattern.Select(x => x[j]).ToArray()[(i - maxLen)..i];
            var rhs = pattern.Select(x => x[j]).ToArray()[i..(i + maxLen)];

            if (!lhs.SequenceEqual(rhs.Reverse().ToArray()))
            {
                break;
            }
        }
        if (j == pattern[0].Length)
        {
            return i;
        }
    }

    return 0;
}

var patterns = ParsePatterns(lines);

var summary = 0;
var scores = new List<int>();
foreach (var pattern in patterns)
{
    var score = FindVerticalMirror(pattern);
    if (score == 0)
    {
        score = 100 * FindHorizontalMirror(pattern);
    }
    scores.Add(score);
    summary += score;
}
Console.WriteLine($"Summary: {summary}");

// Part2
summary = 0;
for (int p = 0; p < patterns.Count; p++)
{
    int score = 0;
    var pattern = patterns[p];
    for (int i = 0; i < pattern.Length; i++)
    {
        for (int j = 0; j < pattern[i].Length; j++)
        {
            score = 0;
            var oldPattern = pattern[i][j];
            pattern[i][j] = pattern[i][j] == '#' ? '.' : '#';

            score = FindVerticalMirror(pattern, scores[p]);

            if (score == 0)
            {
                score = 100 * FindHorizontalMirror(pattern, scores[p] / 100);
            }

            if (score != 0)
            {
                //Console.WriteLine($"Smudge at {j},{i} with score {score}");
                break;
            }

            pattern[i][j] = oldPattern;
        }

        if (score != 0)
        {
            break;
        }
    }

    summary += score;
}
Console.WriteLine($"Summary: {summary}");
