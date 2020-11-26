using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandsList
{
    public class DecksCommand : GenericCommand
    {
        public DecksCommand()
        {
            Keyword = "decks";
            Description = "Shows content and order of damage decks of all players";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            if (!DamageDecks.Initialized)
            {
                Console.Write("\nDamage decks are not initalized yet!", true, "red");
            }
            else
            {
                ShowDecks();
            }
        }

        private void ShowDecks()
        {
            Console.Write("\nPlayer1 Deck:", true, "green");
            foreach (var card in DamageDecks.GetDamageDeck(Players.PlayerNo.Player1).Deck)
            {
                Console.Write(card.Name, false, "green");
            }

            Console.Write("\nPlayer2 Deck:", true, "green");
            foreach (var card in DamageDecks.GetDamageDeck(Players.PlayerNo.Player2).Deck)
            {
                Console.Write(card.Name, false, "green");
            }
        }
    }
}
