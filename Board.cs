using System;
using System.Collections.Generic;
using System.Linq;

namespace Monopoly
{
    public class Board : IBoard
    {
        private readonly Dictionary<string, IPropertyGroup> nameToGroupMapping;
        private readonly Dictionary<int, IPropertyGroup> mapIndexToGroupMapping;
        private const int _moneyPaidForPassingGo = 200;

        public IPropertyGroup[] PropertyGroups { get; set; }
        public int PropertyCount { get; set; }

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

        public IPropertyGroup GetGroupFromProperty(IProperty property)
        {
            return nameToGroupMapping[property.Name];
        }

        public void MovePlayerToProperty(IPlayer player, IProperty property, bool canCollectGo = true)
        {
            if (property.MapIndex < player.Position && canCollectGo)
            {
                player.Money += _moneyPaidForPassingGo;
            }

            player.Position = property.MapIndex;
            Console.WriteLine("{0} moved to property: {1}", player.Name, GetPropertyFromIndex(player.Position).Name);
        }
    }
}