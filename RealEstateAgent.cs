using System;
using System.Collections.Generic;
using System.Linq;

namespace Monopoly
{
    public class RealEstateAgent : IRealEstateAgent
    {
        private Dictionary<IPlayer, List<IProperty>> _ownedProperties;
        private Dictionary<string, Func<IProperty, int, int, int>> _rentFunctions;
        private IBoard _gameBoard;
        private IMortgageBroker _mortgageBroker;
        private const int _minMoneyPlayerMustHaveToBuyProperty = 100;
        private readonly string[] _nonPurchasablePropertyGroups = { "Card Group", "Action Properties", "Uncle Sam's Land" };

        public RealEstateAgent(IBoard gameBoard, IMortgageBroker broker)
        {
            _ownedProperties = new Dictionary<IPlayer, List<IProperty>>();
            
            _gameBoard = gameBoard;
            _mortgageBroker = broker;
            _rentFunctions = new Dictionary<string, Func<IProperty, int, int, int>>()
            {
                {"Normal", CalculateRegularRent},
                {"Incremental", CalculateIncrementalRent},
                {"Multiplicative", CalculateMultiplicativeRent},
            };
        }

        public void Process(IPlayer player, int modifier = 1)
        {
            var property = _gameBoard.GetPropertyFromIndex(player.Position);
            if (!PropertyIsOwned(property))
            {
                AttemptBuyingProperty(player, property);
            }
            else if (!_mortgageBroker.IsPropertyMortgaged(property))
            {
                PayRent(player, property, modifier);
            }
        }

        public void PayRent(IPlayer player, IProperty property, int rentModifier = 1)
        {
            var propertyOwner = GetPropertyOwner(property);
            if (propertyOwner != player)
            {
                var rent = CalculateRent(property, rentModifier, player.LastDiceRoll);
                propertyOwner.Money += rent;
                player.Money -= rent;

                Console.WriteLine("{0} paid ${1} to {2}", player.Name, rent, propertyOwner.Name);
            }
        }

        public bool PropertyIsOwned(IProperty property)
        {
            bool propertyIsOwned = false;
            foreach (var ownedProperties in _ownedProperties.Values)
            {
                propertyIsOwned |= ownedProperties.Contains(property);
            }

            return propertyIsOwned;
        }

        public IPlayer GetPropertyOwner(IProperty property)
        {
            foreach (var owner in _ownedProperties.Keys)
            {
                if (_ownedProperties[owner].Contains(property))
                {
                    return owner;
                }
            }

            return null;
        }

        public List<IProperty> GetAllPropertiesOwnedByPlayer(IPlayer player)
        {
            if (_ownedProperties.ContainsKey(player))
            {
                return _ownedProperties[player];
            }

            return null;
        }

        private void AttemptBuyingProperty(IPlayer player, IProperty property)
        {
            var groupName = _gameBoard.GetGroupFromProperty(property).Name;
            if (player.Money - property.Price > _minMoneyPlayerMustHaveToBuyProperty 
                && !_nonPurchasablePropertyGroups.Contains(groupName))
            {
                PlayerBoughtProperty(player, property);
            }
        }

        public void PlayerBoughtProperty(IPlayer player, IProperty property)
        {
            if (!_ownedProperties.ContainsKey(player))
            {
                _ownedProperties[player] = new List<IProperty>();
            }

            _ownedProperties[player].Add(property);
            player.Money -= property.Price;
            Console.WriteLine("{0} bought property {1}", player.Name, property.Name);
        }

        private List<IProperty> GetOwnedPropertiesInSameGroup(IProperty property)
        {
            var owner = GetPropertyOwner(property);
            var group = _gameBoard.GetGroupFromProperty(property);

            if (owner != null && _ownedProperties.ContainsKey(owner))
            {
                return _ownedProperties[owner].Intersect(group.Properties).ToList();
            }
            
            return new List<IProperty>();
        } 

        private int CalculateRent(IProperty property, int rentModifier = 1, int lastDiceRoll = 0)
        {
            var group = _gameBoard.GetGroupFromProperty(property);
            return _rentFunctions[group.RentFunction](property, rentModifier, lastDiceRoll);
        }

        private int CalculateRegularRent(IProperty property, int modifier = 1, int lastDiceRoll = 0)
        {
            var group = _gameBoard.GetGroupFromProperty(property);
            var ownedPropertiesInGroup = GetOwnedPropertiesInSameGroup(property);
            int allOwnedModifier = (ownedPropertiesInGroup.SequenceEqual(group.Properties)) ? 2 : 1;
            return property.Rent * allOwnedModifier * modifier;
        }

        private int CalculateIncrementalRent(IProperty property, int modifier = 1, int lastDiceRoll = 0)
        {
            var ownedPropertiesInGroup = GetOwnedPropertiesInSameGroup(property).Where(x => x != property);
            var rent = ownedPropertiesInGroup.Aggregate(property.Rent, (current, ownedProperty) => current*2);
            return rent * modifier;
        }

        private int CalculateMultiplicativeRent(IProperty property, int modifier = 1, int lastDiceRoll = 0)
        {
            if (modifier != 1)
            {
                return modifier * lastDiceRoll;
            }

            var group = _gameBoard.GetGroupFromProperty(property);
            bool allPropertiesOwned = group.Properties.Select(GetPropertyOwner).All(x => x != null);
            modifier = (allPropertiesOwned) ? 10 : 4;
            return modifier * lastDiceRoll;
        }
    }
}
