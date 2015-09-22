namespace Monopoly
{
    public class Player
    {
        private int _position;
        public string Name { get; }

        public int Position
        {
            get { return _position; }
            set { _position = value % Monopoly.PropertyCount; }
        }

        public Player(string name = "", int position = 0)
        {
            Name = name;
            Position = position;
        }
    }
}