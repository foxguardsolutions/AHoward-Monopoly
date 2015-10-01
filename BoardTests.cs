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
    }
}
