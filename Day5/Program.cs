var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);
var seeds = lines[0].Replace("seeds:", "").Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();

Dictionary<string, List<string>> mapinputs = [];
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
var maps = mapinputs.ToDictionary(kvp => kvp.Key, kvp => new Map([.. kvp.Value]));

// Go top down from seeds to location since there are fewer seeds.
void Part1(long[] seeds, Dictionary<string, Map> maps)
{
    long lowestLocation = long.MaxValue; // 15880236; //long.MaxValue;
    foreach (var seed in seeds)
    {
        var location = seed.MapTo(maps["seed-to-soil"])
                        .MapTo(maps["soil-to-fertilizer"])
                        .MapTo(maps["fertilizer-to-water"])
                        .MapTo(maps["water-to-light"])
                        .MapTo(maps["light-to-temperature"])
                        .MapTo(maps["temperature-to-humidity"])
                        .MapTo(maps["humidity-to-location"]);

        // Console.WriteLine($"Seed: {seed} -> {location}");
        if (location < lowestLocation)
        {
            lowestLocation = location;
        }
    }

    Console.WriteLine($"Lowest location: {lowestLocation}");
}

// Go bottom up from location to seeds since there are fewer locations.
void Part2(long[] seeds, Dictionary<string, Map> maps)
{
    for (long i = 0; i < long.MaxValue; i++)
    {
        var seed = i.ReverseMapTo(maps["humidity-to-location"])
                    .ReverseMapTo(maps["temperature-to-humidity"])
                    .ReverseMapTo(maps["light-to-temperature"])
                    .ReverseMapTo(maps["water-to-light"])
                    .ReverseMapTo(maps["fertilizer-to-water"])
                    .ReverseMapTo(maps["soil-to-fertilizer"])
                    .ReverseMapTo(maps["seed-to-soil"]);

        // Console.WriteLine($"Seed: {i} -> {seed}");
        if (seed.IsValidSeed(seeds))
        {
            Console.WriteLine($"Lowest location: {i}");
            break;
        }
    }
}

Part1(seeds, maps);
Part2(seeds, maps);


static class LongExtensions
{
    public static long MapTo(this long source, Map map)
    {
        return map[source];
    }

    public static long ReverseMapTo(this long destination, Map map)
    {
        return map.GetSource(destination);
    }

    public static IEnumerable<long> LongRange(this long start, long count)
    {
        for (long i = start; i < start + count; i++)
        {
            yield return i;
        }
    }

    public static bool IsInRange(this long value, long start, long length)
    {
        return value >= start && value < start + length;
    }

    public static bool IsValidSeed(this long value, long[] seeds)
    {
        for (int i = 0; i < seeds.Length; i += 2)
        {
            if (value.IsInRange(seeds[i], seeds[i + 1]))
            {
                return true;
            }
        }

        return false;
    }
}

record Map
{
    readonly SortedDictionary<long, (long Length, long Destination)> sourceMap = [];
    readonly SortedDictionary<long, (long Length, long Destination)> destinationMap = [];

    public Map(string[] lines)
    {
        foreach (var line in lines)
        {
            var numbers = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
            sourceMap.Add(numbers[1], (numbers[2], numbers[0]));
            destinationMap.Add(numbers[0], (numbers[2], numbers[1]));
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

    public long GetSource(long destination)
    {
        var last = destinationMap.Keys.LastOrDefault(k => k <= destination);
        if (last == 0 && !destinationMap.ContainsKey(0))
        {
            return destination;
        }

        var (length, source) = destinationMap[last];
        if (destination <= last + length)
        {
            return destination + source - last;
        }

        return destination;
    }
}
