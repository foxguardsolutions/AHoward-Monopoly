using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class PlayerDequeTests
    {
        private IPlayerFactory playerFactory;
        private IRandomGenerator generator;
        private string[] players;

        [SetUp]
        public void Setup()
        {
            var mgenerator = new Mock<IRandomGenerator>();
            mgenerator.Setup(x => x.Next(1, 6)).Returns(2);
            generator = mgenerator.Object;

            Mock<IBoard> board = new Mock<IBoard>();
            board.SetupProperty(x => x.PropertyCount, 40);

            players = new string[]
            {
                "a",
                "b",
                "c",
                "d"
            };

            var mfactory = new Mock<IPlayerFactory>();
            mfactory.Setup(x => x.GeneratePlayers()).Returns(new IPlayer[] {
                new Player(mgenerator.Object, players[0]),
                new Player(mgenerator.Object, players[1]),
                new Player(mgenerator.Object, players[2]),
                new Player(mgenerator.Object, players[3]),
            });
            playerFactory = mfactory.Object;
        }

        [Test]
        public void ShuffleReordersPlayers()
        {
            // Unfortunately the shuffled version with this generator is the same :(
            string[] shuffledPlayers = new string[]
            {
                players[0],
                players[1],
                players[2],
                players[3],
            };

            PlayerDeque deque = new PlayerDeque(generator, playerFactory);
            Assert.IsTrue(shuffledPlayers.SequenceEqual(deque.Select(x => x.Name).ToArray()));
        }

        [Test]
        public void AdvanceMovesFirstPlayerToLastPosition()
        {
            PlayerDeque deque = new PlayerDeque(generator, playerFactory);
            var first = deque[0];
            deque.AdvanceDeque();
            Assert.AreEqual(first, deque[deque.Count - 1]);
        }
    }
}
