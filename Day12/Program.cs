var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries).ToList();

List<RecordUnit> CreatePart1Records(List<string> lines)
{
    var records = new List<RecordUnit>();
    foreach (var line in lines)
    {
        var parts = line.Split(" ");
        var recordString = parts[0];
        var numbers = parts[1].Split(",").Select(int.Parse).ToList();
        // // Repeat parts[0] 5 times
        // var recordString = Enumerable.Repeat(parts[0], 5).Aggregate((x, y) => x + "?" + y); ;
        // var numbers = Enumerable.Repeat(parts[1].Split(",").Select(int.Parse).ToList(), 5).SelectMany(x => x).ToList();
        records.Add(new RecordUnit(recordString, numbers));
    }
    return records;
}

List<RecordUnit> CreatePart2Records(List<string> lines)
{
    var records = new List<RecordUnit>();
    foreach (var line in lines)
    {
        var parts = line.Split(" ");
        // Repeat parts[0] 5 times
        var recordString = Enumerable.Repeat(parts[0], 5).Aggregate((x, y) => x + "?" + y); ;
        var numbers = Enumerable.Repeat(parts[1].Split(",").Select(int.Parse).ToList(), 5).SelectMany(x => x).ToList();
        records.Add(new RecordUnit(recordString, numbers));
    }
    return records;
}

var memory = new Dictionary<RecordUnit, long>();
void SumAllCombinations(List<RecordUnit> records)
{
    var combinations = new Dictionary<int, long>();

    for (int i = 0; i < records.Count; i++)
    {
        memory.Clear();
        var (record, numbers) = (records[i].Record, records[i].Numbers);

        combinations[i] = FindValidCombinations(record, numbers);
    }
    // sum all combinations
    Console.WriteLine(combinations.Values.Sum());
}

long FindValidCombinations(string record, List<int> numbers)
{
    if (memory.ContainsKey(new RecordUnit(record, numbers)))
    {
        return memory[new RecordUnit(record, numbers)];
    }

    long combinations = 0;
    if (record.Length < numbers.Sum(x => x + 1) - 1)
    {
        return 0;
    }

    if (record.Length == 0)
    {
        return 1;
    }

    if (record[0] == '.' || record[0] == '?')
    {
        combinations += FindValidCombinations(record[1..], numbers);
    }
    if (record[0] == '#' || record[0] == '?')
    {
        var nextNumber = numbers.FirstOrDefault(-1);

        // Not enough strings in record or numbers
        if (nextNumber == -1 || record.Length < nextNumber)
        {
            memory[new RecordUnit(record, numbers)] = combinations;
            return combinations + 0;
        }

        // If end of record then we have a valid combination
        if (record.Length == nextNumber && !record.Contains('.'))
        {
            memory[new RecordUnit(record, numbers)] = combinations + 1;
            return combinations + 1;
        }
        else
        {
            var nextSeq = record.Substring(0, nextNumber);
            if (nextSeq.Contains('.') || record[nextNumber] == '#')
            {
                memory[new RecordUnit(record, numbers)] = combinations;
                return combinations + 0;
            }

            combinations += FindValidCombinations(record[(nextNumber + 1)..], numbers[1..]);
        }
    }

    memory[new RecordUnit(record, numbers)] = combinations;
    return combinations;
}

SumAllCombinations(CreatePart1Records(lines));
SumAllCombinations(CreatePart2Records(lines));

class RecordUnit(string record, List<int> numbers)
{
    public string Record => record;
    public List<int> Numbers => numbers;

    public override int GetHashCode()
    {
        return Record.GetHashCode() + Numbers.Sum(n => n.GetHashCode());
    }

    public override bool Equals(object? other)
    {
        if (other is not RecordUnit otherMemoryUnit)
        {
            return false;
        }

        return Record == otherMemoryUnit.Record && Numbers.SequenceEqual(otherMemoryUnit.Numbers);
    }
}