using System.Collections.Generic;
using System.Dynamic;

namespace Monopoly
{
    public class Board
    {
        private List<Property> properties = new List<Property>();

        public Board()
        {
            foreach (var name in Monopoly.PropertyNames)
            {
                properties.Add(new Property(name));
            }
        }

        public string GetPropertyName(int index)
        {
            return properties[index].Name;
        }
    }
}