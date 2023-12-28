using System.Collections.Immutable;
using System.Security.Cryptography.X509Certificates;

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
var (longestPath, fpaths) = FindLongestPath(map, start, 0, 0, visited, isSlippery: true, path: []);
Console.WriteLine("Longest path: " + longestPath);

maxPathLen = 0;
// Part 2
var (l1, paths) = FindLongestPath(map, start, 0, 0, visited, isSlippery: false, []);
Console.WriteLine("Longest path when not slippery: " + l1);

foreach (var (x, y) in paths)
{
    map[y][x] = 'O';
}

Console.WriteLine("Map:");
foreach (var line in map)
{
    Console.WriteLine(line);
}

(int maxPath, ImmutableArray<(int x, int y)>?) FindLongestPath(char[][] map, int x, int y, int pathLength, bool[][] visited, bool isSlippery, ImmutableArray<(int x, int y)> path)
{
    if (x < 0 || x >= map[0].Length || y < 0 || y >= map.Length || map[y][x] == '#' || visited[y][x])
    {
        return (-1, null); // Return -1 for invalid paths
    }

    if (y == map.Length - 1) // If we're on the last row
    {
        var length = map[y][x] == '.' ? pathLength : -1; // Return pathLength if it's a '.' cell, otherwise return -1
        if (length > maxPathLen)
        {
            maxPathLen = length;
            Console.WriteLine("Found path of length " + length);
        }
        return (length, path.Add((x, y)));
    }

    visited[y][x] = true;
    int maxPath = pathLength;
    var retPath = null as ImmutableArray<(int x, int y)>?;

    if (map[y][x] == '>' && isSlippery)
    {
        (maxPath, retPath) = FindMaxPath(maxPath, retPath, FindLongestPath(map, x + 1, y, pathLength + 1, visited, isSlippery, path)); // Right
    }
    else if (map[y][x] == '<' && isSlippery)
    {
        (maxPath, retPath) = FindMaxPath(maxPath, retPath, FindLongestPath(map, x - 1, y, pathLength + 1, visited, isSlippery, path)); // Left
    }
    else if (map[y][x] == '^' && isSlippery)
    {
        (maxPath, retPath) = FindMaxPath(maxPath, retPath, FindLongestPath(map, x, y - 1, pathLength + 1, visited, isSlippery, path)); // Up
    }
    else if (map[y][x] == 'v' && isSlippery)
    {
        (maxPath, retPath) = FindMaxPath(maxPath, retPath, FindLongestPath(map, x, y + 1, pathLength + 1, visited, isSlippery, path)); // Down
    }
    else
    {
        (maxPath, retPath) = FindMaxPath(maxPath, retPath, FindLongestPath(map, x - 1, y, pathLength + 1, visited, isSlippery, path)); // Left
        (maxPath, retPath) = FindMaxPath(maxPath, retPath, FindLongestPath(map, x + 1, y, pathLength + 1, visited, isSlippery, path)); // Right
        (maxPath, retPath) = FindMaxPath(maxPath, retPath, FindLongestPath(map, x, y - 1, pathLength + 1, visited, isSlippery, path)); // Up
        (maxPath, retPath) = FindMaxPath(maxPath, retPath, FindLongestPath(map, x, y + 1, pathLength + 1, visited, isSlippery, path)); // Down
    }
    visited[y][x] = false;
    return (maxPath == pathLength ? -1 : maxPath, retPath.HasValue ? retPath.Value.Add((x, y)) : null);
}

static (int maxPath, ImmutableArray<(int x, int y)>? Value) FindMaxPath(int currentMax, ImmutableArray<(int x, int y)>? path, (int returnMax, ImmutableArray<(int x, int y)>? path) retVal)
{
    var (returnMax, returnPath) = retVal;
    var maxPath = Math.Max(currentMax, returnMax);
    if (maxPath == returnMax && returnPath.HasValue)
    {
        return (maxPath, returnPath.Value);
    }
    else
    {
        return (maxPath, path);
    }
}