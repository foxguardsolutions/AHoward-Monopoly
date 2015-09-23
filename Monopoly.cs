using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monopoly
{
    public class Monopoly
    {
        private static Random _generator = new Random();

        private Board _gameBoard;
        private List<Player> _players;
        private int _playerTurnIndex = 0;

        public static Random Generator
        {
            get { return _generator; }
        }

        public int PlayerCount
        {
            get { return _players.Count; }
        }

        public int PlayerTurnIndex
        {
            get
            {
                return _playerTurnIndex;
            }
            set
            {
                _playerTurnIndex = value % PlayerCount;
                if (_playerTurnIndex == 0)
                {
                    CompletedRounds++;
                }
            }
        }

        public int CompletedRounds { get; set; }

        public Monopoly(string[] playerNames)
        {
            _gameBoard = new Board();
            InitializePlayers(playerNames);
        }

        ~Monopoly()
        {
            foreach (var player in GetAllPlayers())
            {
                player.TurnEnded -= OnTurnEnded;
            }
        }

        private void InitializePlayers(string[] playerNames)
        {
            _players = new List<Player>();
            foreach (var name in playerNames)
            {
                Player player = new Player(name);
                _players.Add(player);
                player.TurnEnded += OnTurnEnded;
            }

            ShufflePlayers();
        }

        private void ShufflePlayers()
        {
            for (var i = 0; i < 2 * PlayerCount; i++)
            {
                var holder = _players[_generator.Next(0, PlayerCount)];
                _players.Remove(holder);
                _players.Add(holder);
            }
        }

        public Player GetPlayer(int index)
        {
            return _players[index];
        }

        public IEnumerable<Player> GetAllPlayers()
        {
            return Enumerable.Range(0, PlayerCount).Select(GetPlayer);
        }

        public Property GetProperty(int index)
        {
            return _gameBoard.GetProperty(index);
        }

        public IEnumerable<Property> GetAllProperties()
        {
            return Enumerable.Range(0, Board.PropertyCount).Select(GetProperty);
        }

        public void OnTurnEnded(Player player)
        {
            PlayerTurnIndex++;
        }

        public void EmulatePlayerTurn()
        {
            GetPlayer(PlayerTurnIndex).TakeTurn();
        }
    }
}
