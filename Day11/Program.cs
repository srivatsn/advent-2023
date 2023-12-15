var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries).ToList();

// find index of rows that don't have a '#' in them and add a row of '.' to them
var rows = lines.Select((row, index) => (row, index)).Where(r => !r.row.Contains('#')).Select(r => r.index).ToList();
for (int r = 0; r < rows.Count; r++)
{
    lines.Insert(rows[r] + r + 1, string.Empty.PadLeft(lines[0].Length, 'M'));
}
Console.WriteLine("Added {0} rows", rows.Count);

// find index of columns that don't have a '#' in them and add a row of '.' to them
var columns = Enumerable.Range(0, lines[0].Length).Where(i => !lines.Select(r => r[i]).Contains('#')).ToList();
for (int c = 0; c < columns.Count; c++)
{
    lines = lines.Select(l => l.Insert(columns[c] + c + 1, "M")).ToList();
}
Console.WriteLine("Added {0} columns", columns.Count);

// Convers lines to a 2d array of strings
var universe = lines.Select(l => l.Select(c => c.ToString()).ToArray()).ToArray();

int counter = 0;
for (int y = 0; y < universe.Length; y++)
{
    for (int x = 0; x < universe[y].Length; x++)
    {
        if (universe[y][x] == "#")
        {
            counter++;
            universe[y][x] = counter.ToString();
        }
    }
}

// print the universe
for (int y = 0; y < universe.Length; y++)
{
    for (int x = 0; x < universe[y].Length; x++)
    {
        Console.Write(universe[y][x]);
    }
    Console.WriteLine();
}

// for each point find the shortest between it and every other point
var distances = new Dictionary<string, Dictionary<string, long>>();
for (int y = 0; y < universe.Length; y++)
{
    for (int x = 0; x < universe[y].Length; x++)
    {
        if (universe[y][x] != "." && universe[y][x] != "M")
        {
            var start = universe[y][x];
            distances[start] = [];
            for (int y2 = y; y2 < universe.Length; y2++)
            {
                for (int x2 = 0; x2 < universe[y2].Length; x2++)
                {
                    if (universe[y2][x2] != "." && universe[y2][x2] != "M" && int.Parse(universe[y2][x2]) > int.Parse(start))
                    {
                        var end = universe[y2][x2];
                        long distance = Math.Abs(x - x2) + Math.Abs(y - y2);
                        var spaces = universe[y][x < x2 ? x..x2 : x2..x].Count(c => c == "M") + universe.Where((c, i) => i > y && i < y2).Select(i => i[x]).Count(c => c == "M");
                        // Subtract 2 empty row replaced by 'M' already took up 1 and we traversed M and so we counted it as well. 
                        distance += spaces * (1000000 - 2);
                        distances[start][end] = distance;
                    }
                }
            }
        }
    }
}

// sumup all the distances
long sum = 0;
foreach (var start in distances.Keys)
{
    foreach (var end in distances[start].Keys)
    {
        Console.WriteLine("{0} -> {1} = {2}", start, end, distances[start][end]);
        sum += distances[start][end];
    }
}

Console.WriteLine(sum);