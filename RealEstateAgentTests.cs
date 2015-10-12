using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class RealEstateAgentTests
    {
        private IMortgageBroker broker;
        private IBoard board;
        private IRealEstateAgent agent;
        private IPlayer player;
        private Mock<IRandomGenerator> mockGenerator;

        [SetUp]
        public void Setup()
        {
            mockGenerator = new Mock<IRandomGenerator>();
            mockGenerator.Setup(x => x.Next(1, 6)).Returns(2);

            board = new Board(File.ReadAllText("json\\propertyGroups.json"));
            broker = new MortgageBroker();
            agent = new RealEstateAgent(board, broker);
            broker.EstateAgent = agent;
            player = new Player(mockGenerator.Object);
            player.RollBothDice();
        }

        [Test]
        public void PlayerBoughtPropertyCausesIsPropertyOwnedToReturnTrue()
        {
            var property = board.GetPropertyFromName("Reading RailRoad");
            Assert.IsFalse(agent.PropertyIsOwned(property));
            agent.PlayerBoughtProperty(player, property);
            Assert.IsTrue(agent.PropertyIsOwned(property));
        }

        [Test]
        public void GetPropertyOwnerReturnsThePlayerThatOwnsAGivenPropertyOrNull()
        {
            var property = board.GetPropertyFromName("Reading RailRoad");
            Assert.IsNull(agent.GetPropertyOwner(property));
            agent.PlayerBoughtProperty(player, property);
            Assert.AreEqual(player, agent.GetPropertyOwner(property));
        }

        [Test]
        public void GetAllPropertiesOwnedByPropertyReturnsCompleteSetOfAllPropertiesOwned()
        {
            List<IProperty> properties = new List<IProperty>()
            {
                board.GetPropertyFromName("Reading RailRoad"),
                board.GetPropertyFromName("Electric Company"),
                board.GetPropertyFromName("Mediterranean Avenue")
            };

            Assert.IsNull(agent.GetAllPropertiesOwnedByPlayer(player));

            foreach (var property in properties)
            {
                agent.PlayerBoughtProperty(player, property);
            }

            Assert.IsTrue(properties.SequenceEqual(agent.GetAllPropertiesOwnedByPlayer(player)));
        }

        [TestCase("Mediterranean Avenue", 1, 1, Result = 20)]
        [TestCase("Mediterranean Avenue", 2, 1, Result = 40)]
        [TestCase("Pennsylvania RailRoad", 1, 1, Result = 25)]
        [TestCase("Pennsylvania RailRoad", 2, 1, Result = 50)]
        [TestCase("Pennsylvania RailRoad", 3, 1, Result = 100)]
        [TestCase("Pennsylvania RailRoad", 4, 1, Result = 200)]
        [TestCase("Electric Company", 1, 1, Result = 16)]
        [TestCase("Electric Company", 2, 1, Result = 40)]
        [TestCase("Electric Company", 1, 3, Result = 12)]
        public int PayRentSelectsAdjustsRentAmountBasedOnPropertyGroupType(string propertyName, int propertiesToOwn, int rentModifier)
        {
            var owner = new Player(null);
            var property = board.GetPropertyFromName(propertyName);
            var group = board.GetGroupFromProperty(property);
            property = group.Properties[0];

            for (int i = 0; i < propertiesToOwn; i++)
            {
                owner.Money += group.Properties[i].Price;
                agent.PlayerBoughtProperty(owner, group.Properties[i]);
            }

            agent.PayRent(player, property, rentModifier);
            return owner.Money;
        }
    }
}
