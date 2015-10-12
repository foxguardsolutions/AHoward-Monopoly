using Moq;
using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class PlayerTests
    {
        private Player player;

        [SetUp]
        public void Setup()
        {
            var mockGenerator = new Mock<IRandomGenerator>();
            mockGenerator.Setup(x => x.Next(1, 6)).Returns(2);
            player = new Player(mockGenerator.Object);
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

        [Test]
        public void RollBothDiceIncreasesConsecutiveDoublesRolled()
        {
            Assert.AreEqual(0, player.ConsecutiveDoublesRolled);
            player.RollBothDice();
            Assert.AreEqual(1, player.ConsecutiveDoublesRolled);
            player.RollBothDice();
            Assert.AreEqual(2, player.ConsecutiveDoublesRolled);
            player.RollBothDice();
            Assert.AreEqual(3, player.ConsecutiveDoublesRolled);
        }
    }
}
