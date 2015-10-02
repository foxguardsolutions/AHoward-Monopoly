using System.IO;
using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class PlayerTests
    {
        private IBoard gameBoard;
        private IRandomGenerator generator;
        private Player player;

        [SetUp]
        public void Setup()
        {
            generator = new RandomGeneratorMoc();
            gameBoard = new Board(File.ReadAllText("json\\propertyGroupsTests.json"));
            player = new Player(generator, gameBoard);
        }

        [TestCase(0, Result = 0)]
        [TestCase(1, Result = 1)]
        [TestCase(2, Result = 2)]
        [TestCase(3, Result = 3)]
        [TestCase(4, Result = 0)]
        [TestCase(5, Result = 1)]
        [TestCase(6, Result = 2)]
        public int PlayerPositionWrapsAroundAfterExceedingNumberOfProperties(int position)
        {
            player.Position = position;
            return player.Position;
        }

        [TestCase(Result = 2)]
        public int RollDieReturnsNumberBetween1and6()
        {
            return player.RollDie();
        }

        [TestCase(Result = 4)]
        public int RollBothDiceReturnsNumberBeen2and12()
        {
            return player.RollBothDice();
        }
    }
}
