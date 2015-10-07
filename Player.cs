using System.Collections.Generic;

namespace Monopoly
{
    public class Player : IPlayer
    {
        private int _position;
        private const int _minDieRoll = 1, _maxDieRoll = 6;
        public string Name { get; }

        public IBoard GameBoard { get; }
        public IRandomGenerator Generator { get; }
        public int Position
        {
            get { return _position; }
            set { _position = value % GameBoard.PropertyCount; }
        }

        public int Money { get; set; } = 0;

        public int LastDiceRoll { get; set; } = 0;

        public int ConsecutiveDoublesRolled { get; set; }
        public int ConsecutiveTurnsInJail { get; set; }
        public bool IsInJail { get; set; }
        public List<Card> GetOutOfJailFreeCards { get; set; } = new List<Card>();

        public Player(IRandomGenerator generator, IBoard gameBoard, string name = "")
        {
            Generator = generator;
            GameBoard = gameBoard;
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
            }
            else
            {
                ConsecutiveDoublesRolled = 0;
            }
            return LastDiceRoll;
        }

        public void ReleaseFromJail()
        {
            IsInJail = false;
            ConsecutiveTurnsInJail = 0;
        }

        public Card UseGetOutOfJailFreeCard()
        {
            if (IsInJail && GetOutOfJailFreeCards.Count > 0)
            {
                var card = GetOutOfJailFreeCards[0];
                GetOutOfJailFreeCards.Remove(card);
                ReleaseFromJail();
                return card;
            }

            return null;
        }
    }
}