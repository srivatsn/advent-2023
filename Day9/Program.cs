var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);

var histories = lines.Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()).ToArray();

int sumNext = 0;
int sumPrev = 0;
foreach (var history in histories)
{
    var nextValue = PredictNextValue(history);
    var prevValue = PredictPreviousValue(history);
    sumNext += nextValue;
    sumPrev += prevValue;
}
Console.WriteLine(sumNext);
Console.WriteLine(sumPrev);

int PredictNextValue(int[] history)
{
    if (history.All(v => v == 0))
    {
        return 0;
    }

    var differenceArray = history.Select((v, i) => i == 0 ? 0 : v - history[i - 1]).Skip(1).ToArray();
    var nextDiffValue = PredictNextValue(differenceArray);
    return history.Last() + nextDiffValue;
}

int PredictPreviousValue(int[] history)
{
    if (history.All(v => v == 0))
    {
        return 0;
    }

    var differenceArray = history.Select((v, i) => i == 0 ? 0 : v - history[i - 1]).Skip(1).ToArray();
    var nextPrevValue = PredictPreviousValue(differenceArray);
    return history.First() - nextPrevValue;
}