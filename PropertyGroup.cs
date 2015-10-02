using System.Linq;

namespace Monopoly
{
    public class PropertyGroup : IPropertyGroup
    {
        public IPlayer[] Owners { get; set; }

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
    }
}
