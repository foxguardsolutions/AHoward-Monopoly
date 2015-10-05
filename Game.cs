using System;
using System.Collections.Generic;
using System.Linq;

namespace Monopoly
{
    public class Game : IGame
    {
        private IProperty[] properties;
        private Dictionary<IProperty, Action<IPlayer, IProperty>> actionProperties;
        public IBoard GameBoard { get; }
        public IPlayerDeque Players { get; set; } = null;

        public Game(IBoard gameBoard)
        {
            GameBoard = gameBoard;
            properties = gameBoard.Properties;
            actionProperties = new Dictionary<IProperty, Action<IPlayer, IProperty>>()
            {
                { GameBoard.GetPropertyFromName("Go To Jail"), SendPlayerToJail },
                { GameBoard.GetPropertyFromName("Income Tax"), CollectIncomeTax },
                { GameBoard.GetPropertyFromName("Luxury Tax"), CollectLuxuryTax },
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
            if (actionProperties.ContainsKey(currentProperty))
            {
                actionProperties[currentProperty](player, currentProperty);
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
            player.Money -= Math.Min(player.Money * 0.20, 200);
            Console.WriteLine("Income Tax Collected, '{0}' net worth is now ${1}", player.Name, player.Money);
        }

        private void CollectLuxuryTax(IPlayer player, IProperty currentProperty)
        {
            player.Money -= 75;
            Console.WriteLine("Luxary tax Collected, {0} net worth is now ${1}", player.Name, player.Money);
        }
    }
}
