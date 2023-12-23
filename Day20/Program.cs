var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);

var modules = ParseModules(lines);

int cycleCount = 1;
long lowPulseCount = 0, highPulseCount = 0;
for (int i = 1; i <= 1000; i++)
{
    (int lc, int hc) = PressButton();
    lowPulseCount += lc;
    highPulseCount += hc;
    if (modules.All(kvp => kvp.Value.IsInInitialState()))
    {
        cycleCount = i;
        lowPulseCount = 1000 / cycleCount * lowPulseCount;
        highPulseCount = 1000 / cycleCount * highPulseCount;
        break;
    }

}
long totalCount = lowPulseCount * highPulseCount;
Console.WriteLine($"Total count: {totalCount}");

// Part 2
var output = new OutputModule("rx");
output.SetInputs(modules);
var rxInput = output.Inputs[0];
var theirInputs = modules[rxInput].Inputs;
List<int> cycles = [];
foreach (var theirInput in theirInputs)
{
    cycles.Add(FindCycle(theirInput));
}
int lcm = cycles.Aggregate((a, b) => LCM(a, b));
Console.WriteLine(lcm);

int LCM(int a, int b)
{
    return a * b / GCD(a, b);
}

int GCD(int a, int b)
{
    while (b != 0)
    {
        int temp = b;
        b = a % b;
        a = temp;
    }
    return a;
}

int FindCycle(string moduleName)
{
    int cycleCount = 0;
    for (int i = 1; i <= 10000000; i++)
    {
        PressButton();
        if (modules[moduleName].IsInInitialState())
        {
            cycleCount = i;
            if (cycleCount != 1)
            {
                break;
            }
        }
    }
    return cycleCount;
}


(int lowPulseCount, int highPulseCount) PressButton()
{
    Queue<(string, string, PulseType)> nextSteps = new();
    nextSteps.Enqueue(("button", "broadcaster", PulseType.Low));

    int lowPulseCount = 1, highPulseCount = 0;
    while (nextSteps.Count > 0)
    {
        var (source, destination, pulseType) = nextSteps.Dequeue();
        if (!modules.ContainsKey(destination))
        {
            continue;
        }
        var nextStepsFromModule = modules[destination].ActOnPulse(modules, source, pulseType);
        foreach (var (nextModule, nextPulseType) in nextStepsFromModule)
        {
            // Console.WriteLine($"{destination} -> {nextModule} ({pulseType} -> {nextPulseType})");
            if (nextPulseType == PulseType.Low)
            {
                lowPulseCount++;
            }
            else
            {
                highPulseCount++;
            }
            nextSteps.Enqueue((destination, nextModule, nextPulseType));
        }
    }

    return (lowPulseCount, highPulseCount);
}

Dictionary<string, Module> ParseModules(string[] lines)
{
    Dictionary<string, Module> modules = new();
    foreach (var line in lines)
    {
        var parts = line.Split("->");
        var moduleName = parts[0].Trim();
        var destinationList = parts[1].Split(",").Select(s => s.Trim()).ToList();

        if (moduleName == "broadcaster")
        {
            modules.Add(moduleName, new BroadcastModule(moduleName, destinationList));
        }
        else if (moduleName.StartsWith("%"))
        {
            moduleName = moduleName[1..];
            modules.Add(moduleName, new FlipFlopModule(moduleName, destinationList));
        }
        else if (moduleName.StartsWith("&"))
        {
            moduleName = moduleName[1..];
            modules.Add(moduleName, new ConjunctionModule(moduleName, destinationList));
        }
    }

    foreach (var module in modules.Values)
    {
        if (module is ConjunctionModule conjunctionModule)
        {
            conjunctionModule.SetInputs(modules);
        }
    }
    return modules;
}

enum PulseType { Low, High }
enum State { Off, On }

abstract class Module(string name, List<string> destinationModules)
{
    public string Name { get; init; } = name;
    public List<string> Inputs { get; init; } = [];

    public List<string> DestinationModules { get; init; } = destinationModules;
    public abstract List<(string, PulseType)> ActOnPulse(Dictionary<string, Module> modules, string sourceModule, PulseType pulseType);
    public abstract bool IsInInitialState();

    public virtual void SetInputs(Dictionary<string, Module> modules)
    {
        foreach (var module in modules.Values)
        {
            if (module.DestinationModules.Contains(Name))
            {
                Inputs.Add(module.Name);
            }
        }
    }
}

class BroadcastModule(string name, List<string> destinationModules) : Module(name, destinationModules)
{
    public override List<(string, PulseType)> ActOnPulse(Dictionary<string, Module> modules, string sourceModule, PulseType pulseType)
    {
        return DestinationModules.Select(d => (d, pulseType)).ToList();
    }

    public override bool IsInInitialState() => true;
}

class FlipFlopModule(string name, List<string> destinationModules) : Module(name, destinationModules)
{
    private State state = State.Off;

    public override List<(string, PulseType)> ActOnPulse(Dictionary<string, Module> modules, string sourceModule, PulseType pulseType)
    {
        if (pulseType == PulseType.High)
        {
            return [];
        }
        var pulseTypeToSend = state == State.On ? PulseType.Low : PulseType.High;
        state = state == State.On ? State.Off : State.On;
        return DestinationModules.Select(d => (d, pulseTypeToSend)).ToList();
    }

    public override bool IsInInitialState() => state == State.Off;
}

class ConjunctionModule(string name, List<string> destinationModules) : Module(name, destinationModules)
{
    private readonly Dictionary<string, PulseType> memoryOfInputs = [];

    public override List<(string, PulseType)> ActOnPulse(Dictionary<string, Module> modules, string sourceModule, PulseType pulseType)
    {
        memoryOfInputs[sourceModule] = pulseType;

        PulseType pulseTypeToSend = memoryOfInputs.All(kvp => kvp.Value == PulseType.High) ? PulseType.Low : PulseType.High;
        return DestinationModules.Select(d => (d, pulseTypeToSend)).ToList();
    }

    public override bool IsInInitialState() => memoryOfInputs.Any(kvp => kvp.Value == PulseType.Low);

    public override void SetInputs(Dictionary<string, Module> modules)
    {
        base.SetInputs(modules);

        foreach (var input in Inputs)
        {
            memoryOfInputs.Add(input, PulseType.Low);
        }
    }
}

class OutputModule(string name) : Module(name, [])
{
    public override List<(string, PulseType)> ActOnPulse(Dictionary<string, Module> modules, string sourceModule, PulseType pulseType) => [];
    public override bool IsInInitialState() => true;
}