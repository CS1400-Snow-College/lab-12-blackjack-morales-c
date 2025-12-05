// The game of Blackjack

class Program
{
    static Random rand = new Random();

    static void Main()
    {
        Console.Write("What is your name? ");
        string name = Console.ReadLine();

        int money = LoadMoney(name);

        Console.WriteLine($"Welcome {name}, you have ${money}.");

        while (true)
        {
            Console.Write("\nEnter your bet: ");
            int bet = int.Parse(Console.ReadLine());

            if (bet > money)
            {
                Console.WriteLine("Not enough money!");
                continue;
            }

            List<string> deck = CreateDeck();
            Shuffle(deck);

            List<string> player = new List<string>();
            List<string> dealer = new List<string>();

            player.Add(Draw(deck));
            dealer.Add(Draw(deck));
            player.Add(Draw(deck));
            dealer.Add(Draw(deck));

            Console.WriteLine("\nYour hand: " + string.Join(" ", player));
            Console.WriteLine("Dealer shows: " + dealer[0]);

            bool roundOver = false;

            while (!roundOver)
            {
                Console.Write("\n(H)it, (S)tand, (D)ouble: ");
                string choice = Console.ReadLine().ToUpper();

                if (choice == "H")
                {
                    player.Add(Draw(deck));
                    Console.WriteLine("You drew: " + string.Join(" ", player));

                    if (HandValue(player) > 21)
                    {
                        Console.WriteLine("Bust! You Lost!");
                        money -= bet;
                        roundOver = true;
                    }
                }
                else if (choice == "S")
                {
                    roundOver = true;
                }
                else if (choice == "D")
                {
                    if (money < bet * 2)
                    {
                        Console.WriteLine("Not enough money.");
                        continue;
                    }

                    bet *= 2;
                    player.Add(Draw(deck));
                    Console.WriteLine("You drew: " + string.Join(" ", player));

                    if (HandValue(player) > 21)
                    {
                        Console.WriteLine("Bust!");
                        money -= bet;
                        roundOver = true;
                    }

                    roundOver = true;
                }
            }

            if (HandValue(player) <= 21)
            {
                Console.WriteLine("\nDealer shows: " + string.Join(" ", dealer));

                while (HandValue(dealer) < 17)
                {
                    dealer.Add(Draw(deck));
                    Console.WriteLine("Dealer hits: " + string.Join(" ", dealer));
                }

                int p = HandValue(player);
                int d = HandValue(dealer);

                Console.WriteLine($"\nYour total: {p} ... Dealer: {d}");

                if (d > 21 || p > d)
                {
                    Console.WriteLine("You win!");
                    money += bet;
                }
                else if (p < d)
                {
                    Console.WriteLine("You lose!");
                    money -= bet;
                }
                else
                {
                    Console.WriteLine("Push");
                }
            }

            Console.WriteLine($"You now have ${money}.");

            Console.Write("Play again? yes or no: ");
            if (Console.ReadLine().ToLower() != "yes")
                break;
        }

        SaveMoney(name, money);
        Console.WriteLine("Game saved!");
    }

    // Creating the deck
    static List<string> CreateDeck()
    {
        string[] suits = { "♠", "♥", "♦", "♣" };
        string[] values = { "2","3","4","5","6","7","8","9","10","J","Q","K","A" };

        List<string> deck = new List<string>();
        foreach (var suit in suits)
            foreach (var value in values)
                deck.Add($"{value}{suit}");

        return deck;
    }

    // Shuffeling cards
    static void Shuffle(List<string> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int r = rand.Next(deck.Count);
            string temp = deck[i];
            deck[i] = deck[r];
            deck[r] = temp;
        }
    }

    // Drawing a card
    static string Draw(List<string> deck)
    {
        string card = deck[0];
        deck.RemoveAt(0);
        return card;
    }

    // Value of hand
    static int HandValue(List<string> hand)
    {
        int value = 0;
        int aces = 0;

        foreach (string card in hand)
        {
            string r = card.Substring(0, card.Length - 1);

            if (r == "A")
            {
                value += 11;
                aces++;
            }
            else if (r == "K" || r == "Q" || r == "J")
                value += 10;
            else
                value += int.Parse(r);
        }

        while (value > 21 && aces > 0)
        {
            value -= 10;
            aces--;
        }

        return value;
    }

    // Loading players money
    static int LoadMoney(string name)
    {
        if (!File.Exists("players.txt"))
            return 100;

        foreach (var line in File.ReadAllLines("players.txt"))
        {
            var parts = line.Split(',');
            if (parts[0] == name)
                return int.Parse(parts[1]);
        }

        return 100;
    }

    // Saving players money
    static void SaveMoney(string name, int money)
    {
        List<string> lines = new List<string>();

        if (File.Exists("players.txt"))
            lines.AddRange(File.ReadAllLines("players.txt"));

        bool found = false;

        for (int i = 0; i < lines.Count; i++)
        {
            var parts = lines[i].Split(',');
            if (parts[0] == name)
            {
                lines[i] = $"{name},{money}";
                found = true;
            }
        }

        if (!found)
            lines.Add($"{name},{money}");

        File.WriteAllLines("players.txt", lines);
    }
}
