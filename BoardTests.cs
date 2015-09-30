using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class BoardTests
    {
        [TestCase("a", Result = 1)]
        [TestCase("a", "b", Result = 2)]
        [TestCase("a", "b", "c", Result = 3)]
        [TestCase("a", "b", "c", "d", Result = 4)]
        public int PropertyCountEqualsNumberOfProperties(params string[] names)
        {
            IBoard board = new Board(new PropertyFactory(names));
            return board.PropertyCount;
        }

        [TestCase("a", "a", "b", "c", "d", Result = 0)]
        [TestCase("b", "a", "b", "c", "d", Result = 1)]
        [TestCase("c", "a", "b", "c", "d", Result = 2)]
        [TestCase("d", "a", "b", "c", "d", Result = 3)]
        public int GetPropertyPositionFromNameReturnsCorrectIndex(string toFind, params string[] names)
        {
            IBoard board = new Board(new PropertyFactory(names));
            return board.GetPropertyPositionFromName(toFind);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void GetPropertyFromIndexReturnsCorrectProperty(int index)
        {
            IProperty[] properties =
            {
                new Property("a"),
                new Property("b"),
                new Property("c"),
                new Property("d"),
            };
            IBoard board = new Board(new PropertyFactory(new string[] { }));
            board.Properties = properties;
            Assert.AreEqual(properties[index], board.GetPropertyFromIndex(index));
        }

        [TestCase("a", 0)]
        [TestCase("b", 1)]
        [TestCase("c", 2)]
        [TestCase("d", 3)]
        public void GetPropertyFromNameReturnsCorrectProperty(string name, int expectedIndex)
        {
            IProperty[] properties =
            {
                new Property("a"),
                new Property("b"),
                new Property("c"),
                new Property("d"),
            };
            IBoard board = new Board(new PropertyFactory(new string[] { }));
            board.Properties = properties;
            Assert.AreEqual(properties[expectedIndex], board.GetPropertyFromName(name));
        }
    }
}
