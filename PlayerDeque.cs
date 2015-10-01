using System.Collections.Generic;
using System.Linq;

namespace Monopoly
{
    public class PlayerDeque : List<IPlayer>, IPlayerDeque
    {
        private readonly IRandomGenerator _generator;

        public IPlayer PreviousPlayer
        {
            get { return this.Last(); }
        }

        public PlayerDeque(IRandomGenerator generator, IPlayerFactory playerFactory) : base(playerFactory.GeneratePlayers())
        {
            _generator = generator;
            Shuffle();
        }

        public void Shuffle()
        {
            for (int i = 0; i < Count * 2; i++)
            {
                IPlayer temp = this[_generator.Next(0, Count - 1)];
                Remove(temp);
                Add(temp);
            }
        }

        public void AdvanceDeque()
        {
            if (Count > 0)
            {
                IPlayer player = this[0];
                Remove(player);
                Add(player);
            }
        }

        public void TakeTurn()
        {
            if (Count > 0)
            {
                this[0].TakeTurn();
                AdvanceDeque();
            }
        }
    }
}
