using System.Linq;

namespace Monopoly
{
    public class Board : IBoard
    {
        public IProperty[] Properties { get; set; }
        public int PropertyCount
        {
            get { return Properties.Length; }
        }

        public Board(IPropertyFactory propertyFactory)
        {
            Properties = propertyFactory.GenerateProperties();
        }

        public int GetPropertyPositionFromName(string name)
        {
            return Properties.Select(x => x.Name).ToList().IndexOf(name);
        }

        public IProperty GetPropertyFromIndex(int index)
        {
            return Properties[index];
        }

        public IProperty GetPropertyFromName(string name)
        {
            return GetPropertyFromIndex(GetPropertyPositionFromName(name));
        }
    }
}