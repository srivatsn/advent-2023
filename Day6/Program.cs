using System.Diagnostics.Contracts;

var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);

void FindMargin(long[] times, long[] distances)
{
    long margin = 1;
    for (int i = 0; i < times.Length; i++)
    {
        long waysToBeatRecord = 0;
        for (long chargeTime = 1; chargeTime <= times[i]; chargeTime++)
        {
            var distance = (times[i] - chargeTime) * chargeTime;
            if (distance > distances[i])
            {
                waysToBeatRecord++;
            }
        }
        margin *= waysToBeatRecord;
    }

    Console.WriteLine($"Margin: {margin}");
}

// Part 1
var times = lines[0].Replace("Time: ", "").Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
var distances = lines[1].Replace("Distance: ", "").Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
FindMargin(times, distances);

// Part 2
times = [long.Parse(lines[0].Replace("Time: ", "").Replace(" ", ""))];
distances = [long.Parse(lines[1].Replace("Distance: ", "").Replace(" ", ""))];
FindMargin(times, distances);
