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
            get { return _playerTurnIndex; }
            set { _playerTurnIndex = value % PlayerCount; }
        }

        public int CompletedRounds { get; set; }

        public Monopoly(string[] playerNames)
        {
            _gameBoard = new Board();
            InitializePlayers(playerNames);
        }

        private void InitializePlayers(string[] playerNames)
        {
            _players = new List<Player>();
            foreach (var name in playerNames)
            {
                _players.Add(new Player(name));
            }

            ShufflePlayers();
            GetPlayer(0).TurnEnded += OnTurnEnded;
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

        public Property GetProperty(int index)
        {
            return _gameBoard.GetProperty(index);
        }

        public void OnTurnEnded(Player sender)
        {
            sender.TurnEnded -= OnTurnEnded;
            PlayerTurnIndex += 1;
            GetPlayer(PlayerTurnIndex).TurnEnded += OnTurnEnded;

            if (PlayerTurnIndex == 0)
            {
                CompletedRounds++;
            }
        }

        public static int Main(string[] args)
        {
            return 0;
        }
    }
}
