using System;

namespace Monopoly
{
    public class Game : IGame
    {
        private IProperty[] properties;
        public IBoard GameBoard { get; }
        public IPlayerDeque Players { get; set; } = null;

        public Game(IBoard gameBoard)
        {
            GameBoard = gameBoard;
            properties = gameBoard.Properties;
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
                Players.TakeTurn();
                PrintPlayerPosition();
            }
        }

        private void PrintPlayerPosition()
        {
            IPlayer player = Players.PreviousPlayer;
            Console.WriteLine(
                "Player '{0}' moved to Property '{1}'",
                player.Name,
                properties[player.Position].Name);
        }
    }
}
