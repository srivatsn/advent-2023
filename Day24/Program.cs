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


double[,] matrix = new double[4, 4];
double[] vector = new double[4];

// Part 2
// Got the math from this post - https://www.reddit.com/r/adventofcode/comments/18q40he/2023_day_24_part_2_a_straightforward_nonsolver/
for (int i = 0; i < 4; i++)
{
    // The equations are of the form:
    // (dy'-dy) X + (dx-dx') Y + (y-y') DX + (x'-x) DY = x' dy' - y' dx' - x dy + y dx
    // where (x,y) and (x',y') are the points and (dx,dy) and (dx',dy') are the velocities
    // The columns are X, Y, DX, DY, X

    matrix[i, 0] = lines[i + 1].V.Y - lines[i].V.Y;
    matrix[i, 1] = lines[i].V.X - lines[i + 1].V.X;
    matrix[i, 2] = lines[i].P.Y - lines[i + 1].P.Y;
    matrix[i, 3] = lines[i + 1].P.X - lines[i].P.X;
    vector[i] = lines[i + 1].P.X * lines[i + 1].V.Y - lines[i + 1].P.Y * lines[i + 1].V.X - lines[i].P.X * lines[i].V.Y + lines[i].P.Y * lines[i].V.X;
}

var solution = SolveLinearEquations(matrix, vector);

(double x, double y) = (solution[0], solution[1]);

// Now do the same thing for Y and Z
for (int i = 0; i < 4; i++)
{
    // The equations are of the form:
    // (dz'-dz) Y + (dy-dy') Z + (y-y') DY + (x'-x) DZ = x' dz' - y' dy' - x dz + y dy
    // where (x,y) and (x',y') are the points and (dy,dz) and (dy',dz') are the velocities
    // The columns are Y, Z, DY, DZ, Y

    matrix[i, 0] = lines[i + 1].V.Z - lines[i].V.Z;
    matrix[i, 1] = lines[i].V.Y - lines[i + 1].V.Y;
    matrix[i, 2] = lines[i].P.Z - lines[i + 1].P.Z;
    matrix[i, 3] = lines[i + 1].P.Y - lines[i].P.Y;
    vector[i] = lines[i + 1].P.Y * lines[i + 1].V.Z - lines[i + 1].P.Z * lines[i + 1].V.Y - lines[i].P.Y * lines[i].V.Z + lines[i].P.Z * lines[i].V.Y;
}

solution = SolveLinearEquations(matrix, vector);
double z = solution[1];

long total = (long)(Math.Round(x) + Math.Round(y) + Math.Round(z));
Console.WriteLine($"Total - {total}");

static double[] SolveLinearEquations(double[,] matrix, double[] vector)
{
    int length = vector.Length;

    for (int i = 0; i < length; i++)
    {
        double maxElement = Math.Abs(matrix[i, i]);
        int maxRow = i;
        for (int k = i + 1; k < length; k++)
        {
            if (Math.Abs(matrix[k, i]) > maxElement)
            {
                maxElement = Math.Abs(matrix[k, i]);
                maxRow = k;
            }
        }

        for (int k = i; k < length; k++)
        {
            double tmp = matrix[maxRow, k];
            matrix[maxRow, k] = matrix[i, k];
            matrix[i, k] = tmp;
        }

        double tmp2 = vector[maxRow];
        vector[maxRow] = vector[i];
        vector[i] = tmp2;

        for (int k = i + 1; k < length; k++)
        {
            double c = -matrix[k, i] / matrix[i, i];
            for (int j = i; j < length; j++)
            {
                if (i == j)
                {
                    matrix[k, j] = 0;
                }
                else
                {
                    matrix[k, j] += c * matrix[i, j];
                }
            }
            vector[k] += c * vector[i];
        }
    }

    double[] solution = new double[length];
    for (int i = length - 1; i >= 0; i--)
    {
        solution[i] = vector[i] / matrix[i, i];
        for (int k = i - 1; k >= 0; k--)
        {
            vector[k] -= matrix[k, i] * solution[i];
        }
    }
    return solution;
}


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