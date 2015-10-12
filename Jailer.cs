using System;
using System.Collections.Generic;

namespace Monopoly
{
    public class Jailer : IJailer
    {
        private IBoard _gameBoard;
        private List<IPlayer> _jailedPlayers;
        private Dictionary<IPlayer, List<Card>> _getOutOfJailCards;  
        private const int _maxRoundsPlayerMayStayInJail = 3;
        private const int _moneyPaidToGetOutOfJail = 50;
        private const int _consecutiveDoublesThatAreLegal = 2;
        public ICardDealer Dealer { get; set; }

        public Jailer(IBoard gameBoard)
        {
            _gameBoard = gameBoard;
            _jailedPlayers = new List<IPlayer>();
            _getOutOfJailCards = new Dictionary<IPlayer, List<Card>>();
        }

        public void Process(IPlayer player)
        {
            if (player.ConsecutiveDoublesRolled > _consecutiveDoublesThatAreLegal)
            {
                SendPlayerToJail(player);
            }
            else if (IsInJail(player))
            {
                AttemptGettingOutOfJail(player);
            }
            else if (_gameBoard.GetPropertyFromIndex(player.Position).Name == "Go To Jail")
            {
                SendPlayerToJail(player);
            }
        }

        public void SendPlayerToJail(IPlayer player)
        {
            var property = _gameBoard.GetPropertyFromName("Jail");
            _gameBoard.MovePlayerToProperty(player, property, false);
            _jailedPlayers.Add(player);
            Console.WriteLine("{0} was sent to jail!", player.Name);
        }

        public void GivePlayerGetOutJailFreeCard(IPlayer player, Card card)
        {
            if (!_getOutOfJailCards.ContainsKey(player))
            {
                _getOutOfJailCards.Add(player, new List<Card>());
            }

            _getOutOfJailCards[player].Add(card);
            Console.WriteLine("{0} acquired a get out of jail free card!", player.Name);
        }

        private void ReleasePlayerFromJail(IPlayer player)
        {
            _jailedPlayers.Remove(player);
            Console.WriteLine("{0} released from jail!", player.Name);
        }

        public bool IsInJail(IPlayer player)
        {
            return _jailedPlayers.Contains(player);
        }

        private void AttemptGettingOutOfJail(IPlayer player)
        {
            if (player.ConsecutiveDoublesRolled > 0)
            {
                ReleasePlayerFromJail(player);
            }
            else if (_getOutOfJailCards.ContainsKey(player) && _getOutOfJailCards[player].Count > 0)
            {
                var card = _getOutOfJailCards[player][0];
                Dealer.PutCardBackInDeck(card);
                _getOutOfJailCards[player].Remove(card);
                ReleasePlayerFromJail(player);
            }
            else if (player.ConsecutiveTurnsInJail >= _maxRoundsPlayerMayStayInJail || player.Money > 1000)
            {
                player.Money -= _moneyPaidToGetOutOfJail;
                ReleasePlayerFromJail(player);
            }
            else
            {
                player.ConsecutiveTurnsInJail++;
            }
        }
    }
}
