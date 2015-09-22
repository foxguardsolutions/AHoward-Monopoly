using System;

namespace Monopoly
{
    public delegate void TurnEndedDelegate(object sender);

    public class Player
    {
        private int _position;
        private int _minDieRoll = 1, _maxDieRoll = 6;
        public string Name { get; }
        public event TurnEndedDelegate TurnEnded;

        public int Position
        {
            get { return _position; }
            set { _position = value % Board.PropertyCount; }
        }

        public Player(string name = "", int position = 0)
        {
            Name = name;
            Position = position;
        }

        public int RollDie()
        {
            return Monopoly.Generator.Next(_minDieRoll, _maxDieRoll + 1);
        }

        public void TakeTurn()
        {
            Position += RollDie() + RollDie();

            if (TurnEnded != null)
            {
                TurnEnded(this);
            }
        }
    }
}