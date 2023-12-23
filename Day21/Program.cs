var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);

var map = lines.Select(l => l.ToCharArray()).ToArray();

var (startx, starty) = GetStartingPosition();
var nextSteps = new Queue<(int x, int y)>();
nextSteps.Enqueue((startx, starty));

for (int i = 0; i < 20; i++)
{
    // Dequeue all the steps from nextsteps
    var steps = new List<(int x, int y)>();
    while (nextSteps.Count > 0)
    {
        steps.Add(nextSteps.Dequeue());
    }

    // For each step, enqueue all the adjacent steps
    foreach (var (x, y) in steps)
    {
        Visit(x - 1, y);
        Visit(x + 1, y);
        Visit(x, y - 1);
        Visit(x, y + 1);
    }

    Console.WriteLine($"After {i + 1} turns: {nextSteps.Count} steps");
    PrintMap(nextSteps);
}

void PrintMap(Queue<(int x, int y)> nextSteps)
{
    for (int y = 0; y < map.Length; y++)
    {
        for (int x = 0; x < map[y].Length; x++)
        {
            if (nextSteps.Any(s => s.x == x && s.y == y))
            {
                Console.Write('O');
            }
            else
            {
                Console.Write(map[y][x]);
            }
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

Console.WriteLine(nextSteps.Count());

void Visit(int x, int y)
{
    if (x < 0 || x >= map[0].Length || y < 0 || y >= map.Length)
    {
        return;
    }
    if (map[y][x] != '#')
    {
        if (!nextSteps.Contains((x, y)))
        {
            nextSteps.Enqueue((x, y));
        }
    }
}

(int x, int y) GetStartingPosition()
{
    for (int y = 0; y < map.Length; y++)
    {
        for (int x = 0; x < map[y].Length; x++)
        {
            if (map[y][x] == 'S')
            {
                return (x, y);
            }
        }
    }
    throw new Exception("No starting position found");
}