using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class PropertyGroupFactoryTests
    {
        [SetUp]
        public void Setup()
        {
            var kernel = new Ninject.StandardKernel();
            kernel.Bind<IProperty>().To<Property>();
        }

        [Test]
        public void GeneratePropertyGroupsWorksCorrectly()
        {
            var jsonStrings = File.ReadAllLines("json\\propertyGroupsTests.json");
            string jsonString = jsonStrings.Aggregate((current, str) => current + str);

            PropertyGroup[] groups = PropertyGroupFactory.GenerateGroups(jsonString);

            Assert.AreEqual(2, groups.Length);
            foreach (var group in groups)
            {
                Assert.AreEqual(2, group.Properties.Length);
                Assert.AreEqual(null, group.Owners);
                Assert.AreEqual(typeof(Property), group.Properties[0].GetType());
            }
        }
    }
}
