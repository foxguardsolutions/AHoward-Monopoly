using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monopoly
{
    public class Monopoly
    {
        private static readonly string[] _propertyNames =
        {
            "Go",
            "Mediterranean Avenue",
            "Community Chest",
            "Baltic Avenue",
            "Income Tax",
            "Reading RailRoad",
            "Oriental Avenue",
            "Chance",
            "Vermont Avenue",
            "Connecticut Avenue",
            "Jail",
            "St. Charles Place",
            "Electric Company",
            "States Avenue",
            "Virginia Avenue",
            "Pennsylvania Railroad",
            "St. James Place",
            "Community Chest",
            "Tennessee Avenue",
            "New York Avenue",
            "Free Parking",
            "Kentucky Avenue",
            "Chance",
            "Indiana Avenue",
            "Illinois Avenue",
            "B. & O. Railroad",
            "Atlantic Avenue",
            "Ventnor Avenue",
            "Water Works",
            "Marvin Gardens",
            "Go To Jail",
            "Pacific Avenue",
            "North Carolina Avenue",
            "Community Chest",
            "Pennsylvania Avenue",
            "Short Line Railroad",
            "Chance",
            "Park Place",
            "Luxury Tax",
            "Boardwalk",
        };

        private Board _gameBoard;
        private List<Player> _players;
        private Random _generator = new Random();
        private int _minDieRoll = 1, _maxDieRoll = 6;

        public static string[] PropertyNames
        {
            get { return _propertyNames; }
        }

        public static int PropertyCount
        {
            get { return _propertyNames.Length; }
        }

        public Monopoly()
        {
            _gameBoard = new Board();
            _players = new List<Player>();
        }

        public int RollDie()
        {
            return _generator.Next(_minDieRoll, _maxDieRoll + 1);
        }

        public static int Main(string[] args)
        {
            return 0;
        }
    }
}
