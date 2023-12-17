var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n").ToList();

char[][] map = lines.Select(l => l.ToCharArray()).ToArray();

void RollNorth()
{
    for (int j = 0; j < map[0].Length; j++)
    {
        for (int i = 1; i < map.Length; i++)
        {

            if (map[i][j] == '.' || map[i][j] == '#')
            {
                continue;
            }

            var columnSlice = map.Select(l => l[j]).ToArray()[..i];
            var index = Array.FindLastIndex(columnSlice, c => c == '#' || c == 'O');
            if (index == i - 1)
            {
                continue;
            }
            map[index + 1][j] = map[i][j];
            map[i][j] = '.';
        }
    }
}

void RollWest()
{
    for (int i = 0; i < map.Length; i++)
    {
        for (int j = 1; j < map[i].Length; j++)
        {
            if (map[i][j] == '.' || map[i][j] == '#')
            {
                continue;
            }

            var rowSlice = map[i][..j];
            var index = Array.FindLastIndex(rowSlice, c => c == '#' || c == 'O');
            if (index == j - 1)
            {
                continue;
            }
            map[i][index + 1] = map[i][j];
            map[i][j] = '.';
        }
    }
}

void RollSouth()
{
    for (int j = 0; j < map[0].Length; j++)
    {
        for (int i = map.Length - 2; i >= 0; i--)
        {

            if (map[i][j] == '.' || map[i][j] == '#')
            {
                continue;
            }

            var columnSlice = map.Select(l => l[j]).ToArray()[(i + 1)..];
            var index = i + Array.FindIndex(columnSlice, c => c == '#' || c == 'O');
            if (index == i)
            {
                continue;
            }
            if (index == i - 1)
            {
                index = map.Length - 1;
            }

            map[index][j] = map[i][j];
            map[i][j] = '.';
        }
    }
}

void RollEast()
{
    for (int i = 0; i < map.Length; i++)
    {
        for (int j = map[i].Length - 2; j >= 0; j--)
        {
            if (map[i][j] == '.' || map[i][j] == '#')
            {
                continue;
            }

            var rowSlice = map[i][(j + 1)..];
            var index = j + Array.FindIndex(rowSlice, c => c == '#' || c == 'O');
            if (index == j)
            {
                continue;
            }
            if (index == j - 1)
            {
                index = map[i].Length - 1;
            }

            map[i][index] = map[i][j];
            map[i][j] = '.';
        }
    }
}

void Cycle()
{
    RollNorth();
    RollWest();
    RollSouth();
    RollEast();
}

var cyclesSeen = new Dictionary<string, int>();
const int NUMITERATIONS = 1000000000;
for (int i = 0; i < NUMITERATIONS; i++)
{
    var mapString = map.Select(l => new string(l)).Aggregate((x, y) => x + "\n" + y);
    int cycleSize = -1;
    if (cyclesSeen!.TryGetValue(mapString, out var cycleNumber))
    {
        cycleSize = i - cycleNumber;
    }
    else
    {
        cyclesSeen.Add(mapString, i);
    }

    Cycle();

    if (cycleSize != -1)
    {
        i = NUMITERATIONS - ((NUMITERATIONS - i) % cycleSize);
    }
}

Console.WriteLine($"Weight: {ComputeWeight(map)}");

static int ComputeWeight(char[][] map)
{
    int totalWeight = 0;
    for (int i = 0; i < map.Length; i++)
    {
        for (int j = 0; j < map[i].Length; j++)
        {
            if (map[i][j] == 'O')
            {
                totalWeight += map.Length - i;
            }
        }
    }

    return totalWeight;
}