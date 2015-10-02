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

        [TestCase("Park Place", Result = 10)]
        [TestCase("Boulevard", Result = 12)]
        [TestCase("Other Place", Result = 9)]
        [TestCase("Another Boulevard", Result = 8)]
        public int GetPropertyPositionFromNameReturnsCorrectIndex(string toFind)
        {
            return board.GetPropertyPositionFromName(toFind);
        }

        [TestCase(0, 0, 10)]
        [TestCase(0, 1, 12)]
        [TestCase(1, 0, 9)]
        [TestCase(1, 1, 8)]
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
    }
}
