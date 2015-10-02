using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class PropertyGroupTests
    {
        private IPropertyGroup group;
        private IProperty[] properties;
        private Player[] players;

        [SetUp]
        public void Setup()
        {
            RandomGeneratorMoc generator = new RandomGeneratorMoc();
            Board board = new Board(File.ReadAllText("json\\propertyGroupsTests.json"));
            players = new Player[]
            {
                new Player(generator, board, "A"),
                new Player(generator, board, "B"),
                new Player(generator, board, "C"),
            };

            group = board.PropertyGroups[0];
            properties = group.Properties;
        }

        [TestCase(0, Result = false)]
        [TestCase(1, Result = false)]
        [TestCase(2, Result = true)]
        public bool AllPropertiesOwnedReturnsFalseWhenSomePropertiesAreNotOwned(int propertiesToOwn)
        {
            for (int i = 0; i < propertiesToOwn; i++)
            {
                properties[i].Owner = players[0];
            }

            return group.AllPropertiesOwned();
        }

        [TestCase("Park Place", 0)]
        [TestCase("Boulevard", 1)]
        public void GetPropertyFromNameFetchesAppropriatePropertyObject(string name, int expectedIndex)
        {
            Assert.AreEqual(properties[expectedIndex], group.GetPropertyFromName(name));
        }

        [TestCase(10, 0)]
        [TestCase(12, 1)]
        public void GetPropertyFromPropertyIndexReturnsCorrectProperty(int mapIndex, int expectedIndex)
        {
            Assert.AreEqual(properties[expectedIndex], group.GetPropertyFromPropertyIndex(mapIndex));
        }

        [TestCase(0, Result = false)]
        [TestCase(1, Result = true)]
        [TestCase(2, Result = false)]
        public bool HasSingleOwnerReturnsTrueWhenOnlyOnePlayerOwnsAnyOfTheProperties(int numberOfOwners)
        {
            List<Player> newOwners = new List<Player>();
            for (int i = 0; i < numberOfOwners; i++)
            {
                newOwners.Add(players[i]);
            }

            group.Owners = newOwners.ToArray();

            return group.HasSingleOwner();
        }
    }
}
