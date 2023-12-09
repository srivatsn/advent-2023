var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);

// Part 1
var fullSeeds = lines[0].Replace("seeds:", "").Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();

// Part 2 
long lowestLocation = long.MaxValue;
for (int i =0; i < fullSeeds.Length; i+=2)
{
    Console.WriteLine("Iteration: " + i);
    var seeds = fullSeeds[i].LongRange(fullSeeds[i+1]).ToArray();
    Console.WriteLine($"Seeds: {seeds.Length}");
    Dictionary<string, List<string>> mapinputs = new Dictionary<string, List<string>>();
    foreach (var line in lines.Skip(1))
    {
        if (line.EndsWith("map:"))
        {
            var mapName = line.Replace(" map:", "");
            mapinputs.Add(mapName, []);
        }
        else if (!string.IsNullOrWhiteSpace(line))
        {
            mapinputs.Last().Value.Add(line);
        }
    }

    var maps = mapinputs.ToDictionary(kvp => kvp.Key, kvp => new Map(kvp.Value.ToArray()));
    Console.WriteLine($"Maps: {maps.Count}");
    foreach (var seed in seeds)
    {
        var location = seed.MapTo(maps["seed-to-soil"])
                        .MapTo(maps["soil-to-fertilizer"])
                        .MapTo(maps["fertilizer-to-water"])
                        .MapTo(maps["water-to-light"])
                        .MapTo(maps["light-to-temperature"])
                        .MapTo(maps["temperature-to-humidity"])
                        .MapTo(maps["humidity-to-location"]);

        if (location < lowestLocation)
        {
            lowestLocation = location;
        }
    }
    Console.WriteLine($"Lowest location: {lowestLocation}");
}

Console.WriteLine($"Lowest location: {lowestLocation}");

static class LongExtensions
{
    public static long MapTo(this long source, Map map)
    {
        return map[source];
    }

    public static IEnumerable<long> LongRange(this long start, long count)
    {
        for (long i = start; i < start + count; i++)
        {
            yield return i;
        }
    }
}

record Map
{
    readonly SortedDictionary<long, (long Length, long Destination)> sourceMap = [];

    public Map(string[] lines)
    {
        foreach (var line in lines)
        {
            var numbers = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
            sourceMap.Add(numbers[1], (numbers[2], numbers[0]));
        }
    }

    public long this[long source]
    {
        get
        {
            var last = sourceMap.Keys.LastOrDefault(k => k <= source);
            if (last == 0 && !sourceMap.ContainsKey(0))
            {
                return source;
            }

            var (length, destination) = sourceMap[last];
            if (source <= last + length)
            {
                return destination + source - last;
            }

            return source;
        }
    }
}