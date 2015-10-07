using System;
using System.Collections.Generic;
using System.Linq;

namespace Monopoly
{
    public class Game : IGame
    {
        private CardDeck _communityChest, _chance;
        private readonly Dictionary<string, Action<IPlayer, IProperty, int>> actionProperties;
        private readonly Dictionary<string, Action<IPlayer, Card>> actionCards; 
        private const int _moneyPaidForPassingGo = 200;
        private const int _moneyPaidToGetOutOfJail = 50;
        private const int _consecutiveDoublesRolledToGoToJail = 3;
        private const int _roundsToPlay = 20;
        private const int _maxRoundsPlayerMayStayInJail = 3;
        private const int _minMoneyPlayerMustHaveToUnmortgageProperties = 200;
        private const int _maxMoneyPlayerMustHaveToMortgageProperties = 100;
        private const int _minMoneyPlayerMustHaveToBuyProperty = 100;
        private const double _percentagePaidForMortgagingProperty = 0.75;
        private const double _percentagePaidForUnmortgagingProperty = 1.00;
        private const double _percentagePaidForIncomeTax = 0.20;
        private const int _maxIncomeTaxPaid = 200;
        private const int _luxuryTaxPaid = 75;
        public IBoard GameBoard { get; }
        public IQueue Players { get; set; } = null;

        public CardDeck CommunityChest { set { _communityChest = value; } }
        public CardDeck Chance { set { _chance = value; } }

        public Game(IBoard gameBoard, CardDeck communityChest, CardDeck chance)
        {
            GameBoard = gameBoard;
            actionProperties = new Dictionary<string, Action<IPlayer, IProperty, int>>()
            {
                { "Send Player To Jail", SendPlayerToJail },
                { "Pay Income Tax", CollectIncomeTax },
                { "Pay Luxury Tax", CollectLuxuryTax },
                { "Regular Property Action", PerformRegularPropertyAction },
                { "Utility Action", PerformUtilityAction },
                { "RailRoad Action", PerformRailRoadAction },
                { "Collect Chance", DrawChance },
                { "Collect Community Chest", DrawCommunityChest }
            };

            actionCards = new Dictionary<string, Action<IPlayer, Card>>()
            {
                { "Move To Property", MovePlayerToPropertyIndicatedByCard },
                { "Move To Nearest Property", MovePlayerToClosestProperty },
                { "Bank pays player", BankPaysPlayer },
                { "Player pays bank", PlayerPaysBank },
                { "Send Player To Jail", SendPlayerToJail },
                { "Pay other players", PlayerPaysAllOtherPlayers },
                { "Collect from other players", OtherPlayersPayPlayer },
                { "Move Player Back Spaces", MovePlayerBackSpaces },
                { "Player gets Get out of Jail Free card", AwardGetOutOfJailFreeCard }
            };

            _communityChest = communityChest;
            _chance = chance;
        }

        // Just to make Ninject happy
        public Game(IBoard gameBoard) : this(gameBoard, null, null)
        {
        }

        public void Play()
        {
            int roundsPlayed = 0;
            while (roundsPlayed < _roundsToPlay)
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
            if (!player.IsInJail)
            {
                AdvancePlayer(player);
                EndPlayerTurn(player);
            }
            
            Players.AdvanceDeque();
        }

        public void BeginPlayerTurn(IPlayer player)
        {
            player.RollBothDice();
            if (player.IsInJail)
            {
                TryGettingOutOfJail(player);
            }
            else
            {
                if (player.ConsecutiveDoublesRolled == _consecutiveDoublesRolledToGoToJail)
                {
                    SendPlayerToJail(player, GameBoard.GetPropertyFromIndex(player.Position));
                }
            }

            if (!player.IsInJail)
            {
                TryMortgagingProperties(player);
            }
        }

        public void EndPlayerTurn(IPlayer player)
        {
            if (player.Money > _minMoneyPlayerMustHaveToUnmortgageProperties)
            {
                var properties = GameBoard.GetAllPropertiesOwnedByPlayer(player);
                foreach (var property in properties)
                {
                    if (property.Mortgaged && player.Money > _minMoneyPlayerMustHaveToUnmortgageProperties)
                    {
                        UnMortgageProperty(property, player);
                    }
                }
            }
        }

        public void AdvancePlayer(IPlayer player)
        {
            int lastPosition = player.Position;
            player.Position += player.LastDiceRoll;
            CheckPlayerPassedGo(player, lastPosition);

            IProperty playerPosition = GameBoard.GetPropertyFromIndex(player.Position);
            PerformActions(player, playerPosition);
        }

        private void CheckPlayerPassedGo(IPlayer player, int lastPosition)
        {
            if (lastPosition > player.Position)
            {
                player.Money += _moneyPaidForPassingGo;
                Console.WriteLine("{0} passed Go, net worth is now ${1}", player.Name, player.Money);
            }
        }

        private void PerformActions(IPlayer player, IProperty currentProperty, int rentModifier = 1)
        {
            PrintPlayerPosition(player, currentProperty);
            var action = currentProperty.Action ?? string.Empty;
            if (actionProperties.ContainsKey(action))
            {
                actionProperties[action](player, currentProperty, rentModifier);
            }
        }

