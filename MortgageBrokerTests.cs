using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class MortgageBrokerTests
    {
        private IMortgageBroker broker;
        private IBoard board;
        private IRealEstateAgent agent;
        private IPlayer player;
        private IProperty property;

        [SetUp]
        public void Setup()
        {
            var mockBoard = new Mock<IBoard>();
            mockBoard.SetupProperty(x => x.PropertyCount, 40);
            property = new Property();
            player = new Player(null);
            var mockAgent = new Mock<IRealEstateAgent>();
            mockAgent.Setup(x => x.GetAllPropertiesOwnedByPlayer(player)).Returns(new List<IProperty>(new IProperty[] {property}));
            broker = new MortgageBroker();
            broker.EstateAgent = mockAgent.Object;
        }

        [Test]
        public void MortgagePropertyMakesIsMortgagedReturnTrue()
        {
            Assert.IsFalse(broker.IsPropertyMortgaged(property));
            broker.MortgageProperty(player, property);
            Assert.IsTrue(broker.IsPropertyMortgaged(property));
        }

        [Test]
        public void UnmortgagePropertyMakesIsMortgagedReturnFalse()
        {
            Assert.IsFalse(broker.IsPropertyMortgaged(property));
            broker.MortgageProperty(player, property);
            Assert.IsTrue(broker.IsPropertyMortgaged(property));
            broker.UnmortgageProperty(player, property);
            Assert.IsFalse(broker.IsPropertyMortgaged(property));
        }

        [TestCase(5, Result = true)]
        [TestCase(100, Result = false)]
        public bool ProcessPlayerWillMortgageAPlayersPropertyWhenFundsAreLow(int startMoney)
        {
            player.Money = startMoney;
            broker.Process(player);
            return broker.IsPropertyMortgaged(property);
        }

        [TestCase(2000, Result = false)]
        [TestCase(1000, Result = true)]
        public bool ProcessPlayerWillUnmortgageAPlayersPropertyWhenFundsArePlentiful(int startMoney)
        {
            broker.MortgageProperty(player, property);
            player.Money = startMoney;
            broker.Process(player);
            return broker.IsPropertyMortgaged(property);
        }
    }
}
