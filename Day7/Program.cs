var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);

SortedDictionary<Hand, int> handBidMap = [];
foreach (var line in lines)
{
    var parts = line.Split(" ");
    var hand = new Hand(parts[0], jokerDeck: true);
    var bid = int.Parse(parts[1]);
    handBidMap.Add(hand, bid);
}

var total = 0;
for (int i = 0; i < handBidMap.Count; i++)
{
    Console.WriteLine($"{handBidMap.ElementAt(i).Key} {handBidMap.ElementAt(i).Value} Rank: {i + 1}");
    total += handBidMap.ElementAt(i).Value * (i + 1);
}
Console.WriteLine(total);

enum HandType
{
    HighCard,
    Pair,
    TwoPair,
    ThreeOfAKind,
    FullHouse,
    FourOfAKind,
    FiveOfAKind,
}

record Card(char Value, bool JokerDeck = false) : IComparable<Card>
{
    private static readonly List<char> Order = ['2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A'];
    private static readonly List<char> JokerDeckOrder = ['J', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'Q', 'K', 'A'];

    public int CompareTo(Card? other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        if (JokerDeck)
        {
            return JokerDeckOrder.IndexOf(Value).CompareTo(JokerDeckOrder.IndexOf(other.Value));
        }

        return Order.IndexOf(Value).CompareTo(Order.IndexOf(other.Value));
    }
}

record Hand : IComparable<Hand>
{
    public Hand(string cards, bool jokerDeck = false)
    {
        this.JokerDeck = jokerDeck;
        this.Cards = cards.Select(c => new Card(c, jokerDeck)).ToArray();
        Type = ComputeType();

    }

    private HandType ComputeType()
    {
        var groups = Cards.GroupBy(c => c).Where(g => JokerDeck ? g.Key.Value != 'J' : true).Select(g => g.Count()).OrderByDescending(c => c).ToList();
        var numJokers = JokerDeck ? Cards.Count(c => c.Value == 'J') : 0;

        if (groups.Count == 0 && numJokers == 5)
        {
            return HandType.FiveOfAKind;
        }
        else if (groups[0] + numJokers == 5)
        {
            return HandType.FiveOfAKind;
        }
        else if (groups[0] + numJokers == 4)
        {
            return HandType.FourOfAKind;
        }
        else if ((groups[0] == 3 && groups[1] == 2) || (groups[0] == 2 && groups[1] == 2 && numJokers == 1))
        {
            return HandType.FullHouse;
        }
        else if (groups[0] + numJokers == 3)
        {
            return HandType.ThreeOfAKind;
        }
        else if ((groups[0] == 2 && groups[1] == 2) || (groups[0] == 2 && numJokers == 1))
        {
            return HandType.TwoPair;
        }
        else if (groups[0] + numJokers == 2)
        {
            return HandType.Pair;
        }
        else
        {
            return HandType.HighCard;
        }
    }

    public int CompareTo(Hand? other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        if (Type == other.Type)
        {
            for (int i = 0; i < Cards.Length; i++)
            {
                if (Cards[i] != other.Cards[i])
                {
                    return Cards[i].CompareTo(other.Cards[i]);
                }
            }
            return 0;
        }
        else
        {
            return Type.CompareTo(other.Type);
        }
    }

    public override string ToString()
    {
        return $"{Type} {string.Join("", Cards.Select(c => c.Value))}";
    }

    public bool JokerDeck { get; }
    public Card[] Cards { get; }
    public HandType Type { get; init; }
}