var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);

var bricks = lines.Select(l =>
{
    var parts = l.Split('~');
    var pointA = parts[0].Split(',');
    var pointB = parts[1].Split(',');
    return new Brick(
        new Point(int.Parse(pointA[0]), int.Parse(pointA[1]), int.Parse(pointA[2])),
        new Point(int.Parse(pointB[0]), int.Parse(pointB[1]), int.Parse(pointB[2])));
}).OrderBy(x => x.A.Z < x.B.Z ? x.A.Z : x.B.Z).ToArray();

// Let the bricks fall to the ground and settle.
for (int i = 0; i < bricks.Length; i++)
{
    Brick? brick = bricks[i];
    var newBrick = LetItFall(brick, i);
    if (brick != newBrick)
    {
        brick.A = newBrick.A;
        brick.B = newBrick.B;
    }
}

// Part 1 - look for bricks that are not supporting any other brick
int numCanBeDisintegrated = 0;
for (int i = 0; i < bricks.Length; i++)
{
    Brick? brick = bricks[i];
    if (!IsSupportingBrick(brick, bricks))
    {
        numCanBeDisintegrated++;
    }
}

Console.WriteLine($"Number of bricks that can be disintegrated: {numCanBeDisintegrated}");

// Part 2 - For each brick, remove it and count all the bricks that get moved because of it.
int countOfTotalMoves = 0;
for (int i = 0; i < bricks.Length; i++)
{
    Brick? brick = bricks[i];
    var newBricks = bricks.Where(b => b != brick).ToArray();
    for (int j = 0; j < newBricks.Length; j++)
    {
        var oldBrick = newBricks[j];
        var newBrick = LetItFall(oldBrick, j, null, newBricks);
        if (oldBrick != newBrick)
        {
            newBricks[j] = newBrick;
            countOfTotalMoves++;
        }
    }
    Console.WriteLine($"After brick {i + 1} is removed, number of bricks that can be disintegrated: {countOfTotalMoves}");
}

Console.WriteLine($"Total number of moves: {countOfTotalMoves}");

bool IsSupportingBrick(Brick brick, Brick[] bricks)
{
    // Find bricks just above it
    var bricksAbove = bricks.Where(b => b.A.Z == brick.A.Z + 1 || b.A.Z == brick.B.Z + 1 ||
                                        b.B.Z == brick.A.Z + 1 || b.B.Z == brick.B.Z + 1).ToArray();

    if (bricksAbove.Length == 0)
    {
        return false;
    }

    foreach (var brickAbove in bricksAbove)
    {
        var newBrick = LetItFall(brickAbove, Array.IndexOf(bricks, brickAbove), brick);
        if (brickAbove != newBrick)
        {
            return true;
        }
    }

    return false;
}

Brick LetItFall(Brick brick, int index, Brick? brickToRemove = null, Brick[]? allBricks = null)
{
    var newBrick = brick;
    var lowerZ = brick.A.Z < brick.B.Z ? brick.A.Z : brick.B.Z;
    allBricks ??= bricks;

    while (true)
    {
        // If brick is on the ground, return
        if (newBrick.A.Z == 1 || newBrick.B.Z == 1)
        {
            return newBrick;
        }

        var movedBrick = new Brick(new Point(newBrick.A.X, newBrick.A.Y, newBrick.A.Z - 1), new Point(newBrick.B.X, newBrick.B.Y, newBrick.B.Z - 1));

        // If movedBrick intersects with any other brick, return
        for (int i = index - 1; i >= 0; i--)
        {
            var brickAtZ = allBricks[i];
            if (brickAtZ != brick && brickAtZ != brickToRemove && brickAtZ.Intersects(movedBrick))
            {
                return newBrick;
            }
        }
        newBrick = movedBrick;
    }
}

record Point(int X, int Y, int Z);
record Brick(Point A, Point B)
{
    public Point A { get; set; } = A;
    public Point B { get; set; } = B;
    public bool Intersects(Brick other)
    {
        return A.X <= other.B.X && B.X >= other.A.X &&
               A.Y <= other.B.Y && B.Y >= other.A.Y &&
               A.Z <= other.B.Z && B.Z >= other.A.Z;
    }
}