using System;
using System.Collections.Generic;
using System.Linq;

namespace Monopoly
{
    public class PropertyGroup : IPropertyGroup
    {
        public IPlayer[] Owners { get; set; } = new IPlayer[0];

        public IProperty[] Properties { get; set; }

        public bool AllPropertiesOwned()
        {
            return Properties.All(x => x.Owner != null);
        }

        public IProperty GetPropertyFromName(string name)
        {
            return Properties.Where(x => x.Name == name).First();
        }

        public IProperty GetPropertyFromPropertyIndex(int index)
        {
            return Properties.Where(x => x.MapIndex == index).First();
        }

        public bool HasSingleOwner()
        {
            return Owners.Length == 1;
        }

        public void AddOwner(IPlayer player)
        {
            if (!Owners.Contains(player))
            {
                List<IPlayer> ownerCopy = Owners.ToList();
                ownerCopy.Add(player);
                Owners = ownerCopy.ToArray();
            }
        }

        public IProperty[] GetPropertiesInGroupOwnedByPlayer(IPlayer player)
        {
            return Properties.Where(x => x.Owner == player).ToArray();
        }

        public int GetNumberOfPropertiesInGroupOwnedByPlayer(IPlayer player)
        {
            return GetPropertiesInGroupOwnedByPlayer(player).Count();
        }
    }
}
