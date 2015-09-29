using System.Linq;

namespace Monopoly
{
    public class PropertyFactory : IPropertyFactory
    {
        private string[] _propertyNames;

        public PropertyFactory(string[] names)
        {
            _propertyNames = names;
        }

        public IProperty[] GenerateProperties()
        {
            return _propertyNames.Select(property => new Property(property)).Cast<IProperty>().ToArray();
        }
    }
}
