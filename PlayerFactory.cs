using System.Linq;

namespace Monopoly
{
    public class PlayerFactory : IPlayerFactory
    {
        private string[] _playerNames;
        private IRandomGenerator _generator;
        private IBoard _board;

        public PlayerFactory(string[] names, IRandomGenerator generator, IBoard board)
        {
            _playerNames = names;
            _generator = generator;
            _board = board;
        }

        public IPlayer[] GeneratePlayers()
        {
            return _playerNames.Select(player => new Player(_generator, _board, player)).Cast<IPlayer>().ToArray();
        }
    }
}