        private void PrintPlayerPosition(IPlayer player, IProperty currentProperty)
        {
            Console.WriteLine(
                "Player '{0}' moved to Property '{1}'",
                player.Name,
                currentProperty.Name);
        }

        private void SendPlayerToJail(IPlayer player, IProperty currentProperty, int rentModifier = 1)
        {
            Console.WriteLine("Sending player '{0}' to Jail!", player.Name);
            player.Position = GameBoard.GetPropertyPositionFromName("Jail");
            player.IsInJail = true;
        }

        private void SendPlayerToJail(IPlayer player, Card card)
        {
            var property = GameBoard.GetPropertyFromIndex(player.Position);
            SendPlayerToJail(player, property);
        }

        private void CollectIncomeTax(IPlayer player, IProperty currentProperty, int rentModifier = 1)
        {
            player.Money -= Math.Min((int)(player.Money * _percentagePaidForIncomeTax), _maxIncomeTaxPaid) * rentModifier;
            Console.WriteLine("Income Tax Collected, '{0}' net worth is now ${1}", player.Name, player.Money);
        }

        private void CollectLuxuryTax(IPlayer player, IProperty currentProperty, int rentModifier = 1)
        {
            player.Money -= _luxuryTaxPaid * rentModifier;
            Console.WriteLine("Luxary tax Collected, {0} net worth is now ${1}", player.Name, player.Money);
        }

        private void PerformRegularPropertyAction(IPlayer player, IProperty currentProperty, int rentModifier = 1)
        {
            BuyOrPayRent(player, currentProperty, PayRent, rentModifier);
        }

        private void PerformUtilityAction(IPlayer player, IProperty currentProperty, int rentModifier = 1)
        {
            BuyOrPayRent(player, currentProperty, PayUtility, rentModifier);
        }

        private void PerformRailRoadAction(IPlayer player, IProperty currentProperty, int rentModifier = 1)
        {
            BuyOrPayRent(player, currentProperty, PayRailRoadToll, rentModifier);
        }

        private void BuyOrPayRent(IPlayer player, IProperty currentProperty, Action<IPlayer, IProperty, int> rentFunction, int rentModifier = 1)
        {
            if (!currentProperty.IsOwned())
            {
                BuyProperty(player, currentProperty);
            }
            else if (currentProperty.Owner != player)
            {
                rentFunction(player, currentProperty, rentModifier);
            }
        }

