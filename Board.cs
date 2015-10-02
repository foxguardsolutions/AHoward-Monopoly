using System;
using System.Collections.Generic;
using System.Linq;

namespace Monopoly
{
    public class Board : IBoard
    {
        private Dictionary<string, IPropertyGroup> nameToGroupMapping;
        private Dictionary<int, IPropertyGroup> mapIndexToGroupMapping;

        public IPropertyGroup[] PropertyGroups { get; set; }
        public int PropertyCount { get; }

        public Board(string propertyGroupData)
        {
            nameToGroupMapping = new Dictionary<string, IPropertyGroup>();
            mapIndexToGroupMapping = new Dictionary<int, IPropertyGroup>();
            PropertyGroups = PropertyGroupFactory.GenerateGroups(propertyGroupData);
            PropertyCount = PropertyGroups.Sum(x => x.Properties.Length);
            GenerateMappings();
        }

        private void GenerateMappings()
        {
            foreach (var group in PropertyGroups)
            {
                foreach (var property in group.Properties)
                {
                    if (!nameToGroupMapping.ContainsKey(property.Name))
                    {
                        nameToGroupMapping.Add(property.Name, group);
                    }

                    mapIndexToGroupMapping.Add(property.MapIndex, group);
                }
            }
        }

        public int GetPropertyPositionFromName(string name)
        {
            return GetPropertyFromName(name).MapIndex;
        }

        public IProperty GetPropertyFromIndex(int index)
        {
            return mapIndexToGroupMapping[index].GetPropertyFromPropertyIndex(index);
        }

        public IProperty GetPropertyFromName(string name)
        {
            return nameToGroupMapping[name].GetPropertyFromName(name);
        }

        public void PlayerPurchasedProperty(IPlayer player, IProperty property)
        {
            nameToGroupMapping[property.Name].AddOwner(player);
            property.Owner = player;
        }

        public int CalculateRent(IProperty property)
        {
            if (property.Mortgaged)
            {
                return 0;
            }

            var group = nameToGroupMapping[property.Name];
            if (group.AllPropertiesOwned() && group.HasSingleOwner())
            {
                return property.Rent * 2;
            }

            return property.Rent;
        }

        public IProperty[] GetAllPropertiesOwnedByPlayer(IPlayer player)
        {
            IProperty[] ownedProperties = { };
            foreach (var group in PropertyGroups)
            {
                ownedProperties = ownedProperties.Concat(group.GetPropertiesInGroupOwnedByPlayer(player)).ToArray();
            }

            return ownedProperties;
        }
    }
}