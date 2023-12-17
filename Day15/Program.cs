var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var words = input.Split(",", StringSplitOptions.RemoveEmptyEntries);

int sum = 0;
foreach (var word in words)
{
    sum += Hash(word);
}

Console.WriteLine(sum);

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