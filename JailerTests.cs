using System.IO;
using Moq;
using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class JailerTests
    {
        private IJailer jailer;
        private IBoard board;
        private Mock<ICardDealer> dealer;
        private IPlayer player;

        [SetUp]
        public void Setup()
        {
            board = new Board(File.ReadAllText("json\\propertyGroups.json"));
            dealer = new Mock<ICardDealer>();
            player = new Player(null);
            jailer = new Jailer(board);
            jailer.Dealer = dealer.Object;
        }

        [Test]
        public void SendPlayerToJailCausesIsInJailToReturnTrue()
        {
            Assert.IsFalse(jailer.IsInJail(player));
            jailer.SendPlayerToJail(player);
            Assert.IsTrue(jailer.IsInJail(player));
        }

        [TestCase(0, Result = false)]
        [TestCase(2, Result = false)]
        [TestCase(3, Result = true)]
        [TestCase(5, Result = true)]
        public bool ProcessSendsPlayerToJailWhenConsecutiveDoublesRolledIsMoreThan2(int doublesRolled)
        {
            player.ConsecutiveDoublesRolled = doublesRolled;
            jailer.Process(player);
            return jailer.IsInJail(player);
        }

        [TestCase("Go To Jail", Result = true)]
        [TestCase("Go", Result = false)]
        [TestCase("Vermont Avenue", Result = false)]
        public bool ProcessSendsPlayerToJailWhenHisPositionIsOnGoToJailProperty(string startPropertyName)
        {
            var index = board.GetPropertyFromName(startPropertyName).MapIndex;
            player.Position = index;
            jailer.Process(player);
            return jailer.IsInJail(player);
        }

        [Test]
        public void ProcessReleasesPlayerFromJailWhenHeRollsDoubles()
        {
            player.ConsecutiveDoublesRolled = 1;
            jailer.SendPlayerToJail(player);
            jailer.Process(player);
            Assert.IsFalse(jailer.IsInJail(player));
        }

        [Test]
        public void ProcessReleasesPlayerFromJailIfHeHasAGetOutOfJailFreeCard()
        {
            jailer.GivePlayerGetOutJailFreeCard(player, new Card());
            jailer.SendPlayerToJail(player);
            jailer.Process(player);
            Assert.IsFalse(jailer.IsInJail(player));
        }

        [TestCase(0, Result = true)]
        [TestCase(100, Result = true)]
        [TestCase(1001, Result = false)]
        public bool ProcessReleasesPlayerFromJailWhenHeBuysOutOfJail(int startMoney)
        {
            player.Money = startMoney;
            jailer.SendPlayerToJail(player);
            jailer.Process(player);
            return jailer.IsInJail(player);
        }


        [TestCase(0, Result = true)]
        [TestCase(2, Result = true)]
        [TestCase(3, Result = false)]
        public bool ProcessReleasesPlayerFromJailWhenHeHasSpent3RoundsInJail(int turnsInJail)
        {
            player.ConsecutiveTurnsInJail = turnsInJail;
            jailer.SendPlayerToJail(player);
            jailer.Process(player);
            Assert.AreEqual((turnsInJail >= 3) ? -50 : 0, player.Money);
            return jailer.IsInJail(player);
        }
    }
}
