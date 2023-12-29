var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var inputLines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);

var lines = new List<Line>();

foreach (var line in inputLines)
{
    var parts = line.Split("@");
    var numbers = parts[0].Trim().Split(",").Select(long.Parse).ToArray();
    var point = new Point(numbers[0], numbers[1], numbers[2]);

    var vs = parts[1].Trim().Split(",").Select(long.Parse).ToArray();
    var velocity = new Velocity(vs[0], vs[1], vs[2]);

    lines.Add(new Line(point, velocity));
}

int count = 0;
for (int i = 0; i < lines.Count - 1; i++)
{
    for (int j = i + 1; j < lines.Count; j++)
    {
        if (lines[i].Intersects(lines[j]))
        {
            count++;
        }
    }
}

Console.WriteLine(count);

record Point(long X, long Y, long Z);
record Velocity(long X, long Y, long Z);
record Line
{
    public Point P { get; init; }
    public Velocity V { get; init; }

    public double Slope { get; init; }
    public double Constant { get; init; }
    const long boundary1 = 200000000000000, boundary2 = 400000000000000;

    public Line(Point p, Velocity v)
    {
        P = p;
        V = v;

        // If the line was y = ax + b, then the slope would be a and the constant would be b.
        // Slope = y2-y1/x2-x1 which is the same as velocity y/velocity x
        Slope = (double)v.Y / v.X;
        Constant = p.Y - Slope * p.X;
    }

    public bool Intersects(Line other)
    {
        // Console.WriteLine();
        // Console.WriteLine($"Checking {P} {V} with {other.P} {other.V}");
        // If the slopes are the same, then the lines are parallel and will never intersect
        if (Slope == other.Slope)
        {
            // Console.WriteLine("Parallel");
            return false;
        }

        double x = (other.Constant - Constant) / (Slope - other.Slope);
        double y = Slope * x + Constant;
        // Console.WriteLine($"Intersection at {x},{y}");

        double time1 = (double)(x - P.X) / V.X;
        double time2 = (double)(x - other.P.X) / other.V.X;
        // Console.WriteLine($"Time: {time1} and {time2}");
        // If it's in the past then it doesn't count.
        if (time1 < 0 || time2 < 0)
        {
            // Console.WriteLine("Past");
            return false;
        }

        if (x >= boundary1 && x <= boundary2 && y >= boundary1 && y <= boundary2)
        {
            // Console.WriteLine("In the box");
            return true;
        }

        // Console.WriteLine("Not in the box");
        return false;
    }
}