var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);

var map = lines.Select(l => l.ToCharArray()).ToArray();
bool[][] energized = new bool[map.Length][];
var nextSteps = new HashSet<(int x, int y, char direction)>();

int GetEnergy(int x, int y, char direction)
{
    // Clear energized and nextsteps.
    energized = energized.Select((r, i) => energized[i] = new bool[map![0].Length]).ToArray();
    nextSteps!.Clear();

    TraceBeam(x, y, direction);
    return energized.SelectMany(r => r).Count(e => e);
}

// Part 1
Console.WriteLine(GetEnergy(0, 0, 'E'));

// Part 2
int maxEnergy = 0;
for (int y = 0; y < map.Length; y++)
{
    for (int x = 0; x < map[0].Length; x++)
    {
        if (x != 0 && x != map[0].Length - 1 && y != 0 && y != map.Length - 1)
        {
            continue;
        }

        char direction = (x, y) switch
        {
            (0, _) => 'E',
            (_, 0) => 'S',
            _ when x == map[0].Length - 1 => 'W',
            _ when y == map.Length - 1 => 'N',
            _ => throw new Exception("Invalid direction")
        };

        int energy = GetEnergy(x, y, direction);
        if (energy > maxEnergy)
        {
            maxEnergy = energy;
        }
    }
}
Console.WriteLine(maxEnergy);

void TraceBeam(int x, int y, char direction)
{
    if (x < 0 || x >= map[0].Length || y < 0 || y >= map.Length)
    {
        return;
    }

    energized[y][x] = true;

    var nextDirections = GetDirections(map[y][x], direction);
    foreach (var nextDirection in nextDirections)
    {
        if (nextSteps.Contains((x, y, nextDirection)))
        {
            continue;
        }
        nextSteps.Add((x, y, nextDirection));

        (x, y) = Next(x, y, nextDirection);
        TraceBeam(x, y, nextDirection);
    }
    return;
}

char[] GetDirections(char v, char direction)
{
    return v switch
    {
        '.' => [direction],
        '/' => direction switch
        {
            'N' => ['E'],
            'E' => ['N'],
            'S' => ['W'],
            'W' => ['S'],
            _ => throw new Exception("Invalid direction")
        },
        '\\' => direction switch
        {
            'N' => ['W'],
            'E' => ['S'],
            'S' => ['E'],
            'W' => ['N'],
            _ => throw new Exception("Invalid direction")
        },
        '|' => direction switch
        {
            'N' or 'S' => [direction],
            'E' or 'W' => ['N', 'S'],
            _ => throw new Exception("Invalid direction")
        },
        '-' => direction switch
        {
            'E' or 'W' => [direction],
            'N' or 'S' => ['E', 'W'],
            _ => throw new Exception("Invalid direction")
        },
        _ => [direction],
    };
}

(int nx, int ny) Next(int x, int y, char direction)
{
    return direction switch
    {
        'N' => (x, y - 1),
        'E' => (x + 1, y),
        'S' => (x, y + 1),
        'W' => (x - 1, y),
        _ => throw new Exception("Invalid direction"),
    };
}