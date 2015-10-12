using System;
using System.Collections.Generic;
using System.Linq;

namespace Monopoly
{
    public class MortgageBroker : IMortgageBroker
    {
        private Dictionary<IPlayer, List<IProperty>> _mortgagedProperties;
        private const double _percentagePaidForMortgagingProperty = 0.75;
        private const double _percentagePaidForUnmortgagingProperty = 1.00;
        private const int _minFundsBeforeMortgaging = 100;
        private const int _minFundsBeforeUnmortgaging = 1000;
        public IRealEstateAgent EstateAgent { get; set; }

        public MortgageBroker()
        {
            _mortgagedProperties = new Dictionary<IPlayer, List<IProperty>>();
        }

        public void Process(IPlayer player)
        {
            var ownedProperties = EstateAgent.GetAllPropertiesOwnedByPlayer(player);
            if (player.Money < _minFundsBeforeMortgaging && ownedProperties != null)
            {
                AttemptMortgagingProperty(player, ownedProperties);
            }
            else if (player.Money > _minFundsBeforeUnmortgaging)
            {
                AttemptUnmortgagingProperty(player);
            }
            
        }

        public bool IsPropertyMortgaged(IProperty property)
        {
            bool propertyMortgaged = false;
            foreach (var propertyList in _mortgagedProperties.Values)
            {
                propertyMortgaged |= propertyList.Contains(property);
            }

            return propertyMortgaged;
        }

        private void AttemptMortgagingProperty(IPlayer player, List<IProperty> ownedProperties)
        {
            if (_mortgagedProperties.ContainsKey(player))
            {
                var unmortgagedProperties = ownedProperties.Except(_mortgagedProperties[player]).ToList();
                MortgageProperty(player, unmortgagedProperties[0]);
            }
            else
            {
                MortgageProperty(player, ownedProperties[0]);
            }
        }

        private void AttemptUnmortgagingProperty(IPlayer player)
        {
            if (_mortgagedProperties.ContainsKey(player))
            {
                foreach (var property in _mortgagedProperties[player].ToArray())
                {
                    if (player.Money - CalculateUnmortgageMoneyPaid(property) > _minFundsBeforeUnmortgaging)
                    {
                        UnmortgageProperty(player, property);
                    }
                }
            }
        }

        public void MortgageProperty(IPlayer player, IProperty property)
        {
            if (!_mortgagedProperties.ContainsKey(player))
            {
                _mortgagedProperties[player] = new List<IProperty>();
            }

            _mortgagedProperties[player].Add(property);
            player.Money += (int) (property.Price *_percentagePaidForMortgagingProperty);
            Console.WriteLine("{0} mortgaged property {1}", player.Name, property.Name);
        }

        public void UnmortgageProperty(IPlayer player, IProperty property)
        {
            _mortgagedProperties[player].Remove(property);
            player.Money -= CalculateUnmortgageMoneyPaid(property);
            Console.WriteLine("{0} unmortgaged property {1}", player.Name, property.Name);
        }

        private int CalculateUnmortgageMoneyPaid(IProperty property)
        {
            return (int)(property.Price * _percentagePaidForUnmortgagingProperty);
        }
    }
}
