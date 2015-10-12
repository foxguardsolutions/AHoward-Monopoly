using System;

namespace Monopoly
{
    public class Banker : IBanker
    {
        private IBoard _gameBoard;
        private string _taxableGroup = "Uncle Sam's Land";
        private string _incomeTaxLand = "Income Tax";
        private string _luxuryTaxLand = "Luxury Tax";
        private const double _percentagePaidForIncomeTax = 0.20;
        private const int _maxIncomeTaxPaid = 200;
        private const int _luxuryTaxPaid = 75;

        public Banker(IBoard gameBoard)
        {
            _gameBoard = gameBoard;
        }

        public void CollectFromPlayer(IPlayer player, int amount)
        {
            player.Money -= amount;
            Console.WriteLine("{0} paid ${1} to the bank", player.Name, amount);
        }

        public void PayPlayer(IPlayer player, int amount)
        {
            player.Money += amount;
            Console.WriteLine("Bank paid ${1} to {0}", player.Name, amount);
        }

        public void Process(IPlayer player)
        {
            var property = _gameBoard.GetPropertyFromIndex(player.Position);
            var group = _gameBoard.GetGroupFromProperty(property);
            if (group.Name == _taxableGroup)
            {
                TaxPlayer(player, property);
            }
        }

        private void TaxPlayer(IPlayer player, IProperty property)
        {
            var amount = 0;
            if (property.Name == _luxuryTaxLand)
            {
                amount = _luxuryTaxPaid;
            }
            else if (property.Name == _incomeTaxLand)
            {
                amount = Math.Min((int)(player.Money * _percentagePaidForIncomeTax), _maxIncomeTaxPaid);
            }

            CollectFromPlayer(player, amount);
        }
    }
}
