using Moq;
using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class GameTests
    {
        private bool brokerCalled, agentCalled, jailerCalled, dealerCalled, bankerCalled;
        private Game game;
        private IProperty[] properties;
        private Mock<IMortgageBroker> broker;
        private Mock<IRealEstateAgent> agent;
        private Mock<IJailer> jailer;
        private Mock<ICardDealer> dealer;
        private Mock<IBanker> banker;
        private PlayerDeque PlayerQueue;
        private Mock<IRandomGenerator> generator;

       [SetUp]
        public void Setup()
        {
            generator = new Mock<IRandomGenerator>();
            generator.Setup(x => x.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(2);


            properties = new IProperty[]
            {
                new Property("a", mapIndex: 0),
                new Property("b", mapIndex: 1),
                new Property("c", mapIndex: 2),
                new Property("d", mapIndex: 3),
                new Property("e", mapIndex: 4),
            };

            Mock<IBoard> gameBoard = new Mock<IBoard>();
            gameBoard.SetupProperty(x => x.PropertyCount, properties.Length);
            var names = new string[] { "A", "B", "C", "D" };
            PlayerFactory factory = new PlayerFactory(names, generator.Object);
            PlayerQueue = new PlayerDeque(generator.Object, factory);


            foreach (var property in properties)
            {
                gameBoard.Setup(x => x.GetPropertyFromName(property.Name)).Returns(property);
                gameBoard.Setup(x => x.GetPropertyFromIndex(property.MapIndex)).Returns(property);
                gameBoard.Setup(x => x.MovePlayerToProperty(PlayerQueue[0], property, true))
                    .Callback(() => PlayerQueue[0].Position = property.MapIndex);
                gameBoard.Setup(x => x.MovePlayerToProperty(PlayerQueue[1], property, true))
                    .Callback(() => PlayerQueue[1].Position = property.MapIndex);
            }

            

            brokerCalled = false;
            agentCalled = false;
            jailerCalled = false;
            dealerCalled = false;
            bankerCalled = false;

            broker = new Mock<IMortgageBroker>();
            broker.Setup(x => x.Process(It.IsAny<IPlayer>())).Callback(() => brokerCalled = true);
            agent = new Mock<IRealEstateAgent>();
            agent.Setup(x => x.Process(It.IsAny<IPlayer>(), 1)).Callback(() => agentCalled = true);
            jailer = new Mock<IJailer>();
            jailer.Setup(x => x.IsInJail(PlayerQueue[0])).Returns(false);
            jailer.Setup(x => x.IsInJail(PlayerQueue[1])).Returns(true);
            jailer.Setup(x => x.Process(It.IsAny<IPlayer>())).Callback(() => jailerCalled = true);
            dealer = new Mock<ICardDealer>();
            dealer.Setup(x => x.Process(It.IsAny<IPlayer>())).Callback(() => dealerCalled = true);
            banker = new Mock<IBanker>();
            banker.Setup(x => x.Process(It.IsAny<IPlayer>())).Callback(() => bankerCalled = true);

            game = new Game(gameBoard.Object, jailer.Object, broker.Object, agent.Object, dealer.Object, banker.Object);
        }

        [Test]
        public void WhenPlayerIsInJailBeginPlayerTurnCallsJailerAndDoesNothingElse()
        {
            game.BeginPlayerTurn(PlayerQueue[1]);
            Assert.IsTrue(jailerCalled);
            Assert.IsFalse(brokerCalled);
        }

        [Test]
        public void WhenPlayerIsNotInJailBeginPlayerTurnCallsBroker()
        {
            game.BeginPlayerTurn(PlayerQueue[0]);
            Assert.IsTrue(jailerCalled);
            Assert.IsTrue(brokerCalled);
        }

        [Test]
        public void WhenPlayerIsNotInJailBeginPlayerTurnMovesPlayerForward()
        {
            var player = PlayerQueue[0];
            player.Position = 3;
            game.BeginPlayerTurn(player);
            Assert.AreEqual(2, player.Position);
        }

        [Test]
        public void WhenPlayerIsNotInJailEndPlayerTurnCallsBroker()
        {
            game.BeginPlayerTurn(PlayerQueue[0]);
            Assert.IsTrue(brokerCalled);
        }

        [Test]
        public void WhenPlayerIsInJailEndPlayerTurnDoesNotCallBroker()
        {
            game.BeginPlayerTurn(PlayerQueue[1]);
            Assert.IsFalse(brokerCalled);
        }

        [Test]
        public void WhenPlayerIsInJailTakeTurnDoesNothing()
        {
            game.TakeTurn(PlayerQueue[1]);
            Assert.IsFalse(agentCalled);
            Assert.IsFalse(jailerCalled);
            Assert.IsFalse(dealerCalled);
            Assert.IsFalse(bankerCalled);
        }

        [Test]
        public void WhenPlayerIsNotInJailTakeTurnCallsMostActors()
        {
            game.TakeTurn(PlayerQueue[0]);
            Assert.IsTrue(agentCalled);
            Assert.IsTrue(jailerCalled);
            Assert.IsTrue(dealerCalled);
            Assert.IsTrue(bankerCalled);
        }
    }
}
