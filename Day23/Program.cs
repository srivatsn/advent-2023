var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);

var map = lines.Select(l => l.ToCharArray()).ToArray();
var start = lines[0].IndexOf('.');

bool[][] visited = new bool[map.Length][];
for (int i = 0; i < map.Length; i++)
{
    visited[i] = new bool[map[i].Length];
}
int maxPathLen = 0;

// Part 1
var longestPath = FindLongestPath(map, start, 0, 0, visited, isSlippery: true);
Console.WriteLine("Longest path: " + longestPath);

maxPathLen = 0;
// Part 2
var l1 = FindLongestPath(map, start, 0, 0, visited, isSlippery: false);
Console.WriteLine("Longest path when not slippery: " + l1);

int FindLongestPath(char[][] map, int x, int y, int pathLength, bool[][] visited, bool isSlippery)
{
    if (x < 0 || x >= map[0].Length || y < 0 || y >= map.Length || map[y][x] == '#' || visited[y][x])
    {
        return -1; // Return -1 for invalid paths
    }

    if (y == map.Length - 1) // If we're on the last row
    {
        var length = map[y][x] == '.' ? pathLength : -1; // Return pathLength if it's a '.' cell, otherwise return -1
        if (length > maxPathLen)
        {
            maxPathLen = length;
            Console.WriteLine("Found path of length " + length);
        }
        return length;
    }

    visited[y][x] = true;
    int maxPath = pathLength;

    if (map[y][x] == '>' && isSlippery)
    {
        maxPath = Math.Max(maxPath, FindLongestPath(map, x + 1, y, pathLength + 1, visited, isSlippery)); // Right
    }
    else if (map[y][x] == '<' && isSlippery)
    {
        maxPath = Math.Max(maxPath, FindLongestPath(map, x - 1, y, pathLength + 1, visited, isSlippery)); // Left
    }
    else if (map[y][x] == '^' && isSlippery)
    {
        maxPath = Math.Max(maxPath, FindLongestPath(map, x, y - 1, pathLength + 1, visited, isSlippery)); // Up
    }
    else if (map[y][x] == 'v' && isSlippery)
    {
        maxPath = Math.Max(maxPath, FindLongestPath(map, x, y + 1, pathLength + 1, visited, isSlippery)); // Down
    }
    else
    {
        maxPath = Math.Max(maxPath, FindLongestPath(map, x - 1, y, pathLength + 1, visited, isSlippery)); // Left
        maxPath = Math.Max(maxPath, FindLongestPath(map, x + 1, y, pathLength + 1, visited, isSlippery)); // Right
        maxPath = Math.Max(maxPath, FindLongestPath(map, x, y - 1, pathLength + 1, visited, isSlippery)); // Up
        maxPath = Math.Max(maxPath, FindLongestPath(map, x, y + 1, pathLength + 1, visited, isSlippery)); // Down
    }
    visited[y][x] = false;
    return maxPath == pathLength ? -1 : maxPath;
}