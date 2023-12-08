using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);

int sumPoints = 0;
var cards = new Card[lines.Length];
var cardQueue = new Queue<Card>();

for (int i = 0; i < lines.Length; i++)
{
    var line = lines[i];
    var parts = line.Split(":");
    var cardNumber = parts[0].Replace("Card ", "");

    var numberParts = parts[1].Split("|");
    var winningNumbers = numberParts[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
    var drawnNumbers = numberParts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

    var card = new Card(int.Parse(cardNumber), winningNumbers, drawnNumbers);
    var count = card.Matches;
    var points = count == 0 ? 0 : (int)Math.Pow(2, count - 1);
    cards[i] = card;
    cardQueue.Enqueue(card);
    sumPoints += points;
}

Console.WriteLine(sumPoints);
int cardCount = 0;
while (cardQueue.Count > 0)
{
    var card = cardQueue.Dequeue();
    ProcessCard(card);
    cardCount++;
}
Console.WriteLine(cardCount);

void ProcessCard(Card card)
{
    var count = card.Matches;
    for (int i = card.number; i < cards.Length && i < card.number + count; i++)
    {
        cardQueue.Enqueue(cards[i]);
    }
}

record Card(int number, int[] winningNumbers, int[] drawnNumbers)
{
    public int Matches => drawnNumbers.Count(n => winningNumbers.Contains(n));
}