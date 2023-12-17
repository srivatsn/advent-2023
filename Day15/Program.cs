var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var words = input.Split(",", StringSplitOptions.RemoveEmptyEntries);

// Part 1
int sum = 0;
foreach (var word in words)
{
    sum += Hash(word);
}

Console.WriteLine(sum);

// Part 2

var boxes = new Box[256];
boxes = boxes.Select(b => new Box()).ToArray();

foreach (var word in words)
{
    var label = word.Split(['=', '-'])[0];
    var hash = Hash(label);

    // Add the lens
    if (word.Contains('='))
    {
        var focalLength = int.Parse(word.Split('=')[1]);

        var lens = boxes[hash].Lenses.Find(l => l.Label == label);
        if (lens != null)
        {
            lens.FocalLength = focalLength;
        }
        else
        {
            boxes[hash].Lenses.Add(new Lens(label, focalLength));
        }
    }
    else
    {
        // Remove the lens
        var lens = boxes[hash].Lenses.Find(l => l.Label == label);
        if (lens != null)
        {
            boxes[hash].Lenses.Remove(lens);
        }
    }
}

int focusingPower = 0;
for (int i = 0; i < 256; i++)
{
    for (int j = 0; j < boxes[i].Lenses.Count; j++)
    {
        focusingPower += (i + 1) * (j + 1) * boxes[i].Lenses[j].FocalLength;
    }
}

Console.WriteLine(focusingPower);


static int Hash(string word)
{
    int hash = 0;
    foreach (var c in word)
    {
        hash += c;
        hash *= 17;
        hash %= 256;
    }
    return hash;
}

record Box(List<Lens> Lenses)
{
    public Box() : this([]) { }
}

record Lens
{
    public string Label { get; init; }
    public int FocalLength { get; set; }

    public Lens(string label, int focalLength)
    {
        Label = label;
        FocalLength = focalLength;
    }
}