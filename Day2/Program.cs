using System.Text.RegularExpressions;

var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);

Dictionary<string, int> expected = new Dictionary<string, int> {
    {"red", 12 }, {"green", 13}, {"blue", 14}
};

var games = new List<Game>();
foreach (var line in lines)
{
    var parts = line.Split(":");
    var game = parts[0];
    var set = parts[1].Split(";");
    var gameNumber = Regex.Match(game, "Game (?<game>\\d+)").Groups["game"].Value;
    var gameList = new List<Set>();
    foreach (var item in set)
    {
        var cubes = item.Split(",");
        var setObj = new List<Cube>();
        foreach (var cube in cubes)
        {
            var match = Regex.Match(cube, "(?<number>\\d+) (?<color>\\w+)");
            var number = match.Groups["number"].Value;
            var color = match.Groups["color"].Value;
            var cubeObj = new Cube(int.Parse(number), color);
            setObj.Add(cubeObj);
        }
        gameList.Add(new Set(Cube: [.. setObj]));
    }
    games.Add(new Game(Number: int.Parse(gameNumber), Set: [.. gameList]));
}

bool IsValidCube(Cube cube)
{
    if (expected.ContainsKey(cube.Color) && expected[cube.Color] >= cube.Number)
    {
        return true;
    }
    return false;
}

bool IsValidSet(Set set)
{
    foreach (var cube in set.Cube)
    {
        if (!IsValidCube(cube))
        {
            return false;
        }
    }
    return true;
}

bool IsValidGame(Game game)
{
    foreach (var set in game.Set)
    {
        if (!IsValidSet(set))
        {
            return false;
        }
    }
    return true;
}

int FindPowerOfGame(Game game)
{
    var maxRed = game.Set.Max(set => set.Cube.FirstOrDefault(cube => cube.Color == "red")?.Number ?? 0);
    var maxGreen = game.Set.Max(set => set.Cube.FirstOrDefault(cube => cube.Color == "green")?.Number ?? 0);
    var maxBlue = game.Set.Max(set => set.Cube.FirstOrDefault(cube => cube.Color == "blue")?.Number ?? 0);

    return maxRed * maxGreen * maxBlue;
}

// int sumOfIndex = 0;
// foreach (var game in games)
// {
//     var gameNumber = game.Number;
//     if (IsValidGame(game))
//     {
//         sumOfIndex += gameNumber;
//     }
// }

// Console.WriteLine($"Sum of index: {sumOfIndex}");

int sum = games.Sum(game => FindPowerOfGame(game));
Console.WriteLine($"Sum of power: {sum}");

record Cube(int Number, string Color);
record Set(Cube[] Cube);
record Game(int Number, Set[] Set);

