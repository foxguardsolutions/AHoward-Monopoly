using System.IO;
using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class BoardTests
    {
        private IBoard board;

        [SetUp]
        public void Setup()
        {
            board = new Board(File.ReadAllText("json\\propertyGroupsTests.json"));
        }

        [Test]
        public void PropertyCountEqualsNumberOfProperties()
        {
            Assert.AreEqual(4, board.PropertyCount);
        }

        [TestCase("Park Place", Result = 0)]
        [TestCase("Boulevard", Result = 1)]
        [TestCase("Other Place", Result = 2)]
        [TestCase("Another Boulevard", Result = 3)]
        public int GetPropertyPositionFromNameReturnsCorrectIndex(string toFind)
        {
            return board.GetPropertyPositionFromName(toFind);
        }

        [TestCase(0, 0, 0)]
        [TestCase(0, 1, 1)]
        [TestCase(1, 0, 2)]
        [TestCase(1, 1, 3)]
        public void GetPropertyFromIndexReturnsCorrectProperty(int groupIndex, int propertyIndex, int mapIndex)
        {
            Assert.AreEqual(board.PropertyGroups[groupIndex].Properties[propertyIndex], board.GetPropertyFromIndex(mapIndex));
        }

        [TestCase("Park Place", 0, 0)]
        [TestCase("Boulevard", 0, 1)]
        [TestCase("Other Place", 1, 0)]
        [TestCase("Another Boulevard", 1, 1)]
        public void GetPropertyFromNameReturnsCorrectProperty(string name, int groupIndex, int propertyIndex)
        {
            Assert.AreEqual(board.PropertyGroups[groupIndex].Properties[propertyIndex], board.GetPropertyFromName(name));
        }

        [TestCase("Park Place", Result = "Test Group 1")]
        [TestCase("Other Place", Result = "Test Group 2")]
        public string GetGroupFromPropertyReturnsCorrectProperty(string propertyName)
        {
            var property = board.GetPropertyFromName(propertyName);
            var group = board.GetGroupFromProperty(property);
            return group.Name;
        }

        [TestCase("Park Place", Result = 0)]
        [TestCase("Boulevard", Result = 1)]
        [TestCase("Another Boulevard", Result = 3)]
        public int MovePlayerToPropertyUpdatesPlayersPosition(string propertyName)
        {
            Player player = new Player(null);
            var property = board.GetPropertyFromName(propertyName);
            player.Position = 0;
            board.MovePlayerToProperty(player, property);
            return player.Position;
        }
    }
}
