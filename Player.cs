using System;

namespace Monopoly
{
    public class Player : IPlayer
    {
        private int _position;
        private const int _minDieRoll = 1, _maxDieRoll = 6;
        public string Name { get; }

        public IBoard GameBoard { get; }
        public IRandomGenerator Generator { get; }
        public int Position { get; set; }

        public int Money { get; set; } = 0;

        public int LastDiceRoll { get; set; } = 0;

        public int ConsecutiveDoublesRolled { get; set; }
        public int ConsecutiveTurnsInJail { get; set; }

        public Player(IRandomGenerator generator, string name = "")
        {
            Generator = generator;
            Name = name;
            Position = 0;
        }

        public int RollDie()
        {
            return Generator.Next(_minDieRoll, _maxDieRoll);
        }

        public int RollBothDice()
        {
            int dieRoll1 = RollDie();
            int dieRoll2 = RollDie();
            LastDiceRoll = dieRoll1 + dieRoll2;
            if (dieRoll1 == dieRoll2)
            {
                ConsecutiveDoublesRolled++;
                Console.WriteLine("{0} rolled doubles!", Name);
            }
            else
            {
                ConsecutiveDoublesRolled = 0;
            }
            return LastDiceRoll;
        }
    }
}