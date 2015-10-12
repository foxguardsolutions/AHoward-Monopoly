using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class PropertyGroupTests
    {
        private IPropertyGroup group;
        private IProperty[] properties;

        [SetUp]
        public void Setup()
        {
            properties = new IProperty[]
            {
                new Property("Park Place", mapIndex : 10),
                new Property("Boulevard", mapIndex : 20), 
            };

            group = new PropertyGroup();
            group.Properties = properties;
        }

        [TestCase("Park Place", 0)]
        [TestCase("Boulevard", 1)]
        public void GetPropertyFromNameFetchesAppropriatePropertyObject(string name, int expectedIndex)
        {
            Assert.AreEqual(properties[expectedIndex], group.GetPropertyFromName(name));
        }

        [TestCase(10, 0)]
        [TestCase(20, 1)]
        public void GetPropertyFromPropertyIndexReturnsCorrectProperty(int mapIndex, int expectedIndex)
        {
            Assert.AreEqual(properties[expectedIndex], group.GetPropertyFromPropertyIndex(mapIndex));
        }
    }
}
