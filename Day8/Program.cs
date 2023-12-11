var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);

var instructions = lines[0];

Dictionary<string, Node> nodes = [];

foreach (var line in lines[1..])
{
    var parts = line.Split(" = ");
    var label = parts[0];
    var nodeParts = parts[1].Replace("(", "").Replace(")", "").Split(", ");
    var node = new Node(nodeParts[0], nodeParts[1]);
    nodes.Add(label, node);
}

long GetNumberOfSteps(string instructions, Dictionary<string, Node> nodes, string startingNode = "AAA", bool endWithZ = false)
{
    long counter = 0;
    var currentNode = startingNode;
    while (endWithZ ? !currentNode.EndsWith('Z') : currentNode != "ZZZ")
    {
        var node = nodes[currentNode];
        var nextTurn = instructions[(int)(counter % instructions.Length)];
        currentNode = nextTurn == 'L' ? node.Left : node.Right;
        counter++;
    }

    Console.WriteLine($"Number of steps: {counter}");
    return counter;
}

// Part1
GetNumberOfSteps(instructions, nodes);


// Part2
var startingNodes = nodes.Keys.Where(k => k.EndsWith('A')).ToArray();
var steps = startingNodes.Select(n => GetNumberOfSteps(instructions, nodes, n, true)).ToArray();
// Find least common multiple of the steps for each individual starting node.
Console.WriteLine($"Number of steps: {LCM(steps)}");

long LCM(long[] numbers)
{
    long lcm = numbers[0];

    for (long i = 1; i < numbers.Length; i++)
    {
        long gcd = GCD(lcm, numbers[i]);
        lcm = Math.Abs(lcm * numbers[i]) / gcd;
    }

    return lcm;
}

long GCD(long a, long b)
{
    while (b != 0)
    {
        long temp = b;
        b = a % b;
        a = temp;
    }

    return a;
}

record Node(string Left, string Right);