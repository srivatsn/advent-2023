var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);

char[][] map = lines.Select(l => l.ToCharArray()).ToArray();
// path is a 2D array of 0s and 1s. 0 means not visited, 1 means visited.
int[][] path = new int[lines.Length][];
path = path.Select(p => new int[map[0].Length]).ToArray();

// The map has a 'S' for the starting point but deduce the actual starting pipe by looking at the two starting vectors and the direction they are pointing.
char FindStartingPipe(char[][] map, int startX, int startY, List<Vector> currentVectors)
{
    if (currentVectors.Count != 2)
    {
        throw new Exception($"Expected 2 starting vectors, found {currentVectors.Count}");
    }

    var sequence = currentVectors[0].Direction.ToString() + currentVectors[1].Direction.ToString();

    return sequence switch
    {
        "EW" or "WE" => '-',
        "EN" or "NE" => 'L',
        "ES" or "SE" => 'F',
        "WN" or "NW" => 'J',
        "WS" or "SW" => '7',
        "NS" or "SN" => '|',

        _ => throw new Exception($"Unknown sequence: {sequence} at {startX}, {startY}")
    };
}

// Find all the starting vectors. There should be 2.
List<Vector> FindStartingVectors(char[][] map, int startX, int startY)
{
    var vectors = new List<Vector>();
    if (startY > 0 && "|7F".Contains(map[startY - 1][startX]))
    {
        vectors.Add(new Vector(startX, startY - 1, 'N'));
    }
    if (startY < map.Length - 1 && "|LJ".Contains(map[startY + 1][startX]))
    {
        vectors.Add(new Vector(startX, startY + 1, 'S'));
    }
    if (startX > 0 && "-LF".Contains(map[startY][startX - 1]))
    {
        vectors.Add(new Vector(startX - 1, startY, 'W'));
    }
    if (startX < map[startY].Length - 1 && "-J7".Contains(map[startY][startX + 1]))
    {
        vectors.Add(new Vector(startX + 1, startY, 'E'));
    }
    return vectors;
}

// Given a vector, find the next vector to traverse to based on the current direction.
Vector GetNextVector(char[][] map, Vector p)
{
    var sequence = p.Direction.ToString() + map[p.Y][p.X].ToString();
    var nextVector = sequence switch
    {
        "E-" => new Vector(p.X + 1, p.Y, 'E'),
        "W-" => new Vector(p.X - 1, p.Y, 'W'),
        "E7" or "WF" => new Vector(p.X, p.Y + 1, 'S'),
        "EJ" or "WL" => new Vector(p.X, p.Y - 1, 'N'),
        "N|" => new Vector(p.X, p.Y - 1, 'N'),
        "S|" => new Vector(p.X, p.Y + 1, 'S'),
        "NF" or "SL" => new Vector(p.X + 1, p.Y, 'E'),
        "N7" or "SJ" => new Vector(p.X - 1, p.Y, 'W'),
        _ => throw new Exception($"Unknown sequence: {sequence} at {p.X}, {p.Y}")
    };

    return nextVector;
}

// Find the starting point and mark it as visited.
var (_, startX, startY) = map.SelectMany((row, y) => row.Select((c, x) => (c, x, y))).First(p => p.c == 'S');
path[startY][startX] = 1;

// Find the starting vectors. There should be 2.
var currentVectors = FindStartingVectors(map, startX, startY);
char startingPipe = FindStartingPipe(map, startX, startY, currentVectors);
currentVectors.ForEach(p => path[p.Y][p.X] = 1);

// Part 1
int counter = 1;
while (!currentVectors.All(p => p.X == currentVectors.First().X && p.Y == currentVectors.First().Y))
{
    currentVectors = currentVectors.Select(p => GetNextVector(map, p)).ToList();
    currentVectors.ForEach(p => path[p.Y][p.X] = 1);
    counter++;
}
Console.WriteLine($"Steps: {counter}");

// Part 2
// From any point, traverse in one direction and see how many times we cross a wall. Even means we are outside and odd means we are inside.
for (int y = 0; y < path.Length; y++)
{
    for (int x = 0; x < path[0].Length; x++)
    {
        if (path[y][x] != 0)
        {
            continue;
        }

        int count = 0;
        string connectedSegment = "";
        // Count the number of times we cross a wall in the horizontal direction.
        // If there is a connected segment of pipes we want to look at them together
        // If the segment is looks like F7 or LJ then we count it two crossings
        // If the segment is looks like FJ or L7 then we count it as one crossing since it's basically a straight line with a bend.
        for (int i = x + 1; i < path[y].Length; i++)
        {
            if (isConnected(map[y][i], map[y][i - 1]) && path[y][i] == 1)
            {
                connectedSegment += map[y][i].ToString().Replace('S', startingPipe);
                continue;
            }
            else if (connectedSegment.Length > 1)
            {
                if ((connectedSegment[0] == 'F' && connectedSegment.Last() == '7') ||
                    (connectedSegment[0] == 'L' && connectedSegment.Last() == 'J'))
                {
                    count++;
                }
            }
            connectedSegment = map[y][i].ToString().Replace('S', startingPipe);

            if (path[y][i] == 1)
            {
                count++;
            }
        }
        if (connectedSegment.Length > 1 &&
            ((connectedSegment[0] == 'F' && connectedSegment.Last() == '7') ||
             (connectedSegment[0] == 'L' && connectedSegment.Last() == 'J')))
        {
            count++;
        }

        // 2 means outside, 3 means inside
        path[y][x] = count % 2 == 0 ? 2 : 3;
    }
}

var inside = path.SelectMany(p => p).Count(p => p == 3);
Console.WriteLine($"Inside points: {inside}");

// Helper function to determine if two pipes are connected.
bool isConnected(char current, char previous)
{
    string sequence = previous.ToString() + current.ToString();
    return sequence.Replace('S', startingPipe) switch
    {
        "--" or "F-" or "L-" or "-7" or "-J" or "FJ" or "F7" or "L7" or "LJ" => true,
        _ => false
    };
}

record Vector(int X, int Y, char Direction)
{
    public override string ToString() => $"({X}, {Y}, {Direction})";
}