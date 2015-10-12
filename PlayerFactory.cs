using System.Linq;

namespace Monopoly
{
    public class PlayerFactory : IPlayerFactory
    {
        private readonly string[] _playerNames;
        private readonly IRandomGenerator _generator;

        public PlayerFactory(string[] names, IRandomGenerator generator)
        {
            _playerNames = names;
            _generator = generator;
        }

        public IPlayer[] GeneratePlayers()
        {
            return _playerNames.Select(player => new Player(_generator, player)).Cast<IPlayer>().ToArray();
        }
    }
}