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
                { "Regular Property Action", PerformRegularPropertyAction },
                { "Utility Action", PerformUtilityAction },
                { "RailRoad Action", PerformRailRoadAction }
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

        public void TakeTurn(IPlayer player)
        {
            BeginPlayerTurn(player);
            AdvancePlayer(player);
            EndPlayerTurn(player);
            Players.AdvanceDeque();
        }

        public void BeginPlayerTurn(IPlayer player)
        {
            if (player.Money < 100)
            {
                var properties = GameBoard.GetAllPropertiesOwnedByPlayer(player);
                foreach (var property in properties)
                {
                    if (!property.Mortgaged)
                    {
                        MortgageProperty(property, player);
                        return;
                    }
                }
            }
        }

        public void EndPlayerTurn(IPlayer player)
        {
            if (player.Money > 200)
            {
                var properties = GameBoard.GetAllPropertiesOwnedByPlayer(player);
                foreach (var property in properties)
                {
                    if (property.Mortgaged && player.Money > 200)
                    {
                        UnMortgageProperty(property, player);
                    }
                }
            }
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
            BuyOrPayRent(player, currentProperty, PayRent);
        }

        private void PerformUtilityAction(IPlayer player, IProperty currentProperty)
        {
            BuyOrPayRent(player, currentProperty, PayUtility);
        }

        private void PerformRailRoadAction(IPlayer player, IProperty currentProperty)
        {
            BuyOrPayRent(player, currentProperty, PayRailRoadToll);
        }

        private void BuyOrPayRent(IPlayer player, IProperty currentProperty, Action<IPlayer, IProperty> rentFunction)
        {
            if (!currentProperty.IsOwned())
            {
                BuyProperty(player, currentProperty);
            }
            else if (currentProperty.Owner != player)
            {
                rentFunction(player, currentProperty);
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

        private void PayRentAmmount(IPlayer toPlayer, IPlayer fromPlayer, int ammount)
        {
            toPlayer.Money += ammount;
            fromPlayer.Money -= ammount;
            Console.WriteLine(
               "{0} paid ${1} to {2}",
               fromPlayer.Name,
               ammount,
               toPlayer.Name);
        }

        private void PayRent(IPlayer player, IProperty currentProperty)
        {
            PayRentAmmount(currentProperty.Owner, player, GameBoard.CalculateRent(currentProperty));
        }

        private void PayUtility(IPlayer player, IProperty currentProperty)
        {
            var playerOwedMoney = currentProperty.Owner;
            var group = GameBoard.GetGroupFromProperty(currentProperty);
            int ownedProperties = group.GetNumberOfPropertiesInGroupOwnedByPlayer(playerOwedMoney);
            int modifier = (ownedProperties == 1) ? 4 : 10;
            PayRentAmmount(playerOwedMoney, player, modifier * player.LastDiceRoll);
        }

        private void PayRailRoadToll(IPlayer player, IProperty currentProperty)
        {
            var playerOwedMoney = currentProperty.Owner;
            var group = GameBoard.GetGroupFromProperty(currentProperty);
            int ownedProperties = group.GetNumberOfPropertiesInGroupOwnedByPlayer(playerOwedMoney);
            int rentAmmount = 25;
            for (var i = 1; i < ownedProperties; i++)
            {
                rentAmmount *= 2;
            }

            PayRentAmmount(playerOwedMoney, player, rentAmmount);
        }

        private void MortgageProperty(IProperty property, IPlayer player)
        {
            if (!property.Mortgaged)
            {
                property.Mortgaged = true;
                player.Money += (int)(property.Price * 0.75);
                Console.WriteLine(
                    "{0} mortgaged {1}, net worth is now ${2}",
                    player.Name,
                    property.Name,
                    player.Money);
            }
        }

        private void UnMortgageProperty(IProperty property, IPlayer player)
        {
            if (property.Mortgaged && player.Money > property.Price)
            {
                property.Mortgaged = false;
                player.Money -= property.Price;
                Console.WriteLine(
                    "{0} unmortgaged {1}, net worth is now ${2}",
                    player.Name,
                    property.Name,
                    player.Money);
            }
        }
    }
}
