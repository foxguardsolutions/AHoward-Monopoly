using System.IO;
using System.Linq;
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

        [Test]
        public void PlayerPurchasedPropertyUpdatesGroupOwnersAndPropertysOwner()
        {
            Player player = new Player(new RandomGeneratorMoc(), board, "A");
            var property = board.GetPropertyFromName("Park Place");
            board.PlayerPurchasedProperty(player, property);
            Assert.AreEqual(1, board.PropertyGroups[0].Owners.Length);
            Assert.AreEqual(player, board.PropertyGroups[0].Owners[0]);
            Assert.AreEqual(player, property.Owner);
        }

        [Test]
        public void CalculateRentReturnsZeroWhenPropertyIsMortgaged()
        {
            Player player = new Player(new RandomGenerator(), board, "A");
            var property = board.GetPropertyFromName("Park Place");
            property.Mortgaged = true;
            Assert.AreEqual(0, board.CalculateRent(property));
        }

        [Test]
        public void CalculateRentReturnsRentPriceWhenNotAllPropertiesOfGroupAreOwned()
        {
            Player player = new Player(new RandomGenerator(), board, "A");
            var property = board.GetPropertyFromName("Park Place");
            board.PlayerPurchasedProperty(player, property);
            Assert.AreEqual(90, board.CalculateRent(property));
        }

        [Test]
        public void CalculateRentReturnsDoubleRentPriceWhenAllPropertiesOfGroupAreOwned()
        {
            Player player = new Player(new RandomGenerator(), board, "A");
            var parkPlace = board.GetPropertyFromName("Park Place");
            var boulevard = board.GetPropertyFromName("Boulevard");
            board.PlayerPurchasedProperty(player, parkPlace);
            board.PlayerPurchasedProperty(player, boulevard);
            Assert.AreEqual(180, board.CalculateRent(parkPlace));
        }

        [Test]
        public void CalculateRentReturnsRentPriceWhenAllPropertiesOfGroupAreOwnedByDifferentPlayers()
        {
            Player playerA = new Player(new RandomGenerator(), board, "A");
            Player playerB = new Player(new RandomGenerator(), board, "B");
            var parkPlace = board.GetPropertyFromName("Park Place");
            var boulevard = board.GetPropertyFromName("Boulevard");
            board.PlayerPurchasedProperty(playerA, parkPlace);
            board.PlayerPurchasedProperty(playerB, boulevard);
            Assert.AreEqual(90, board.CalculateRent(parkPlace));
        }

        [Test]
        public void GetAllPropertiesOwnedByPlayerReturnsAllPropertiesOwnedByPlayer()
        {
            Player player = new Player(new RandomGeneratorMoc(), board, "A");
            var parkPlace = board.GetPropertyFromName("Park Place");
            var boulevard = board.GetPropertyFromName("Boulevard");
            var otherPlace = board.GetPropertyFromName("Other Place");

            var testSequence = new IProperty[] { };
            Assert.IsTrue(testSequence.SequenceEqual(board.GetAllPropertiesOwnedByPlayer(player)));

            parkPlace.Owner = player;
            testSequence = new IProperty[] { parkPlace };
            Assert.IsTrue(testSequence.SequenceEqual(board.GetAllPropertiesOwnedByPlayer(player)));

            boulevard.Owner = player;
            testSequence = new IProperty[] { parkPlace, boulevard };
            Assert.IsTrue(testSequence.SequenceEqual(board.GetAllPropertiesOwnedByPlayer(player)));

            otherPlace.Owner = player;
            testSequence = new IProperty[] { parkPlace, boulevard, otherPlace };
            Assert.IsTrue(testSequence.SequenceEqual(board.GetAllPropertiesOwnedByPlayer(player)));
        }
    }
}
