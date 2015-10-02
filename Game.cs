using System;
using System.Collections.Generic;

namespace Monopoly
{
    public class Game : IGame
    {
        private Dictionary<string, Action<IPlayer, IProperty>> actionProperties;
        public IBoard GameBoard { get; }
        public IPlayerDeque Players { get; set; } = null;

        public Game(IBoard gameBoard)
        {
            GameBoard = gameBoard;
            actionProperties = new Dictionary<string, Action<IPlayer, IProperty>>()
            {
                { "Send Player To Jail", SendPlayerToJail },
                { "Pay Income Tax", CollectIncomeTax },
                { "Pay Luxury Tax", CollectLuxuryTax },
                { "Regular Property Action", PerformRegularPropertyAction }
            };
        }

        public void Play()
        {
            int roundsPlayed = 0;
            while (roundsPlayed < 20)
            {
                PlayRound();
                roundsPlayed++;
            }
        }

        private void PlayRound()
        {
            for (int i = 0; i < Players.Count; i++)
            {
                TakeTurn(Players.CurrentPlayer);
            }
        }

        private void TakeTurn(IPlayer player)
        {
            AdvancePlayer(player);
            Players.AdvanceDeque();
        }

        public void AdvancePlayer(IPlayer player)
        {
            int diceRoll = player.RollBothDice();
            player.Position += diceRoll;
            IProperty playerPosition = GameBoard.GetPropertyFromIndex(player.Position);

            // Player passed Go
            if (player.Position < diceRoll)
            {
                player.Money += 200;
                Console.WriteLine("{0} passed Go, net worth is now ${1}", player.Name, player.Money);
            }

            PerformActions(player, playerPosition);
        }

        private void PerformActions(IPlayer player, IProperty currentProperty)
        {
            PrintPlayerPosition(player, currentProperty);
            var action = currentProperty.Action ?? string.Empty;
            if (actionProperties.ContainsKey(action))
            {
                actionProperties[action](player, currentProperty);
            }
        }

        private void PrintPlayerPosition(IPlayer player, IProperty currentProperty)
        {
            Console.WriteLine(
                "Player '{0}' moved to Property '{1}'",
                player.Name,
                currentProperty.Name);
        }

        private void SendPlayerToJail(IPlayer player, IProperty currentProperty)
        {
            Console.WriteLine("Sending player '{0}' to Jail!", player.Name);
            player.Position = GameBoard.GetPropertyPositionFromName("Jail");
        }

        private void CollectIncomeTax(IPlayer player, IProperty currentProperty)
        {
            player.Money -= Math.Min((int)(player.Money * 0.20), 200);
            Console.WriteLine("Income Tax Collected, '{0}' net worth is now ${1}", player.Name, player.Money);
        }

        private void CollectLuxuryTax(IPlayer player, IProperty currentProperty)
        {
            player.Money -= 75;
            Console.WriteLine("Luxary tax Collected, {0} net worth is now ${1}", player.Name, player.Money);
        }

        private void PerformRegularPropertyAction(IPlayer player, IProperty currentProperty)
        {
            if (!currentProperty.IsOwned())
            {
                BuyProperty(player, currentProperty);
            }
            else if (currentProperty.Owner != player)
            {
                PayRent(player, currentProperty);
            }
        }

        private void BuyProperty(IPlayer player, IProperty currentProperty)
        {
            // This is not a requirement, but a good life decision
            if (player.Money - currentProperty.Price > 100)
            {
                player.Money -= currentProperty.Price;
                GameBoard.PlayerPurchasedProperty(player, currentProperty);
                Console.WriteLine(
                    "'{0}' bought '{1}' for ${2}",
                    player.Name,
                    currentProperty.Name,
                    currentProperty.Price);
            }
        }

        private void PayRent(IPlayer player, IProperty currentProperty)
        {
            int rentAmmount = GameBoard.CalculateRent(currentProperty);
            currentProperty.Owner.Money += rentAmmount;
            player.Money -= rentAmmount;
            Console.WriteLine(
                "{0} paid ${1} to {2}",
                player.Name,
                rentAmmount,
                currentProperty.Owner.Name);
        }
    }
}
