using System.Linq;

namespace Monopoly
{
    public class PropertyGroup : IPropertyGroup
    {
        public string Name { get; set; }
        public IProperty[] Properties { get; set; }
        public string RentFunction { get; set; }

        public IProperty GetPropertyFromName(string name)
        {
            return Properties.Where(x => x.Name == name).First();
        }

        public IProperty GetPropertyFromPropertyIndex(int index)
        {
            return Properties.Where(x => x.MapIndex == index).First();
        }
    }
}