        private void BuyProperty(IPlayer player, IProperty currentProperty)
        {
            // This is not a requirement, but a good life decision
            if (player.Money - currentProperty.Price > _minMoneyPlayerMustHaveToBuyProperty)
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

        private void PayRentAmount(IPlayer toPlayer, IPlayer fromPlayer, int Amount)
        {
            toPlayer.Money += Amount;
            fromPlayer.Money -= Amount;
            Console.WriteLine(
               "{0} paid ${1} to {2}",
               fromPlayer.Name,
               Amount,
               toPlayer.Name);
        }

        private void PayRent(IPlayer player, IProperty currentProperty, int rentModifier = 1)
        {
            PayRentAmount(currentProperty.Owner, player, GameBoard.CalculateRent(currentProperty) * rentModifier);
        }

        private void PayUtility(IPlayer player, IProperty currentProperty, int rentModifier = 1)
        {
            var playerOwedMoney = currentProperty.Owner;
            var group = GameBoard.GetGroupFromProperty(currentProperty);
            int ownedProperties = group.GetNumberOfPropertiesInGroupOwnedByPlayer(playerOwedMoney);
            int modifier = (rentModifier == 1) ? (ownedProperties == 1) ? 4 : 10 : rentModifier;
            PayRentAmount(playerOwedMoney, player, modifier * player.LastDiceRoll);
        }

        private void PayRailRoadToll(IPlayer player, IProperty currentProperty, int rentModifier = 1)
        {
            var playerOwedMoney = currentProperty.Owner;
            var group = GameBoard.GetGroupFromProperty(currentProperty);
            int ownedProperties = group.GetNumberOfPropertiesInGroupOwnedByPlayer(playerOwedMoney);
            int rentAmount = 25;
            for (var i = 1; i < ownedProperties; i++)
            {
                rentAmount *= 2;
            }

            PayRentAmount(playerOwedMoney, player, rentAmount * rentModifier);
        }

        private void TryMortgagingProperties(IPlayer player)
        {
            if (player.Money < _maxMoneyPlayerMustHaveToMortgageProperties)
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

        private void TryGettingOutOfJail(IPlayer player)
        {
            if (player.ConsecutiveDoublesRolled > 0)
            {
                player.ReleaseFromJail();
                Console.WriteLine("Player '{0}' released from jail!", player.Name);
            }
            else if (player.GetOutOfJailFreeCards.Count > 0)
            {
                UseGetOutOfJailFreeCard(player);
            }
            else if (player.ConsecutiveTurnsInJail == _maxRoundsPlayerMayStayInJail || player.Money >= 1000)
            {
                player.Money -= _moneyPaidToGetOutOfJail;
                player.ReleaseFromJail();
                Console.WriteLine("Player '{0}' bought out of jail!", player.Name);
            }
            else
            {
                player.ConsecutiveTurnsInJail++;
            }
        }

        private void MortgageProperty(IProperty property, IPlayer player)
        {
            if (!property.Mortgaged)
            {
                property.Mortgaged = true;
                player.Money += (int)(property.Price * _percentagePaidForMortgagingProperty);
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
                player.Money -= (int)(property.Price * _percentagePaidForUnmortgagingProperty);
                Console.WriteLine(
                    "{0} unmortgaged {1}, net worth is now ${2}",
                    player.Name,
                    property.Name,
                    player.Money);
            }
        }

        private void DrawCommunityChest(IPlayer player, IProperty property, int modifier = 0)
        {
            DrawCard(player, _communityChest);
        }

        private void DrawChance(IPlayer player, IProperty property, int modifier = 0)
        {
            DrawCard(player, _chance);
        }

        private void DrawCard(IPlayer player, CardDeck deck)
        {
            var card = deck.TopCard;
            if (actionCards.ContainsKey(card.Action))
            {
                actionCards[card.Action](player, card);
            }

            deck.AdvanceDeck();
        }

        private void MovePlayerToPropertyIndicatedByCard(IPlayer player, Card card)
        {
            var propertyIndex = GameBoard.GetPropertyPositionFromName(card.AssociatedProperty);
            var lastPosition = player.Position;
            player.Position = propertyIndex;

            CheckPlayerPassedGo(player, lastPosition);

            var property = GameBoard.GetPropertyFromIndex(player.Position);
            PerformActions(player, property, card.Modifier);
        }

        private IProperty FindClosestPropertyInGroupToPlayer(IPropertyGroup group, IPlayer player)
        {
            var property = group.Properties[0];
            foreach (var p in group.Properties)
            {
                property = (FindDistanceForward(player, p)) < FindDistanceForward(player, property) ? p : property;
            }

            return property;
        }

        private void MovePlayerToClosestProperty(IPlayer player, Card card)
        {
            var propertyInGroup = GameBoard.GetPropertyFromName(card.AssociatedProperty);
            var propertyGroup = GameBoard.GetGroupFromProperty(propertyInGroup);
            var property = FindClosestPropertyInGroupToPlayer(propertyGroup, player);

            var lastPosition = player.Position;
            player.Position = property.MapIndex;

            CheckPlayerPassedGo(player, lastPosition);

            PerformActions(player, property, card.Modifier);
        }

        private void MovePlayerBackSpaces(IPlayer player, Card card)
        {
            player.Position = (GameBoard.PropertyCount + player.Position - card.Amount) % GameBoard.PropertyCount;
            var property = GameBoard.GetPropertyFromIndex(player.Position);
            PerformActions(player, property, card.Modifier);
        }

        private void BankPaysPlayer(IPlayer player, Card card)
        {
            Console.WriteLine("Player {0} paid ${1} for drawing card: {2}", player.Name, card.Amount, card.Name);
            player.Money += card.Amount;
        }

        private void PlayerPaysBank(IPlayer player, Card card)
        {
            Console.WriteLine("Player {0} pays bank ${1} for drawing card: {2}", player.Name, card.Amount, card.Name);
            player.Money -= card.Amount;
        }

        private void PlayerPaysAllOtherPlayers(IPlayer player, Card card)
        {
            player.Money -= card.Amount * (Players.Count - 1);
            for (int i = 0; i < Players.Count; i++)
            {
                Players.AdvanceDeque();
                var p = Players.CurrentPlayer;
                if (p != player)
                {
                    p.Money += card.Amount;
                    Console.WriteLine("{0} paid ${1} to {2}", player.Name, card.Amount, p.Name);
                }
            }
        }

        private void OtherPlayersPayPlayer(IPlayer player, Card card)
        {
            player.Money += card.Amount * (Players.Count - 1);
            for (int i = 0; i < Players.Count; i++)
            {
                Players.AdvanceDeque();
                var p = Players.CurrentPlayer;
                if (p != player)
                {
                    p.Money -= card.Amount;
                    Console.WriteLine("{0} paid ${1} to {2}", p.Name, card.Amount, player.Name);
                }
            }
        }

        private int FindDistanceForward(IPlayer player, IProperty targetProperty)
        {
            return (targetProperty.MapIndex - player.Position + GameBoard.PropertyCount) % GameBoard.PropertyCount;
        }

        private void AwardGetOutOfJailFreeCard(IPlayer player, Card card)
        {
            player.GetOutOfJailFreeCards.Add(card);
            if (_chance.Contains(card))
            {
                _chance.RemoveCard(card);
            }
            else
            {
                _communityChest.RemoveCard(card);
            }
        }

        private void UseGetOutOfJailFreeCard(IPlayer player)
        {
            var card = player.UseGetOutOfJailFreeCard();
            if (_chance.ContainsCard(card))
            {
                _chance.AddCard(card);
            }
            else
            {
                _communityChest.AddCard(card);
            }
        }
    }
}
