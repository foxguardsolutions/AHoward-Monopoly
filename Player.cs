namespace Monopoly
{
    public class Player : IPlayer
    {
        private int _position;
        private int _minDieRoll = 1, _maxDieRoll = 6;
        public string Name { get; }

        public IBoard GameBoard { get; }
        public IRandomGenerator Generator { get; }
        public int Position
        {
            get { return _position; }
            set { _position = value % GameBoard.PropertyCount; }
        }

        public int Money { get; set; } = 0;

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
            return RollDie() + RollDie();
        }
    }
}