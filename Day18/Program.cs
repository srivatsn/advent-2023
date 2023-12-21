var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n");

var instructions = lines.Select(line =>
{
    var parts = line.Split(" ");
    return new Instruction(parts[0], int.Parse(parts[1]), parts[2]);
}).ToArray();

Console.WriteLine($"Part 1: {FindAllDugPoints()}");

//Part 2
instructions = instructions.Select(i => Instruction.ParseColor(i.Color)).ToArray();
Console.WriteLine($"Part 2: {FindAllDugPoints()}");

long FindAllDugPoints()
{
    //Find the list of vertices
    var vertices = new List<(long x, long y)>
    {
        (0, 0)
    };

    long x = 0, y = 0;
    long numedgepoints = 0;
    foreach (var instruction in instructions)
    {
        y += instruction.Direction switch
        {
            "U" => -instruction.Count,
            "D" => instruction.Count,
            _ => 0
        };
        x += instruction.Direction switch
        {
            "R" => instruction.Count,
            "L" => -instruction.Count,
            _ => 0
        };

        vertices.Add((x, y));
        numedgepoints += instruction.Count;
    }

    // Use shoelace formula to find the area of the polygon
    // https://en.wikipedia.org/wiki/Shoelace_formula
    long area = 0;
    for (int i = 0; i < vertices.Count - 1; i++)
    {
        int yplus1 = i == vertices.Count - 2 ? 0 : i + 1;
        int yminus1 = i == 0 ? vertices.Count - 1 : i - 1;

        area += vertices[i].x * (vertices[yplus1].y - vertices[yminus1].y);
    }
    area = Math.Abs(area) / 2;

    // Now use Pick's theorem to find the number of points inside the polygon
    // Pick's theorem: A = i + b/2 - 1
    // A = area of the polygon
    // i = number of points inside the polygon
    // b = number of points on the boundary of the polygon

    long insidePoints = area - (numedgepoints / 2) + 1;

    return numedgepoints + insidePoints;
}

record Instruction(string Direction, int Count, string Color)
{
    public static Instruction ParseColor(string color)
    {
        var val = color.Replace("(#", "").Replace(")", "");
        var distance = Convert.ToInt32(val[0..5], 16);
        int directionVal = int.Parse(val[5].ToString());
        var direction = directionVal switch
        {
            0 => "R",
            1 => "D",
            2 => "L",
            3 => "U",
            _ => throw new Exception("Invalid direction")
        };

        return new Instruction(direction, distance, color);
    }
}